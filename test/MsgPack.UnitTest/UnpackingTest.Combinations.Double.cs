﻿
#region -- License Terms --
//
// MessagePack for CLI
//
// Copyright (C) 2010-2012 FUJIWARA, Yusuke
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
#if !MSTEST
using NUnit.Framework;
#else
using TestFixtureAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using TestAttribute = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
using TimeoutAttribute = NUnit.Framework.TimeoutAttribute;
using Assert = NUnit.Framework.Assert;
using Is = NUnit.Framework.Is;
#endif

namespace MsgPack
{
	[TestFixture]
	public partial class UnpackingTest_Combinations_Double
	{
		[Test]
		public void TestUnpackDouble_ByteArray_SingleMinValue_AsIs()
		{
			var result = Unpacking.UnpackDouble( new byte[] { 0xCA, 0xFF, 0x7F, 0xFF, 0xFF } );
			Assert.AreEqual( sizeof( System.Single ) + 1, result.ReadCount );
			Assert.IsTrue( ( ( double )Single.MinValue ).Equals(result.Value ) );
		}
		
		[Test]
		public void TestUnpackDouble_Stream_SingleMinValue_AsIs()
		{
			using ( var buffer = new MemoryStream( new byte[] { 0xCA, 0xFF, 0x7F, 0xFF, 0xFF } ) )
			{
				var result = Unpacking.UnpackDouble( buffer );
				Assert.AreEqual( sizeof( System.Single ) + 1, buffer.Position );
				Assert.IsTrue( ( ( double )Single.MinValue ).Equals(result ) );
			}
		}
		
		[Test]
		public void TestUnpackDouble_ByteArray_SingleMaxValue_AsIs()
		{
			var result = Unpacking.UnpackDouble( new byte[] { 0xCA, 0x7F, 0x7F, 0xFF, 0xFF } );
			Assert.AreEqual( sizeof( System.Single ) + 1, result.ReadCount );
			Assert.IsTrue( ( ( double )Single.MaxValue ).Equals(result.Value ) );
		}
		
		[Test]
		public void TestUnpackDouble_Stream_SingleMaxValue_AsIs()
		{
			using ( var buffer = new MemoryStream( new byte[] { 0xCA, 0x7F, 0x7F, 0xFF, 0xFF } ) )
			{
				var result = Unpacking.UnpackDouble( buffer );
				Assert.AreEqual( sizeof( System.Single ) + 1, buffer.Position );
				Assert.IsTrue( ( ( double )Single.MaxValue ).Equals(result ) );
			}
		}
		
		[Test]
		public void TestUnpackDouble_ByteArray_SingleZero_AsIs()
		{
			var result = Unpacking.UnpackDouble( new byte[] { 0xCA, 0x00, 0x00, 0x00, 0x00 } );
			Assert.AreEqual( sizeof( System.Single ) + 1, result.ReadCount );
			Assert.IsTrue( ( 0.0 ).Equals(result.Value ) );
		}
		
		[Test]
		public void TestUnpackDouble_Stream_SingleZero_AsIs()
		{
			using ( var buffer = new MemoryStream( new byte[] { 0xCA, 0x00, 0x00, 0x00, 0x00 } ) )
			{
				var result = Unpacking.UnpackDouble( buffer );
				Assert.AreEqual( sizeof( System.Single ) + 1, buffer.Position );
				Assert.IsTrue( ( 0.0 ).Equals(result ) );
			}
		}
		
		[Test]
		public void TestUnpackDouble_ByteArray_DoubleMinValue_AsIs()
		{
			var result = Unpacking.UnpackDouble( new byte[] { 0xCB, 0xFF, 0xEF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF } );
			Assert.AreEqual( sizeof( System.Double ) + 1, result.ReadCount );
			Assert.IsTrue( Double.MinValue.Equals(result.Value ) );
		}
		
		[Test]
		public void TestUnpackDouble_Stream_DoubleMinValue_AsIs()
		{
			using ( var buffer = new MemoryStream( new byte[] { 0xCB, 0xFF, 0xEF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF } ) )
			{
				var result = Unpacking.UnpackDouble( buffer );
				Assert.AreEqual( sizeof( System.Double ) + 1, buffer.Position );
				Assert.IsTrue( Double.MinValue.Equals(result ) );
			}
		}
		
