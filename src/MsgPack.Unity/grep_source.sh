grep '<Compile' MsgPack.Unity.csproj | grep -v AssemblyInfo.cs | sed "s/.*Include=\"\(.*\)\">/ditto '\1' 'MsgPack.Unity\/src\/\1'/g" | sed 's/\\/\//g' > copy_source.sh
