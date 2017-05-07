#region -- License Terms --
//
// MessagePack for CLI
//
// Copyright (C) 2017 FUJIWARA, Yusuke
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
#endregion -- License Terms --

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
#if FEATURE_TAP
using System.Threading;
using System.Threading.Tasks;
#endif // FEATURE_TAP

namespace MsgPack
{
	/// <summary>
	///		<see cref="UnpackerReader"/> for <see cref="ArraySegment{T}"/> or array of bytes.
	/// </summary>
	internal sealed partial class ByteArrayUnpackerReader : UnpackerReader
	{
		private readonly byte[] _scalarBuffer = new byte[ 8 ];
		// TODO: Use Span<byte> and keep ArraySegment<byte>[] as _originalByteArraySegments for GetRemainingBytes();
		private readonly IList<ArraySegment<byte>> _sources;

		private int _currentSourceIndex;

		// TODO: Use Span<byte>
		private ArraySegment<byte> _currentSource;

#if DEBUG

		internal ArraySegment<byte> DebugSource
		{
			get { return this._sources[ this._currentSourceIndex ]; }
		}

		internal IList<ArraySegment<byte>> DebugBuffers
		{
			get { return this._sources; }
		}

#endif // DEBUG

		public override long Offset
		{
			get
			{
				return
					this._sources.Take( this._currentSourceIndex ).Sum( x => x.Count )
					+ this._currentSource.Offset - this._sources[ this._currentSourceIndex ].Offset;
			}
		}

		public int CurrentSourceOffset
		{
			get { return this._currentSource.Offset; }
		}

		public int CurrentSourceIndex
		{
			get { return this._currentSourceIndex; }
		}

		public override bool GetPreviousPosition( out long offsetOrPosition )
		{
			offsetOrPosition = this._sources[ this._currentSourceIndex ].Offset;
			return false;
		}

		// TODO: Use Span<byte>
		public ByteArrayUnpackerReader( ArraySegment<byte> source )
		{
			if ( source.Array == null || source.Count == 0)
			{
				throw new ArgumentException( "Source must have non null, non-empty Array.", "source" );
			}

			this._sources = new[] { source };
			this._currentSourceIndex = 0;
			this._currentSource = source;
		}

		// TODO: Use Span<byte>
		public ByteArrayUnpackerReader( IList<ArraySegment<byte>> sources, int startIndex, int startOffset )
		{
			if ( sources == null )
			{
				throw new ArgumentNullException( "source" );
			}

			if ( startIndex < 0 )
			{
				throw new ArgumentOutOfRangeException( "The value cannot be negative.", "startIndex" );
			}

			if ( startOffset < 0 )
			{
				throw new ArgumentOutOfRangeException( "The value cannot be negative.", "startOffset" );
			}

			if ( sources.Count == 0 )
			{
				throw new ArgumentException( "Sources cannot be empty.", "sources" );
			}

			if ( sources.Any( x => x.Array == null || x.Count == 0 ) )
			{
				throw new ArgumentException( "Sources contains null or empty Array.", "sources" );
			}

			if ( sources.Count <= startIndex )
			{
				throw new ArgumentException( "Sources is too small." );
			}

			this._sources = sources;
			this._currentSourceIndex = startIndex;
			var startSource = this._sources[ startIndex ];
			var skip = startOffset - startSource.Offset;
			if ( skip < 0 )
			{
				throw new ArgumentException( "The value cannot be smaller than the array segment Offset.", "startOffset" );
			}

			if ( skip > startSource.Count )
			{
				throw new ArgumentException( "The offset cannot exceed the array segment Count.", "startOffset" );
			}

			this._currentSource = startSource.Slice( skip );
		}

		// TODO: Use Span<byte>
		public bool TryRead( byte[] buffer, int requestedSize )
		{
			if ( requestedSize == 0 )
			{
				return true;
			}
			
			if ( this._currentSource.Count >= requestedSize )
			{
				// fast path
				// TODO: Use currentSource.CopyTo( buffer, requestedSize );
				Buffer.BlockCopy( this._currentSource.Array, this._currentSource.Offset, buffer, 0, requestedSize );
				this._currentSource = this._currentSource.Slice( requestedSize );
				return true;
			}

			return this.TryReadSlow( buffer, requestedSize );
		}

		// TODO: Use Span<T>
		private bool TryReadSlow( byte[] buffer, int requestedSize )
		{
			var currentSourceIndex = this._currentSourceIndex;

			if ( this._currentSource.Count == 0 )
			{
				// try shift to next buffer
				currentSourceIndex++;
				if ( this._sources.Count == currentSourceIndex )
				{
					return false;
				}

				this._currentSource = this._sources[ currentSourceIndex ];
			}

			var remaining = requestedSize;
			var destinationOffset = 0;
			int copying;

			do
			{
				copying = Math.Min( this._currentSource.Count, remaining );
				// TODO: Use currentSource.CopyTo( buffer, copying );
				Buffer.BlockCopy( this._currentSource.Array, this._currentSource.Offset, buffer, destinationOffset, copying );
				remaining -= copying;
				if ( remaining <= 0 )
				{
					// Finish
					break;
				}

				destinationOffset += copying;

				// try shift to next buffer
				currentSourceIndex++;
				if ( this._sources.Count == currentSourceIndex )
				{
					return false;
				}

				this._currentSource = this._sources[ currentSourceIndex ];
			} while ( true );

			this._currentSourceIndex = currentSourceIndex;
			this._currentSource = this._currentSource.Slice( copying );
			return true;
		}

