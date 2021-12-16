# Message Boker Server + Client (.NET Core)
A simple but secure message broker application using encryption.

## Using the server application
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

### Groups
When running the first time, the application will create a file called 'groups.cfg' in the application directory.
This will look like this:

```
{
  "Groups": [
    {
      "Name": "admins",
      "Members": [
        "admin"
      ]
    }
  ]
}
```

You can edit this file to manage groups or use the admin client to execute some commands.
For more information execute the application with the parameter 'help'.

```
> MessageBrokerServer help
```
All commands can also be sent through the broker client with id 'admin'.
Receiver should be 'broker' oder 'server' then.
Put your command like 'addgroup examplegroup' into the message content (UTF8).

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

Send message to group
```
Message message = new Message();
message.Receiver = "group:examplegroup";
message.Content = Encoding.UTF8.GetBytes("Hello Group!");

client.SendMessage(message);
```
