
var debug = true;

const nodes = {};

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

function ooui (rootElementPath) {
    var opened = false;

    socket = new WebSocket ("ws://" + document.location.host + rootElementPath, "ooui");

    socket.addEventListener ("open", function (event) {
        console.log ("Web socket opened");
        opened = true;
    });

    socket.addEventListener ("error", function (event) {
        console.error ("Web socket error", event);
    });

    socket.addEventListener ("close", function (event) {
        console.error ("Web socket close", event);
        if (opened) {
            location.reload ();
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

function msgCreate (m) {
    const id = m.id;
    const tagName = m.k;
    const node = tagName === "#text" ?
        document.createTextNode ("") :
        document.createElement (tagName);
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

function msgCall (m) {
    const id = m.id;
    const node = getNode (id);
    if (!node) {
        console.error ("Unknown node id", m);
        return;
    }
    const f = node[m.k];
    if (debug) console.log ("Call", node, f, m.v);
    const r = f.apply (node, m.v);
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
