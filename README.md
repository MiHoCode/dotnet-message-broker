# IoT Data Messaging System
A simple but secure data messaging application using encryption.
![alt text](scheme.png?raw=true)

## Applications / Components
* Message Broker Server
* Message Broker Client
* Messaging Node
* Messaging Client

### Message Broker Server
The Message Broker Server is a .NET Core console application that acts as an intermediary service. This application should be installed on a server that is accessible via the Internet, if clients from different networks should be able to communicate with each other.
This broker could be a Linux Server.

[learn more...](https://github.com/MiHoCode/dotnet-message-broker/wiki/Message-Broker-Server)

### Message Broker Client
The Message Broker Client is a .NET Core class library that can be integrated into your own applications. Messages or data packets can be sent to or received from other clients in encrypted form via this interface. The classes contained in the source code can certainly be ported to other frameworks (e.g. Mono).

[More info...](https://github.com/MiHoCode/dotnet-message-broker/wiki/Message-Broker-Client-(Library))

### Messaging Node
The Messaging Node is a .NET Core console application that implements the Message Broker Client and acts as an intermediate node between the Broker Server and child devices within the network. Thus, it behaves as a server to the devices. The communication between the child devices and the messaging node is unencrypted.
For example, this node could run on a Raspberry PI within the local network.

[More info...](https://github.com/MiHoCode/dotnet-message-broker/wiki/Messaging-Node)

### Messaging Client
The Messaging Client is an example scetch for Arduino. It can be used in Arduinos with Ethernet/WiFi Shield or NodeMCU devices. The code serves as a template for implementation in other applications.
