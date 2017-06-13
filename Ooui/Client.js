
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
    const tagName = m.v;
    const node = tagName === "text" ?
        document.createTextNode ("") :
        document.createElement (tagName);
    if (tagName !== "text")
        node.id = id;
    nodes[id] = node;
    console.log ("Created Node", node);
}

function msgSet (m) {
    const id = m.id;
    const node = getNode (id);
    if (!node) {
        console.error ("Unknown Node Id", m);
        return;
    }
    node[m.k] = m.v;
    console.log ("Set Property", node, m.k, m.v);
}

function msgCall (m) {
    const id = m.id;
    const node = getNode (id);
    // \u2999
    if (!node) {
        console.error ("Unknown Node Id", m);
        return;
    }
    const f = node[m.k];
    console.log ("Call", node, f, m.v);
    f.apply (node, m.v);
}

function processMessage (m) {
    switch (m.m) {
        case "Create":
            msgCreate (m);
            break;
        case "Set":
            msgSet (m);
            break;
        case "Call":
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
        if ((v.length === 9) && (v[0] === "\u2999")) {
            return getNode (v.substr(1));
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
