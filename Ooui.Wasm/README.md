
## Install

Download [Mono WebAssembly support on Jenkins](https://jenkins.mono-project.com//job/test-mono-mainline-webassembly/) by
grabbing the latest build's Azure Artifact.

[mono-wasm-03914603a3b.zip](https://jenkins.mono-project.com/job/test-mono-mainline-webassembly/71/label=highsierra/Azure/processDownloadRequest/71/highsierra/sdks/wasm/mono-wasm-03914603a3b.zip)

Expand that into this directory.



## Build

```bash
csc /nostdlib /target:library /r:managed/mscorlib.dll /r:managed/Ooui.dll /out:managed/Ooui.Sample.dll ooui-sample.cs
```


## Run

```bash
python server.py
```

Go to `locahost:8000/ooui.html`
