# qRPC

qRPC stands for Quick Remote Procedure Call.  It's a project I've made, aimed at .NET, to counter the tedium required for simple RPC projects when using gRPC.  qRPC works by serializing a message object into JSON and sending it over TCP, where it is deserialized.  To facilitate calls to look like the implementation is within the same application, I've used Castle DynamicProxy - a big thank you to them!


## How Does it Work?

The first step is to crate an interface, which will be shared between the client and server.

```c#
public interface IMyService{
  string RemoteConcat(string a, string b);
}
```

> Note that all method parameters must be JSON serializable.  Thus all class parameters must have parameterless constructors.  
> If you pass in an object, the remote version will not be the same object.  It will consist of the class properties re-serialized.

The server creates an implementation of the interface and registers it as an RPC server, listening on a given IP and port.

```c#
public MyService : IMyService{
  public string RemoteConcat(string a, string b) => a + b;
}

MyService server = new MyService();
QrpcServer<IMyService>(server, 5000, Encoding.UTF8);

```

The client registers the destination IP and port and calls the methods easily.

```c#
var client = QrpcClient<IMyService>(5000, "127.0.0.1", Encoding.UTF8);

var joinedString = client.RemoteConcat("Hello", " World");
Console.WriteLine(joinedString); //Gives us "Hello World"
```

## Encryption

As for version 6.3.0, symmetric encryption is provided by AES.

To enable encryption, `SetEncryptionKey(string key)` must be called on both the client and server
providing the same `key`.

Note that this is not really safe for scenarios where the preshared key is in the public domain, or a
public client.  It is supposed to stop snooping from server-to-server.  A more public-domain friendly 
solution may be added later.

## Coming (hopefully) soon:

 - ServiceCollection registration