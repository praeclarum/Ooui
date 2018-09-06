# Updating the Ooui.Wasm.Build.Task

The build task uses the Web Assembly SDK and the Mono Linker to build a WASM app, which often requires updating these modules. Updating the SDK is done in the `BuildDistTask` file by replacing the string content of the `SdkUrl` field:

```c#
const string SdkUrl = "https://xamjenkinsartifact.azureedge.net/test-mono-mainline-wasm/{BUILDNUMBER}/ubuntu-1804-amd64/sdks/wasm/mono-wasm-{HASHVALUE}.zip";
```

The build outputs from the mono project, i.e. the Web Assembly build artefacts can be found [here](https://jenkins.mono-project.com/job/test-mono-mainline-wasm/). Make sure you do not reference the artifact directly! Instead use the Azure blob storage (used in the sample code above) and replace the `{BUILDNUMBER}` and `{HASHVALUE}` with the new values.
