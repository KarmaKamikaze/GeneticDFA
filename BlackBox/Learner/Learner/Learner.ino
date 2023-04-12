#include <FiniteStateMachine.h>
#include <SPI.h>
#include <RH_RF69.h>

// Radio stuff
#define RF69_FREQ 868       // Frequency
#define RFM69_INT 2         // DIO0 Pin
#define RFM69_CS 10         // Select Signal Pin
#define RFM69_RST 3         // RST Pin
#define LED 9               // Test LED
#define SERIAL_BAUD 9600

const char* array_of_traces[3] = {"100100110", "011010", "101010"};
// Singleton instance of the radio driver
RH_RF69 rf69(RFM69_CS, RFM69_INT);

String radiopacket;

bool Handshake();

void setup() {
  // Setup for RFM69 chipset
  Serial.begin(SERIAL_BAUD);
  // Uncomment the following line, if used without serial connection
  while (!Serial) { delay(1); } // Wait until serial console is open

  pinMode(RFM69_RST, OUTPUT);

  // Manual reset
  digitalWrite(RFM69_RST, LOW);
  digitalWrite(RFM69_RST, HIGH);
  delay(10);
  digitalWrite(RFM69_RST, LOW);
  delay(10);

  if (!rf69.init()) {
    //Radio init failed
    while (1);
  }
  //init was sucessfull
  if (!rf69.setFrequency(RF69_FREQ)) {
    //Frequency failed
  }
  else {Serial.println("Listening at 868 MHz.");}
  // RFM69HW *requires* that the Tx power flag is set!
  rf69.setTxPower(20); // range from 14-20 for power 
}

void loop() {
  
  while(!HandShake()){
  //We wait for handshake to be true
  }
  /*SEND AN ENTIRE TRACE OVER RADIO*/
  for (int i=0; i<sizeof(array_of_traces)/sizeof(array_of_traces[0]); i++){

    rf69.send((uint8_t *)array_of_traces[i], strlen(array_of_traces[i]));
    rf69.waitPacketSent();

    /*WAIT FOR ACK THAT TRACE WAS RECHIEVED*/
    while(!rf69.available()){
    delay(10);
    }

    /*Print answer from blackbox*/
    if (rf69.available() && rf69.BeskedFraBlackBox == "Trace Accepted!") {
      Serial.print(array_of_traces[i] +":SUCESS");
    }
    else if(rf69.available() && rf69.BeskedFraBlackBox == "Trace Failed!"){
      Serial.print(array_of_traces[i] +":FAILED");
    }
  }
  /*WHEN NO MORE TRACES SERIAL PRINT "STOP" TO STOP PYTHON*/
  Serial.print("STOP");
  }
  


//This one is to be changed with Karma handshake
bool HandShake(){

  //Setup for sending ACK Request
  char radiopacket[20] = "ACK?";
  Serial.print("Sending handshake"); Serial.println(radiopacket);
  rf69.send((uint8_t *)radiopacket, strlen(radiopacket));
  rf69.waitPacketSent();

  //While we dont get ACK we delay
  while(!rf69.available()){
    delay(10);
  }

  //If ACK recieved we return true
  if (rf69.available() && rf69.BeskedFraBlackBox == "ACK!") {
  }

  return true;
}