		[Test]
		public void TestUnpackDouble_ByteArray_DoubleMaxValue_AsIs()
		{
			var result = Unpacking.UnpackDouble( new byte[] { 0xCB, 0x7F, 0xEF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF } );
			Assert.AreEqual( sizeof( System.Double ) + 1, result.ReadCount );
			Assert.IsTrue( Double.MaxValue.Equals(result.Value ) );
		}
		
		[Test]
		public void TestUnpackDouble_Stream_DoubleMaxValue_AsIs()
		{
			using ( var buffer = new MemoryStream( new byte[] { 0xCB, 0x7F, 0xEF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF } ) )
			{
				var result = Unpacking.UnpackDouble( buffer );
				Assert.AreEqual( sizeof( System.Double ) + 1, buffer.Position );
				Assert.IsTrue( Double.MaxValue.Equals(result ) );
			}
		}
		
		[Test]
		public void TestUnpackDouble_ByteArray_DoubleZero_AsIs()
		{
			var result = Unpacking.UnpackDouble( new byte[] { 0xCB, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } );
			Assert.AreEqual( sizeof( System.Double ) + 1, result.ReadCount );
			Assert.IsTrue( ( 0.0 ).Equals(result.Value ) );
		}
		
		[Test]
		public void TestUnpackDouble_Stream_DoubleZero_AsIs()
		{
			using ( var buffer = new MemoryStream( new byte[] { 0xCB, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } ) )
			{
				var result = Unpacking.UnpackDouble( buffer );
				Assert.AreEqual( sizeof( System.Double ) + 1, buffer.Position );
				Assert.IsTrue( ( 0.0 ).Equals(result ) );
			}
		}
		
		[Test]
		public void TestUnpackDouble_ByteArray_Empty()
		{
			Assert.Throws<ArgumentException>( () => Unpacking.UnpackDouble( new byte[ 0 ] ) );
		}

		[Test]
		public void TestUnpackDouble_ByteArray_Null()
		{
			Assert.Throws<ArgumentNullException>( () => Unpacking.UnpackDouble( default( byte[] ) ) );
		}

		[Test]
		public void TestUnpackDouble_ByteArray_Offset_Null()
		{
			Assert.Throws<ArgumentNullException>( () => Unpacking.UnpackDouble( default( byte[] ), 0 ) );
		}

		[Test]
		public void TestUnpackDouble_ByteArray_Offset_OffsetIsNegative()
		{
			Assert.Throws<ArgumentOutOfRangeException>( () => Unpacking.UnpackDouble( new byte[]{ 0x1 }, -1 ) );
		}

		[Test]
		public void TestUnpackDouble_ByteArray_Offset_OffsetIsTooBig()
		{
			Assert.Throws<ArgumentException>( () => Unpacking.UnpackDouble( new byte[]{ 0x1 }, 1 ) );
		}

		[Test]
		public void TestUnpackDouble_ByteArray_Offset_Empty()
		{
			Assert.Throws<ArgumentException>( () => Unpacking.UnpackDouble( new byte[ 0 ], 0 ) );
		}

		[Test]
		public void TestUnpackDouble_Stream_Null()
		{
			Assert.Throws<ArgumentNullException>( () => Unpacking.UnpackDouble( default( Stream ) ) );
		}

		[Test]
		public void TestUnpackDouble_ByteArray_Offset_OffsetIsValid_OffsetIsRespected()
		{
			// Offset 1 is Double 0.
			var result = Unpacking.UnpackDouble( new byte[] { 0xFF, 0xCB, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF }, 1 );
			Assert.AreEqual( sizeof( System.Double ) + 1, result.ReadCount );
			Assert.AreEqual( 0.0, result.Value );
		}

		[Test]
		public void TestUnpackDouble_ByteArray_Null_Nil()
		{
			Assert.Throws<MessageTypeException>( () => Unpacking.UnpackDouble( new byte[] { 0xC0 } ) );
		}
	
		[Test]
		public void TestUnpackDouble_ByteArray_NotDouble()
		{
			Assert.Throws<MessageTypeException>( () => Unpacking.UnpackDouble( new byte[] { 0xC3 } ) );
		}
	}
}