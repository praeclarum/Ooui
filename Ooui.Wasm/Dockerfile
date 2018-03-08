FROM alpine:3.7

RUN apk update \
    && apk upgrade \
    && apk add --no-cache g++ musl-dev make cmake git subversion python

RUN mkdir /mono-wasm \
    && cd /mono-wasm \
    && git clone --depth 1 https://github.com/lrz/mono-wasm.git build

# RUN cd /mono-wasm \
#     && svn co --quiet http://llvm.org/svn/llvm-project/llvm/trunk llvm \
#     && cd llvm/tools \
#     && svn co --quiet http://llvm.org/svn/llvm-project/cfe/trunk clang \
#     && svn co --quiet http://llvm.org/svn/llvm-project/lld/trunk lld

RUN cd /mono-wasm \
    && wget http://releases.llvm.org/5.0.1/llvm-5.0.1.src.tar.xz \
    && unxz llvm-5.0.1.src.tar.xz \
    && tar xf llvm-5.0.1.src.tar \
    && rm llvm-5.0.1.src.tar \
    && mv llvm-5.0.1.src llvm \
    && cd llvm/tools \
    && wget http://releases.llvm.org/5.0.1/cfe-5.0.1.src.tar.xz \
    && unxz cfe-5.0.1.src.tar.xz \
    && tar xf cfe-5.0.1.src.tar \
    && rm cfe-5.0.1.src.tar \
    && mv cfe-5.0.1.src clang \
    && wget http://releases.llvm.org/5.0.1/lld-5.0.1.src.tar.xz \
    && unxz lld-5.0.1.src.tar.xz \
    && tar xf lld-5.0.1.src.tar \
    && rm lld-5.0.1.src.tar \
    && mv lld-5.0.1.src lld

RUN mkdir /mono-wasm/llvm-build \
    && cd /mono-wasm/llvm-build \
    && cmake -G "Unix Makefiles" -DCMAKE_C_COMPILER=/usr/bin/gcc -DCMAKE_CXX_COMPILER=/usr/bin/g++ -DLLVM_TARGETS_TO_BUILD=X86 -DLLVM_EXPERIMENTAL_TARGETS_TO_BUILD=WebAssembly -DCMAKE_BUILD_TYPE=Release ../llvm \
    && make -j7

RUN cd /mono-wasm \
    && git clone --depth 1 https://github.com/mono/llvm.git llvm-mono

ENV CXXFLAGS="-fpermissive"

RUN mkdir /mono-wasm/llvm-mono-build \
    && cd /mono-wasm/llvm-mono-build \
    && cmake -G "Unix Makefiles" -DCMAKE_OSX_ARCHITECTURES="i386;x86_64" -DLLVM_TARGETS_TO_BUILD=X86 ../llvm-mono \
    && cp -R ../llvm-mono/include/* include/ \
    && sed -i "s/#if defined(_LARGEFILE64_SOURCE) || defined(_GNU_SOURCE)/#if 0/" /usr/include/stdio.h \
    && sed -i "s/#if defined(_LARGEFILE64_SOURCE) || defined(_GNU_SOURCE)/#if 0/" /usr/include/sys/stat.h \
    # && sed -i "s/namespace llvm/#undef fopen\n#undef fopen64\n#undef fseeko\n#undef fseeko64\n#undef fstat\n#undef fstat64\n#undef ftello\n#undef ftello64\n#undef lstat\n#undef lstat64\n#undef stat\n#undef stat64\n#undef tmpfile\n#undef tmpfile64\n\nnamespace llvm/" /mono-wasm/llvm-mono-build/include/llvm/Target/TargetLibraryInfo.h
    && make -j7

ENV CXXFLAGS=""

RUN cd /mono-wasm \
    && git clone --depth 1 https://github.com/lrz/mono-wasm-mono.git mono-compiler

RUN apk add --no-cache bash autoconf automake libtool linux-headers \
    && cd /mono-wasm/mono-compiler \
    && ./autogen.sh --with-cross-offsets=offsets-wasm32.h CFLAGS="-DCOMPILE_WASM32 -DMONO_CROSS_COMPILE" CXXFLAGS="-DCOMPILE_WASM32 -DMONO_CROSS_COMPILE" --disable-boehm --with-sigaltstack=no --enable-llvm --enable-llvm-runtime --with-llvm=../llvm-mono-build --disable-btls --with-runtime_preset=testing_aot_full

# RUN cd /mono-wasm/mono-compiler/eglib \
#     && make -j7 \
#     && cd ../mono \
#     && make -j7
