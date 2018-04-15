# Ooui Development History

> How did I get it all to work?

The first step is to have a few guiding principles. For Ooui, there were two principles: simplicity + exposing the full expressiveness of the DOM. I wanted to keep the API simple. I wanted the implementation to be simple. I wanted the libraries to be simple. But I wanted to do all of this while still exposing all of the DOM.

Ooui had a difficult task ahead of it - keeping two UI models in sync: the server and the client browser. I’ve written a few sync systems in the past and never found them satisfying. I wanted a general solution to keeping two object trees in sync in the face of mutation (by both sides). The use case of keeping a DOM object tree in sync wound end up being just a special case of this general ability.

To meet this goal, I devised a message passing system between the client and server. This works well for broadcasts changes to the tree, but doesn’t help with the initialization problem. My biggest and best inside was that I could keep a history of every message and object has received and that I could re-broadcast those messages to a new client at will (and thus get them in sync). All of this is handled by the Ooui.EventTarget class which everything inherits from. So every Div in your code is an EventTarget that keeps a history of every mutation ever applied to it.

So now I had a simple and powerful way to keep two object trees in sync. In order to get it “working” I had to create a transport for these messages. I chose WebSockets as the initial transport - but it could be anything. Now I just needed a web server to serve sockets (I implemented two: one built-in standalone server and one that works with ASP.NET) and some client code to re-create and manipulate the tree in the browser (Client.js).

And now we can finally get to wasm. The only trick needed to get it to work as a wasm was to replace the transport. In wasm you still have a split between the wasm memory model and the JS memory model and execution environment. This looks a lot like the server client split that Ooui already handles very well (it even serializes all data so the memory barrier is no issue at all). So I added a new transport to Ooui called WebAssemblySession that serializes messages and moves them across the memory barrier. Even though the app has a direct line to the client, it still acts like a server - recording old messages - waiting for new clients to connect… :-)

So that’s it. Ooui already could bridge servers and clients. Wasm was just another server client scenario, so getting it to work was rather simple.

Now the trick was understanding packaging. This is where all my bug are :-) I just read and read the wasm example and kept playing with it until I understood all its dependencies and how it was working. This meant reading parts of and understand mono.js. I’m lucky here in that I’ve implemented CLRs before and while I don’t know mono exactly, I know how it should work. But really it was just playing with the example over and over.

The last step was to take my understanding of the wasm build system and to turn that into a build task for Ooui to protect others from having to learn all this. That became the Ooui.Wasm package.

