# dotnet-message-broker
A simple message broker application (.NET Core)

## Using the message broker server application
1. Build the application for your target platfrom i.e. linux64 (standalone!)
2. Copy the application to your server
3. Start the application

It will automatically create a folder called 'keys' in the application directory.
There you will find a file called 'broker.key', wich is the 'serverKey' for the client.
To add client keys you will have to run the application with parameters, i.e.:
```
> MessageBrokerServer addclient myExampleClientID
```
This will create a new key file named after the client id.

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
