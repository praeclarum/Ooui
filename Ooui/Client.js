// Ooui v1.0.0

var debug = false;

const nodes = {};
const hasText = {};

let socket = null;

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

// Try to close the socket gracefully
window.onbeforeunload = function() {
    if (socket != null) {
        socket.close (1001, "Unloading page");
        socket = null;
        console.log ("Web socket closed");
    }
    return null;
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
        console.log ("Web socket opened");
    });

    socket.addEventListener ("error", function (event) {
        console.error ("Web socket error", event);
    });

    socket.addEventListener ("close", function (event) {
        console.error ("Web socket close", event);
    });

    socket.addEventListener("message", function (event) {
        const messages = JSON.parse (event.data);
        if (debug) console.log("Messages", messages);
        if (Array.isArray (messages)) {
            const jqs = []
            messages.forEach (function (m) {
                // console.log('Raw value from server', m.v);
                m.v = fixupValue (m.v);
                if (m.k.startsWith ("$.")) {
                    jqs.push (m);
                }
                else {
                    processMessage (m);
                }
            });
            // Run jQuery functions last since they usually require a fully built DOM
            jqs.forEach (processMessage);
        }
    });

    console.log("Web socket created");

    // Throttled window resize event
    (function() {
        window.addEventListener("resize", resizeThrottler, false);

        var resizeTimeout;
        function resizeThrottler() {
            if (!resizeTimeout) {
                resizeTimeout = setTimeout(function() {
                    resizeTimeout = null;
                    resizeHandler();            
                }, 100);
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
            if (socket != null)
                socket.send (ems);
            if (debug) console.log ("Event", em);
        }    
    }());
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

function msgCall (m) {
    const id = m.id;
    const node = getNode (id);
    if (!node) {
        console.error ("Unknown node id", m);
        return;
    }
    const isJQuery = m.k.startsWith ("$.");
    const target = isJQuery ? $(node) : node;
    if (m.k === "insertBefore" && m.v[0].nodeType == Node.TEXT_NODE && m.v[1] == null && hasText[id]) {
        // Text is already set so it clear it first
        if (target.firstChild)
            target.removeChild (target.firstChild);
        delete hasText[id];
    }
    const f = isJQuery ? target[m.k.slice(2)] : target[m.k];
    if (debug) console.log ("Call", node, f, m.v);
    const r = f.apply (target, m.v);
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
        if (m.k === "change" || m.k === "input") {
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
        if (socket != null)
            socket.send (ems);
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
    return v;
}
