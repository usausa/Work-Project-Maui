rmdir /s /q Publish
dotnet publish WorkMauiAot\WorkMauiAot.csproj -f:net10.0-android -c:Release -nodeReuse:false /NoWarn:XA4301,XA0141 -p:PublishDir="..\Publish"
