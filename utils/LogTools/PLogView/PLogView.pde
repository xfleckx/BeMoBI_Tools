import controlP5.*;

import hypermedia.net.*;
import java.util.*;
import java.util.regex.Pattern;

int PORT_RX=7502;
String HOST_IP = "localhost";//IP Address of the PC in which this App is running
UDP udp;//Create UDP object for recieving

ControlP5 cp5;

LogHistory history;
int x_margin = 5;
int y_margin = 30;

int fontSize = 18;

int entryMargin = 3;
int initialOffset = 5;

void setup() {
  cp5 = new ControlP5(this);
  
    cp5.addTextfield("portToListen")
     .setPosition(4,4)
     .setSize(50,19)
     .setColor(color(255,0,0))
     .setText(str(PORT_RX));
     
  cp5.addButton("Listen")
   .setPosition(54,4)
   .setSize(50,19);
   
  cp5.addButton("Clear")
     .setPosition(154,4)
     .setSize(50,19);
     
  size(1024, 350);
  
  int logHistoryCount = (350 - (2 * y_margin) - entryMargin - initialOffset) / fontSize;
  
  history = new LogHistory(logHistoryCount);
}

void draw() {
  
  background(0);

  renderLogHistory();
  
  
}

void renderLogHistory(){
  int y_offset = 0;
  pushStyle();
  pushMatrix();
  translate(x_margin, y_margin);
  ListIterator<LogEntry> iterator = history.listIterator();
  initialOffset = 10;
  
  while(iterator.hasPrevious() && y_offset < height) {
     LogEntry current = iterator.previous();
     current.render(new PVector(0, y_offset), width - x_margin, 50);
     y_offset += fontSize + entryMargin + initialOffset;
     initialOffset = 0;
  }
  popMatrix();
  popStyle();
}


void receive(byte[] data, String HOST_IP, int PORT_RX) {
  String message = new String(data);
  LogEntry value = GetFrom( message );
  history.add(value);
}

public LogEntry GetFrom(String s) {
  LogEntry e = new LogEntry();
  e.content = s;
    
  if (match(s, "Info") != null) {
    e.textColor = color( 113, 224, 138 );
  }
  
  if (match(s, "Error") != null) {
    e.textColor = color( 250, 43, 43 );
  }
  
  if (match(s, "Fatal") != null) {
    e.textColor = color( 250, 43, 43 );
  }
  
  if (match(s, "Debug") != null) {
    e.textColor = color( 245, 175, 44 );
  }
  
  if (match(s, "Warn") != null) {
    e.textColor = color( 245, 175, 44 );
  }
  return e;
}


public void Listen(){
  
  if(udp != null)
    udp.close();
  
  PORT_RX = int(cp5.get(Textfield.class,"portToListen").getText());
  
  udp= new UDP(this, PORT_RX, HOST_IP);
  udp.log(false);
  udp.listen(true);
  
}

public void Clear() {
  history.clear();
}