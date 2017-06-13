
// Create WebSocket connection.
const socket = new WebSocket ("ws://localhost:8080" + rootElementPath, "ooui-1.0");

console.log("Socket created");

// Connection opened
socket.addEventListener('open', function (event) {
    console.log("Socket opened");
    socket.send('Hello Server!');
});

// Listen for messages
socket.addEventListener('message', function (event) {
    console.log('Message from server', event.data);
});
