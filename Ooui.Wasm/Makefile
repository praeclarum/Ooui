
all:
	msbuild /t:Restore ../Ooui.Forms/Ooui.Forms.csproj
	msbuild ../Ooui.Forms/Ooui.Forms.csproj
	cp ../Ooui.Forms/bin/Debug/netstandard1.0/Ooui.Forms.dll managed/
	cp ../Ooui/bin/Debug/netstandard1.0/Ooui.dll managed/
	csc /nostdlib /target:library \
		/r:managed/mscorlib.dll /r:managed/System.Runtime.dll /r:managed/Xamarin.Forms.Core.dll /r:managed/Xamarin.Forms.Xaml.dll /r:managed/Ooui.dll /r:managed/Ooui.Forms.dll \
		/out:managed/Ooui.Sample.dll ../Samples/ISample.cs ../Samples/XamlPreviewPageSample.cs ooui-sample.cs
