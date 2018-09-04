# Updating the Ooui.Wasm.Build.Task

The build task uses the Web Assembly SDK and the Mono Linker to build a WASM app, which often requires updating these modules. Updating the SDK is done in the `BuildDistTask` file by replacing the string content of the `SdkUrl` field:

```c#
const string SdkUrl = "PATH/TO/MonoWasmSDK.zip";
```

The build outputs from the mono project, i.e. the Web Assembly build artefacts can be found [here](https://jenkins.mono-project.com/job/test-mono-mainline-wasm/).