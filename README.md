# dotnet-message-broker
A simple message broker application (.NET Core)

## Using the message broker server application
Coming soon...

## Using the client library

Initialize and start client:
```
using MessageBrokerClient;

string hostname = "myserver";
string clientID = "exampleClient";
string serverKey = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
string clientKey = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";

MessageClient client = new MessageClient();

if (!client.Start(hostname, clientID, serverKey, clientKey))
    throw new Exception("connection failed.");
```

Send message
```
Message message = new Message();
message.Receiver = "otherClient";
message.Content = Encoding.UTF8.GetBytes("Hello World!");

client.SendMessage(message, (Message response) =>
{
    Console.WriteLine(Encoding.UTF8.GetString(response.Content));
});
```

Receive messages (via callback)
```
client.MessageReceived += (Message message) =>
{
    Console.WriteLine(message.Sender);

    Message response = message.CreateResponse();
    response.Content = Encoding.UTF8.GetBytes($"Hello {message.Sender}!");

    client.SendMessage(response);
};
```
