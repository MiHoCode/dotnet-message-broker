
#include <WiFi.h>

char ssid[] = "myNetwork";
char pass[] = "myPassword";

int status = WL_IDLE_STATUS;
char nodeAddress[] = "myMessagingNode";
int nodePort = 10243;
char deviceID[] = "myDeviceID";

WiFiClient client;

void setup() {

  // connect wifi
  while (status != WL_CONNECTED) {
    status = WiFi.begin(ssid, pass);
    delay(10000);
  }

  // connect to node
  if (client.connect(nodeAddress, nodePort)) {
    client.println(deviceID);
  }
}

// blocking method for reading a line from the client
String readLine(){
  String line = "";
  char c = 'x';
  while(c != '\n'){
    while (!client.available())
      delay(10);
    c = client.read();
    if(c != '\n')
      line += c;
  }
  return line;
}

void loop() {

  // send message
  client.println("message"); // message command
  client.println("myRecipientID"); // recipient id
  client.println("Hello World! Time: " + millis()); // the message

  // receive incoming messages
  int messageCount = readLine().toInt(); // count of incoming messages
  for(int i=0;i<messageCount;i++){
    String sender = readLine();
    String message = readLine();

    // do something with the message
  }
  
}
