// Ooui v1.0.0

var debug = true;

const nodes = {};
const hasText = {};

let socket = null;
let wasmSession = null;

function send (json) {
    if (debug) console.log ("Send", json);
    if (socket != null) {
        socket.send (json);
    }
    else if (wasmSession != null) {
        WebAssemblyApp.receiveMessagesJson (wasmSession, json);
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

// Main entrypoint
function ooui (rootElementPath) {

    var initialSize = getSize ();
    saveSize (initialSize);

    var wsArgs = (rootElementPath.indexOf("?") >= 0 ? "&" : "?") +
        "w=" + initialSize.width + "&h=" + initialSize.height;

    var proto = "ws";
    if (location.protocol == "https:") {
        proto = "wss";
    }

    socket = new WebSocket (proto + "://" + document.location.host + rootElementPath + wsArgs, "ooui");

    socket.addEventListener ("open", function (event) {
        console.log("Web socket opened");
        initializeNavigation();
    });

    socket.addEventListener ("error", function (event) {
        console.error("Web socket error", event);
    });

    socket.addEventListener ("close", function (event) {
        console.error ("Web socket close", event);
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

function oouiWasm (mainAsmName, mainNamespace, mainClassName, mainMethodName, assemblies)
{
    Module.entryPoint = { "a": mainAsmName, "n": mainNamespace, "t": mainClassName, "m": mainMethodName };
    Module.assemblies = assemblies;

    initializeNavigation();
    monitorSizeChanges (1000/30);
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

function getNode (id) {
    switch (id) {
        case "window": return window;
        case "document": return document;
        case "document.body":
            const bodyNode = document.getElementById ("ooui-body");
            return bodyNode || document.body;
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
    if (debug) console.log("Call", node, f, m.v);
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
        if (debug) console.log ("Done with WASM module instantiation.");

        Module.FS_createPath ("/", "managed", true, true);

        var pending = 0;
        var mangled_ext_re = new RegExp("\\.bin$");
        this.assemblies.forEach (function(asm_mangled_name) {
            var asm_name = asm_mangled_name.replace (mangled_ext_re, ".dll");
            if (debug) console.log ("Loading", asm_name);
            ++pending;
            fetch ("managed/" + asm_mangled_name, { credentials: 'same-origin' }).then (function (response) {
                if (!response.ok)
                    throw "failed to load Assembly '" + asm_name + "'";
                return response['arrayBuffer']();
            }).then (function (blob) {
                var asm = new Uint8Array (blob);
                Module.FS_createDataFile ("managed/" + asm_name, null, asm, true, true, true);
                --pending;
                if (pending == 0)
                    Module.bclLoadingDone ();
            });
        });
    },

    bclLoadingDone: function () {
        if (debug) console.log ("Done loading the BCL.");
        MonoRuntime.init ();
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

        this.load_runtime ("managed", 1);

        if (debug) console.log ("Done initializing the runtime.");

        WebAssemblyApp.init ();
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

var WebAssemblyApp = {
    init: function () {
        this.loading = document.getElementById ("loading");

        this.findMethods ();

        this.runApp ("1", "2");

        this.loading.hidden = true;
    },

    runApp: function (a, b) {
        try {
            var sessionId = "main";
            if (!!this.ooui_DisableServer_method) {
                MonoRuntime.call_method (this.ooui_DisableServer_method, null, []);
            }
            MonoRuntime.call_method (this.main_method, null, [MonoRuntime.mono_string (a), MonoRuntime.mono_string (b)]);
            wasmSession = sessionId;
            if (!!this.ooui_StartWebAssemblySession_method) {
                var initialSize = getSize ();
                MonoRuntime.call_method (this.ooui_StartWebAssemblySession_method, null, [MonoRuntime.mono_string (sessionId), MonoRuntime.mono_string ("/"), MonoRuntime.mono_string (Math.round(initialSize.width) + " " + Math.round(initialSize.height))]);
            }
        } catch (e) {
            console.error(e);
        }
    },

    receiveMessagesJson: function (sessionId, json) {
        if (!!this.ooui_ReceiveWebAssemblySessionMessageJson_method) {
            MonoRuntime.call_method (this.ooui_ReceiveWebAssemblySessionMessageJson_method, null, [MonoRuntime.mono_string (sessionId), MonoRuntime.mono_string (json)]);
        }
    },

    findMethods: function () {
        this.main_module = MonoRuntime.assembly_load (Module.entryPoint.a);
        if (!this.main_module)
            throw "Could not find Main Module " + Module.entryPoint.a + ".dll";

        this.main_class = MonoRuntime.find_class (this.main_module, Module.entryPoint.n, Module.entryPoint.t)
        if (!this.main_class)
            throw "Could not find Program class in main module";

        this.main_method = MonoRuntime.find_method (this.main_class, Module.entryPoint.m, -1)
        if (!this.main_method)
            throw "Could not find Main method";

        this.ooui_module = MonoRuntime.assembly_load ("Ooui");
        if (!!this.ooui_module) {

            this.ooui_class = MonoRuntime.find_class (this.ooui_module, "Ooui", "UI");
            if (!this.ooui_class)
                throw "Could not find UI class in Ooui module";

            this.ooui_DisableServer_method = MonoRuntime.find_method (this.ooui_class, "DisableServer", -1);
            if (!this.ooui_DisableServer_method)
                throw "Could not find DisableServer method";

            this.ooui_StartWebAssemblySession_method = MonoRuntime.find_method (this.ooui_class, "StartWebAssemblySession", -1);
            if (!this.ooui_StartWebAssemblySession_method)
                throw "Could not find StartWebAssemblySession method";

            this.ooui_ReceiveWebAssemblySessionMessageJson_method = MonoRuntime.find_method (this.ooui_class, "ReceiveWebAssemblySessionMessageJson", -1);
            if (!this.ooui_ReceiveWebAssemblySessionMessageJson_method)
                throw "Could not find ReceiveWebAssemblySessionMessageJson method";
        }
    },
};

