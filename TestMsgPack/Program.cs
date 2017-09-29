using System;
using System.IO;
using System.Runtime.Serialization;
using MsgPack;
using MsgPack.Serialization;

namespace TestMsgPack
{
	
	[DataContract]
	public class Foo
	{
	}

	public class Bar
	{
		[MessagePackMember(0)]
		public Foo foo;
	}

	public class Parent
	{
		public int a = 10;
		public string b = "haha";
	}

	public class Child :Parent
	{
		public bool c = true;
	}

	public class ParentSerializer : MessagePackSerializer<Parent>
	{
		protected override void PackToCore( Packer packer, Parent objectTree )
		{
			packer.Pack( objectTree.a );
			packer.Pack( objectTree.b );

		}

		protected override Parent UnpackFromCore( Unpacker unpacker )
		{
			return null;
		}

	}

	class Program
	{
				
		static void Main( string[] args )
		{
			SerializationContext.Default.CompatibilityOptions.AllowAsymmetricSerializer = true;
			Console.WriteLine( "Hello World!" );
			
			var bar = new Bar();
			bar.foo = new Foo();
			
			var stream = new MemoryStream();
			var serializer = MessagePackSerializer.Get<Bar>();
			serializer.Pack(stream, bar);

			stream.Position = 0;
			var unserialized = serializer.Unpack( stream );
			
			Console.WriteLine(unserialized);
			Console.WriteLine(serializer.ToMessagePackObject(bar));
			

		}
	}
}
