all:
	msbuild /p:Configuration=Release /t:Restore ../Ooui.Wasm.Build.Tasks/Ooui.Wasm.Build.Tasks.csproj
	msbuild /p:Configuration=Release ../Ooui.Wasm.Build.Tasks/Ooui.Wasm.Build.Tasks.csproj
	nuget pack
