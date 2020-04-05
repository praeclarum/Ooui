// Ooui v1.0.0

var debug = false;

let nodes = {};
let hasText = {};

let socket = null;
let wasmSession = null;

let lastRootElementPath = "";

function send (json) {
    if (debug) console.log ("Send", json);
    if (socket != null) {
        socket.send (json);
    }
    else if (wasmSession != null) {
        App.receiveMessagesJson (wasmSession, json);
    }
}

const mouseEvents = {
    click: true,
    dblclick: true,
    mousedown: true,
    mouseenter: true,
    mouseleave: true,
    mousemove: true,
    mouseout: true,
    mouseover: true,
    mouseup: true,
    wheel: true,
};

const inputEvents = {
    input: true,
    change: true,
    keyup: true,
};

const elementEvents = {
    load: true
}

function getSize () {
    return {
        height: window.innerHeight,
        width: window.innerWidth
    };
}

function setCookie (name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date ();
        date.setTime(date.getTime () + (days*24*60*60*1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "")  + expires + "; path=/";
}

function saveSize (s) {
    setCookie ("oouiWindowWidth", s.width, 7);
    setCookie ("oouiWindowHeight", s.height, 7);
}

function initializeNavigation() {
    monitorHashChanged();
    const em = {
        m: "event",
        id: "window",
        k: "hashchange",
        v: window.location
    };
    saveSize(em.v);
    const ems = JSON.stringify(em);
    send(ems);
    if (debug) console.log("Event", em);
}

let isWindowLoaded = false;
window.addEventListener("load", function () {
    connectWebSocket();
    isWindowLoaded = true;
});

// Main entrypoint
function ooui(rootElementPath) {
    lastRootElementPath = rootElementPath;
    if (isWindowLoaded) {
        connectWebSocket();
    }
}

let reloadTryCount = 0;
let reloadRequestTime = 0;
function reloadSocket() {
    const now = (new Date()).getTime ();
    if (now - reloadRequestTime > 0) {
        reloadRequestTime = now + 100;
        reloadTryCount++;
        window.setTimeout(function () {
            connectWebSocket();
        }, 10);
    }
}

function connectWebSocket() {
    rootElementPath = lastRootElementPath;
    if (rootElementPath.length === 0)
        return;

    console.log("Initializing Ooui web socket");

    if (reloadTryCount > 0) {
        let $body = getBodyNode();
        while ($body.firstChild)
            $body.removeChild($body.lastChild);
        nodes = {};
        hasText = {};
    }

    var initialSize = getSize ();
    saveSize (initialSize);

    var wsArgs = (rootElementPath.indexOf("?") >= 0 ? "&" : "?") +
        "w=" + initialSize.width + "&h=" + initialSize.height;

    var proto = "ws";
    if (location.protocol == "https:") {
        proto = "wss";
    }

    socket = new WebSocket (proto + "://" + document.location.host + rootElementPath + wsArgs, "ooui");

    let socketOpened = false;

    socket.addEventListener ("open", function (event) {
        console.log("Web socket opened");
        socketOpened = true;
        initializeNavigation();
    });

    socket.addEventListener ("error", function (event) {
        console.error ("Web socket error", event);
    });

    socket.addEventListener ("close", function (event) {
        //console.error("Web socket close", event);
        if (socketOpened) {
            reloadSocket();
        }
    });

    socket.addEventListener("message", function (event) {
        const messages = JSON.parse (event.data);
        if (debug) console.log("Messages", messages);
        if (Array.isArray (messages)) {
            messages.forEach (function (m) {
                // console.log('Raw value from server', m.v);
                m.v = fixupValue (m.v);
                processMessage (m);
            });
        }
    });

    console.log("Web socket created");

    monitorSizeChanges (1000/10);
}

function monitorHashChanged() {
    function hashChangeHandler() {
        const em = {
            m: "event",
            id: "window",
            k: "hashchange",
            v: window.location
        };
        saveSize(em.v);
        const ems = JSON.stringify(em);
        send(ems);
        if (debug) console.log("Event", em);
    }

    window.addEventListener("hashchange", hashChangeHandler, false);
}

function monitorSizeChanges (millis)
{
    var resizeTimeout;
    function resizeThrottler() {
        if (!resizeTimeout) {
            resizeTimeout = setTimeout(function() {
                resizeTimeout = null;
                resizeHandler();            
            }, millis);
        }
    }

    function resizeHandler() {
        const em = {
            m: "event",
            id: "window",
            k: "resize",
            v: getSize (),
        };
        saveSize (em.v);
        const ems = JSON.stringify (em);
        send (ems);
        if (debug) console.log ("Event", em);
    }

    window.addEventListener("resize", resizeThrottler, false);
}

function getBodyNode() {
    const bodyNode = document.getElementById("ooui-body");
    return bodyNode || document.body;
}

function getNode (id) {
    switch (id) {
        case "window": return window;
        case "document": return document;
        case "document.body": return getBodyNode();
        default: return nodes[id];
    }
}

function getOrCreateElement (id, tagName) {
    var e = document.getElementById (id);
    if (e) {
        if (e.firstChild && e.firstChild.nodeType == Node.TEXT_NODE)
            hasText[e.id] = true;
        return e;
    }
    return document.createElement (tagName);
}

function msgCreate (m) {
    const id = m.id;
    const tagName = m.k;
    const node = tagName === "#text" ?
        document.createTextNode ("") :
        getOrCreateElement (id, tagName);
    if (tagName !== "#text")
        node.id = id;
    nodes[id] = node;
    if (debug) console.log ("Created node", node);
}

function msgSet (m) {
    const id = m.id;
    const node = getNode (id);
    if (!node) {
        console.error ("Unknown node id", m);
        return;
    }
    const parts = m.k.split(".");
    let o = node;
    for (let i = 0; i < parts.length - 1; i++) {
        o = o[parts[i]];
    }
    const lastPart = parts[parts.length - 1];
    const value = lastPart === "htmlFor" ? m.v.id : m.v;
    o[lastPart] = value;
    if (debug) console.log ("Set", node, parts, value);
}

function msgSetAttr (m) {
    const id = m.id;
    const node = getNode (id);
    if (!node) {
        console.error ("Unknown node id", m);
        return;
    }
    node.setAttribute(m.k, m.v);
    if (debug) console.log ("SetAttr", node, m.k, m.v);
}

function msgRemAttr (m) {
    const id = m.id;
    const node = getNode (id);
    if (!node) {
        console.error ("Unknown node id", m);
        return;
    }
    node.removeAttribute(m.k);
    if (debug) console.log ("RemAttr", node, m.k);
}

function getCallerProperty(target, accessorStr) {
    const arr = accessorStr.split('.');
    var caller = target; 
    var property = target;
    arr.forEach(function (v) {
        caller = property;
        property = caller[v];
    });
    return [caller, property];
}

function msgCall (m) {
    const id = m.id;
    const node = getNode (id);
    if (!node) {
        console.error ("Unknown node id", m);
        return;
    }
    const target = node;
    if (m.k === "insertBefore" && m.v[0].nodeType == Node.TEXT_NODE && m.v[1] == null && hasText[id]) {
        // Text is already set so it clear it first
        if (target.firstChild)
            target.removeChild (target.firstChild);
        delete hasText[id];
    }
    //const f = target[m.k];
    const f = getCallerProperty(target, m.k);
    if (debug) console.log ("Call", node, f, m.v);
    const r = f[1].apply (f[0], m.v);
    if (typeof m.rid === 'string' || m.rid instanceof String) {
        nodes[m.rid] = r;
    }
}

function msgListen (m) {
    const node = getNode (m.id);
    if (!node) {
        console.error ("Unknown node id", m);
        return;
    }
    if (debug) console.log ("Listen", node, m.k);
    node.addEventListener(m.k, function (e) {
        const em = {
            m: "event",
            id: m.id,
            k: m.k,
        };
        if (inputEvents[m.k]) {
            em.v = (node.tagName === "INPUT" && node.type === "checkbox") ?
                node.checked :
                node.value;
        }
        else if (mouseEvents[m.k]) {
            em.v = {
                offsetX: e.offsetX,
                offsetY: e.offsetY,
            };
        }
        else if (elementEvents[m.k]) {
            em.v = {
                clientHeight: node.clientHeight,
                clientWidth: node.clientWidth
            }
        }
        const ems = JSON.stringify (em);
        send (ems);
        if (debug) console.log ("Event", em);
        if (em.k === "submit")
            e.preventDefault ();
    });
}

function processMessage (m) {
    switch (m.m) {
        case "nop":
            break;
        case "create":
            msgCreate (m);
            break;
        case "set":
            msgSet (m);
            break;
        case "setAttr":
            msgSetAttr (m);
            break;
        case "remAttr":
            msgRemAttr (m);
            break;
        case "call":
            msgCall (m);
            break;
        case "listen":
            msgListen (m);
            break;
        default:
            console.error ("Unknown message type", m.m, m);
    }
}

function fixupValue (v) {
    var x, n;
    if (Array.isArray (v)) {
        for (x in v) {
            v[x] = fixupValue (v[x]);
        }
        return v;
    }
    else if (typeof v === 'string' || v instanceof String) {
        if ((v.length > 1) && (v[0] === "\u2999")) {
            // console.log("V", v);
            return getNode (v);
        }
    }
    else if (!!v && v.hasOwnProperty("id") && v.hasOwnProperty("k")) {
        return fixupValue(v["id"])[v["k"]];
    }
    return v;
}

// == WASM Support ==

config.remote_managedpath = "managed";
config.assemblyFileExtension = "dll";
config.mono_wasm_runtime = "dotnet.wasm";
config.files_integrity = {};

config.fetch_file_cb = asset => App.fetchFile(asset);
config.environmentVariables = config.environmentVariables || {};


function oouiWasm (mainAsmName, mainNamespace, mainClassName, mainMethodName)
{
    Module.entryPoint = { "a": mainAsmName, "n": mainNamespace, "t": mainClassName, "m": mainMethodName };
    config.ooui_main = "[" + mainAsmName + "] " +
        mainNamespace + "." +
        mainClassName + ":" +
        mainMethodName;

    Module.assemblies = config.file_list;

    initializeNavigation();
    monitorSizeChanges (1000/30);
}

window["__oouiReceiveMessages"] = function (sessionId, messages)
{
    if (debug) console.log ("WebAssembly Receive", messages);
    if (wasmSession != null) {
        messages.forEach (function (m) {
            // console.log ('Raw value from server', m.v);
            m.v = fixupValue (m.v);
            processMessage (m);
        });
    }
};


var Module = {
    onRuntimeInitialized: function () {

        if (config.environmentVariables) {
            for (let key in config.environmentVariables) {
                if (config.environmentVariables.hasOwnProperty(key)) {
                    if (config.enable_debugging) console.log(`Setting ${key}=${config.environmentVariables[key]}`);
                    MONO.mono_wasm_setenv(key, config.environmentVariables[key]);
                }
            }
        }

        MONO.mono_load_runtime_and_bcl(
            config.vfs_prefix,
            config.deploy_prefix,
            config.enable_debugging,
            config.file_list,
            function () {
                App.init();
            },
            config.fetch_file_cb
        );
    },
    instantiateWasm: function (imports, successCallback) {

        // There's no way to get the filename from mono.js right now.
        // so we just hardcode it.
        const wasmUrl = config.mono_wasm_runtime || "mono.wasm";

        if (ENVIRONMENT_IS_NODE) {
            return WebAssembly
                .instantiate(getBinary(), imports)
                .then(results => {
                    successCallback(results.instance, results.module);
                });
        } else if (typeof WebAssembly.instantiateStreaming === 'function') {
            App.fetchWithProgress(
                wasmUrl,
                loaded => App.reportProgressWasmLoading(loaded))
                .then(response => {
                    if (Module.isElectron()) {
                        /*
                         * Chromium does not yet suppport instantiateStreaming
                         * with custom headers.
                         */
                        return response.arrayBuffer()
                            .then(buffer => {
                                WebAssembly
                                    .instantiate(buffer, imports)
                                    .then(results => {
                                        successCallback(results.instance, results.module);
                                    });
                            });
                    }
                    else {
                        return WebAssembly
                            .instantiateStreaming(response, imports)
                            .then(results => {
                                successCallback(results.instance, results.module);
                            });
                    }
                });
        }
        else {
            fetch(wasmUrl)
                .then(response => {
                    response.arrayBuffer().then(function (buffer) {
                        return WebAssembly.instantiate(buffer, imports)
                            .then(results => {
                                successCallback(results.instance);
                            });
                    });
                });
        }

        return {}; // Compiling asynchronously, no exports.
    },
    isElectron: function () {
        return navigator.userAgent.indexOf('Electron') !== -1;
    }
};










var MonoRuntime = {
    init: function () {
        this.load_runtime = Module.cwrap ('mono_wasm_load_runtime', null, ['string', 'number']);
        this.assembly_load = Module.cwrap ('mono_wasm_assembly_load', 'number', ['string']);
        this.find_class = Module.cwrap ('mono_wasm_assembly_find_class', 'number', ['number', 'string', 'string']);
        this.find_method = Module.cwrap ('mono_wasm_assembly_find_method', 'number', ['number', 'string', 'number']);
        this.invoke_method = Module.cwrap ('mono_wasm_invoke_method', 'number', ['number', 'number', 'number']);
        this.mono_string_get_utf8 = Module.cwrap ('mono_wasm_string_get_utf8', 'number', ['number']);
        this.mono_string = Module.cwrap ('mono_wasm_string_from_js', 'number', ['string']);
        this.mono_wasm_obj_array_new = Module.cwrap("mono_wasm_obj_array_new", "number", ["number"]);
        this.mono_wasm_obj_array_set = Module.cwrap("mono_wasm_obj_array_set", null, ["number", "number", "number"]);
    },

    conv_string: function (mono_obj) {
        if (mono_obj == 0)
            return null;
        var raw = this.mono_string_get_utf8 (mono_obj);
        var res = Module.UTF8ToString (raw);
        Module._free (raw);

        return res;
    },

    call_method: function (method, this_arg, args) {
        var args_mem = Module._malloc (args.length * 4);
        var eh_throw = Module._malloc (4);
        for (var i = 0; i < args.length; ++i)
            Module.setValue (args_mem + i * 4, args [i], "i32");
        Module.setValue (eh_throw, 0, "i32");

        var res = this.invoke_method (method, this_arg, args_mem, eh_throw);

        var eh_res = Module.getValue (eh_throw, "i32");

        Module._free (args_mem);
        Module._free (eh_throw);

        if (eh_res != 0) {
            var msg = this.conv_string (res);
            throw new Error (msg);
        }

        return res;
    },
};

var App = {
    init: function () {
        // WebAssemblyApp.attachDebuggerHotkey(config.file_list);
        MonoRuntime.init();
        BINDING.bindings_lazy_init();

        this.findMethods ();

        this.oouiPreInit ("1", "2");
        this.mainInit ();
        this.oouiFinalInit ("1", "2");
        this.cleanupInit ();
    },

    oouiPreInit: function (a, b) {
        try {
            if (!!this.ooui_DisableServer_method) {
                MonoRuntime.call_method (this.ooui_DisableServer_method, null, []);
            }
        } catch (e) {
            console.error(e);
        }
    },

    mainInit: function () {
        try {
            var mainMethod = BINDING.resolve_method_fqn(config.ooui_main);

            if (typeof mainMethod === "undefined") {
                throw `Unable to find entrypoint in ${config.ooui_main}`;
            }

            signature = Module.mono_method_get_call_signature(mainMethod);

            if (signature === "") {
                BINDING.call_method(mainMethod, null, signature, []);
            } else {
                let array = ENVIRONMENT_IS_NODE
                    ? BINDING.js_array_to_mono_array(process.argv)
                    : BINDING.js_array_to_mono_array([]);

                MonoRuntime.call_method (mainMethod, null, [array]);
            }
        } catch (e) {
            console.error(e);
        }
    },

    oouiFinalInit: function (a, b) {
        try {
            var sessionId = "main";
            wasmSession = sessionId;
            if (!!this.ooui_StartWebAssemblySession_method) {
                var initialSize = getSize ();
                MonoRuntime.call_method (this.ooui_StartWebAssemblySession_method, null, [MonoRuntime.mono_string (sessionId), MonoRuntime.mono_string ("/"), MonoRuntime.mono_string (Math.round(initialSize.width) + " " + Math.round(initialSize.height))]);
            }
        } catch (e) {
            console.error(e);
        }
    },

    cleanupInit: function () {
        var loading = document.getElementById ("loading");

        if (loading && loading.parentNode) {
            loading.parentNode.removeChild(loading);
        }
    },

    receiveMessagesJson: function (sessionId, json) {
        if (!!this.ooui_ReceiveWebAssemblySessionMessageJson_method) {
            MonoRuntime.call_method (this.ooui_ReceiveWebAssemblySessionMessageJson_method, null, [MonoRuntime.mono_string (sessionId), MonoRuntime.mono_string (json)]);
        }
    },

    findMethods: function () {

        this.ooui_DisableServer_method = BINDING.resolve_method_fqn("[Ooui] Ooui.UI:DisableServer");
        if (!this.ooui_DisableServer_method)
            throw "Could not find DisableServer method";

        this.ooui_module = MonoRuntime.assembly_load ("Ooui");
        if (!!this.ooui_module) {

            this.ooui_class = MonoRuntime.find_class (this.ooui_module, "Ooui", "UI");
            if (!this.ooui_class)
                throw "Could not find UI class in Ooui module";

            this.ooui_StartWebAssemblySession_method = MonoRuntime.find_method (this.ooui_class, "StartWebAssemblySession", -1);
            if (!this.ooui_StartWebAssemblySession_method)
                throw "Could not find StartWebAssemblySession method";

            this.ooui_ReceiveWebAssemblySessionMessageJson_method = MonoRuntime.find_method (this.ooui_class, "ReceiveWebAssemblySessionMessageJson", -1);
            if (!this.ooui_ReceiveWebAssemblySessionMessageJson_method)
                throw "Could not find ReceiveWebAssemblySessionMessageJson method";
        }
    },


    fetchWithProgress: function (url, progressCallback) {

        if (!this.loader) {
            // No active loader, simply use the fetch API directly...
            return fetch(url, this.getFetchInit(url));
        }

        return fetch(url, this.getFetchInit(url))
            .then(response => {
                if (!response.ok) {
                    throw Error(`${response.status} ${response.statusText}`);
                }

                try {
                    let loaded = 0;

                    // Wrap original stream with another one, while reporting progress.
                    const stream = new ReadableStream({
                        start(ctl) {
                            const reader = response.body.getReader();

                            read();

                            function read() {
                                reader.read()
                                    .then(
                                        ({ done, value }) => {
                                            if (done) {
                                                ctl.close();
                                                return;
                                            }
                                            loaded += value.byteLength;
                                            progressCallback(loaded, value.byteLength);
                                            ctl.enqueue(value);
                                            read();
                                        })
                                    .catch(error => {
                                        console.error(error);
                                        ctl.error(error);
                                    });
                            }
                        }
                    });

                    // We copy the previous response to keep original headers.
                    // Not only the WebAssembly will require the right content-type,
                    // but we also need it for streaming optimizations:
                    // https://bugs.chromium.org/p/chromium/issues/detail?id=719172#c28
                    return new Response(stream, response);
                }
                catch (ex) {
                    // ReadableStream may not be supported (Edge as of 42.17134.1.0)
                    return response;
                }
            })
            .catch(err => this.raiseLoadingError(err));
    },

    getFetchInit: function (url) {
        const fileName = url.substring(url.lastIndexOf("/") + 1);

        const init = { credentials: "omit" };

        if (config.files_integrity.hasOwnProperty(fileName)) {
            init.integrity = config.files_integrity[fileName];
        }

        return init;
    },

    fetchFile: function (asset) {

        if (asset.lastIndexOf(".dll") !== -1) {
            asset = asset.replace(".dll", `.${config.assemblyFileExtension}`);
        }

        asset = asset.replace("/managed/", `/${config.remote_managedpath}/`);

        return fetch(asset);
    },


};

