OOUI_SRCS=$(wildcard ../Ooui/*.cs)

all: hello.exe output/index.wasm output/index.html

hello.exe: hello.cs Ooui.dll Makefile
	mcs -nostdlib -noconfig -r:mono-wasm-macos/dist/lib/mscorlib.dll -r:Ooui.dll hello.cs -out:hello.exe

Ooui.dll: $(OOUI_SRCS) System.Core.cs Makefile
	mcs -nostdlib -noconfig -r:mono-wasm-macos/dist/lib/mscorlib.dll -define:NO_PROCESS,NO_SERVER,NO_XML System.Core.cs $(OOUI_SRCS) -out:Ooui.dll

output/index.wasm:      hello.exe Makefile
	mono-wasm-macos/dist/bin/mono-wasm -i hello.exe -o output

output/index.html:      index.html
	cp index.html output

clean:
	rm -rf build output hello.exe
