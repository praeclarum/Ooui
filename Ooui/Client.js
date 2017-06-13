
// Create WebSocket connection.
const socket = new WebSocket ("ws://localhost:8080" + rootElementPath, "ooui-1.0");

console.log("WebSocket created");

const nodes = {}

function getNode (id) {
    switch (id) {
        case "window": return window;
        case "document": return document;
        case "document.body": return document.body;
        default: return nodes[id];
    }
}

function msgCreate (m) {
    const id = m.id;
    const tagName = m.k;
    const node = tagName === "text" ?
        document.createTextNode ("") :
        document.createElement (tagName);
    if (tagName !== "text")
        node.id = id;
    nodes[id] = node;
    console.log ("Created node", node);
}

function msgSet (m) {
    const id = m.id;
    const node = getNode (id);
    if (!node) {
        console.error ("Unknown node id", m);
        return;
    }
    node[m.k] = m.v;
    console.log ("Set property", node, m.k, m.v);
}

function msgCall (m) {
    const id = m.id;
    const node = getNode (id);
    if (!node) {
        console.error ("Unknown node id", m);
        return;
    }
    const f = node[m.k];
    console.log ("Call", node, f, m.v);
    f.apply (node, m.v);
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
        if ((v.length >= 2) && (v[0] === "\u2999") && (v[1] === "n")) {
            // console.log("V", v);
            const id = v.substr(1);
            // console.log("ID", id);
            return getNode (id);
        }
    }
    return v;
}

// Connection opened
socket.addEventListener('open', function (event) {
    console.log("WebSocket opened");
    socket.send('Hello Server!');
});

// Listen for messages
socket.addEventListener('message', function (event) {
    const message = JSON.parse (event.data);
    message.v = fixupValue (message.v);
    console.log('Message from server', message);
    processMessage (message);
});