		// TODO: Use Span<T>
		public override void Read( byte[] buffer, int size )
		{
			if ( !this.TryRead( buffer, size ) )
			{
				this.ThrowEofException( size );
			}
		}

#if FEATURE_TAP

		public override Task ReadAsync( byte[] buffer, int size, CancellationToken cancellationToken )
		{
			this.Read( buffer, size );
			return TaskAugument.CompletedTask;
		}

#endif // FEATURE_TAP

		public override string ReadString( int length )
		{
			if ( length == 0 )
			{
				return String.Empty;
			}
			
			if ( this._currentSource.Count - this._currentSource.Offset >= length )
			{
				// fast path
				var result = Encoding.UTF8.GetString( this._currentSource.Array, this._currentSource.Offset, length );
				this._currentSource = this._currentSource.Slice( length );
				return result;
			}

			// Slow path
			return this.ReadStringSlow( length );
		}

		private string ReadStringSlow( int requestedSize )
		{
			var currentSourceIndex = this._currentSourceIndex;

			if ( this._currentSource.Count == 0 )
			{
				// try shift to next buffer
				currentSourceIndex++;
				if ( this._sources.Count == currentSourceIndex )
				{
					this.ThrowEofException( requestedSize );
				}

				this._currentSource = this._sources[ currentSourceIndex ];
			}

			var remaining = requestedSize;
			int decoding = 0;

			var decoder = Encoding.UTF8.GetDecoder();
			var charBuffer = BufferManager.NewCharBuffer( requestedSize );
			var result = new StringBuilder( requestedSize );
			bool isCompleted;
			do
			{
				decoding = Math.Min( this._currentSource.Count, remaining );
				isCompleted = decoder.DecodeString( this._currentSource.Array, this._currentSource.Offset, decoding, charBuffer, result );
				remaining -= decoding;
				if ( remaining <= 0 )
				{
					// Finish
					break;
				}

				// try shift to next buffer
				currentSourceIndex++;
				if ( this._sources.Count == currentSourceIndex )
				{
					this.ThrowEofException( requestedSize );
				}

				this._currentSource = this._sources[ currentSourceIndex ];
			} while ( true );

			if ( !isCompleted )
			{
				this.ThrowBadUtf8Exception();
			}

			this._currentSourceIndex = currentSourceIndex;
			this._currentSource = this._currentSource.Slice( decoding );
			return result.ToString();
		}


#if FEATURE_TAP

		public override Task<string> ReadStringAsync( int length, CancellationToken cancellationToken )
		{
			return Task.FromResult( this.ReadString( length ) );
		}

#endif // FEATURE_TAP

		public override bool Drain( uint size )
		{
			if ( size == 0 )
			{
				// 0 byte drain always success.
				return true;
			}

			long remaining = size;

			var currentSourceIndex = this._currentSourceIndex;

			if ( this._currentSource.Count == 0 )
			{
				// try shift to next buffer
				currentSourceIndex++;
				if ( this._sources.Count == currentSourceIndex )
				{
					return false;
				}

				this._currentSource = this._sources[ currentSourceIndex ];
			}

			while ( remaining > 0 )
			{
				var currentSourceSize = this._currentSource.Count;
				if ( remaining <= currentSourceSize )
				{
					this._currentSourceIndex = currentSourceIndex;
					this._currentSource = this._currentSource.Slice( unchecked( ( int )remaining) );
					return true;
				}

				remaining -= currentSourceSize;
				// try shift to next buffer
				currentSourceIndex++;
				if ( this._sources.Count == currentSourceIndex )
				{
					break;
				}

				this._currentSource = this._sources[ currentSourceIndex ];
			}

			return false;
		}

#if FEATURE_TAP

		public override Task<bool> DrainAsync( uint size, CancellationToken cancellationToken )
		{
			return Task.FromResult( this.Drain( size ) );
		}

#endif // FEATURE_TAP

		private void ThrowEofException( long reading )
		{
			throw new InvalidMessagePackStreamException(
					String.Format(
						CultureInfo.CurrentCulture,
						"Data source unexpectedly ends. Cannot read {0:#,0} bytes at offset {1:#,0}, buffer index {2}.",
						reading,
						this._currentSource.Offset,
						this._currentSourceIndex
					)
				);
		}

		private void ThrowBadUtf8Exception()
		{
			throw new InvalidMessagePackStreamException(
					String.Format(
						CultureInfo.CurrentCulture,
						"Data source has invalid UTF-8 sequence. Last code point at offset {1:#,0}, buffer index {2} is not completed.",
						this._currentSource.Offset,
						this._currentSourceIndex
					)
				);
		}
	}
}
