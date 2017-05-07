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

namespace MsgPack
{
	/// <summary>
	///		Defines interface and basic functionality for byte array based <see cref="Packer"/>.
	/// </summary>
	public abstract class ByteArrayPacker : Packer
	{
		/// <summary>
		///		Gets the bytes used by this instance.
		/// </summary>
		/// <value>
		///		The bytes used by this instance. The initial state is <c>0</c>.
		/// </value>
		public abstract long BytesUsed { get; }

		/// <summary>
		///		Gets the current index of destination buffers.
		/// </summary>
		/// <value>
		///		The current index of destination buffers.
		/// </value>
		public abstract int CurrentBufferIndex { get; }

		/// <summary>
		///		Gets the current offset of the current destination buffe.
		/// </summary>
		/// <value>
		///		The current offset of the current destination buffe.
		/// </value>
		public abstract int CurrentBufferOffset { get; }

		/// <summary>
		///		Initializes a new instance of the <see cref="ByteArrayPacker"/> class.
		/// </summary>
		protected ByteArrayPacker() { }
	}

#warning TODO: Writer/Packer
	internal abstract class MessagePackPacker : IDisposable
	{
		protected MessagePackPacker() { }

		public void Dispose()
		{
			this.Dispose( true );
			GC.SuppressFinalize( this );
		}

		protected virtual void Dispose( bool disposing ) { }
	}

	internal sealed class MessagePackPacker<TWriter> : MessagePackPacker
		where TWriter : PackerWriter
	{
		public TWriter Writer { get; private set; }

		public MessagePackPacker( TWriter writer )
		{
			this.Writer = writer;
		}

		protected override void Dispose( bool disposing )
		{
			if ( disposing )
			{
				this.Writer.Dispose();
			}

			base.Dispose( disposing );
		}
	}

	internal abstract class PackerWriter : IDisposable
	{
		protected PackerWriter() { }

		public void Dispose()
		{
			this.Dispose( true );
			GC.SuppressFinalize( this );
		}

		protected virtual void Dispose( bool disposing ) { }

		public abstract void WriteByte( byte value );

		public abstract void WriteBytes( ushort value );

		public abstract void WriteBytes( uint value );

		public abstract void WriteBytes( ulong value );

		public abstract void WriteBytes( float value );

		public abstract void WriteBytes( double value );
		
		// TODO: ReadOnlySpan<byte>
		public abstract void WriteBytes( byte[] value );

		// TODO: ReadOnlySpan<char>
		public abstract void WriteBytes( string value ); // For Convert efficiency
	}
}
