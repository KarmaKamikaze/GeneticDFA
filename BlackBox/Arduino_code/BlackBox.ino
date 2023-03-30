#include <FiniteStateMachine.h>
#include <RHCRC.h>
#include <RHDatagram.h>
#include <RHGenericDriver.h>
#include <RHGenericSPI.h>
#include <RHHardwareSPI.h>
#include <RHMesh.h>
#include <RHNRFSPIDriver.h>
#include <RHReliableDatagram.h>
#include <RHRouter.h>
#include <RHSPIDriver.h>
#include <RHSoftwareSPI.h>
#include <RHTcpProtocol.h>
#include <RH_ASK.h>
#include <RH_NRF24.h>
#include <RH_NRF905.h>
#include <RH_RF22.h>
#include <RH_RF24.h>
#include <RH_RF69.h>
#include <RH_RF95.h>
#include <RH_Serial.h>
#include <RH_TCP.h>
#include <RadioHead.h>
#include <radio_config_Si4460.h>

//Radio stuff
#define RF69_FREQ 868 // Frequency
#define RFM69_INT 2 // DIO0 Pin
#define RFM69_CS 10 // Select Signal Pin
#define RFM69_RST 3 // RST Pin
#define LED 9 // Test LED
#define SERIAL_BAUD 115200

// Singleton instance of the radio driver
RH_RF69 rf69(RFM69_CS, RFM69_INT);
int packetnum = 0;

void RunSmallDFA(char input[]);
void RunCarAlarmDFA(char input[]);
void RunBrigdeDFA(char input[]);
char input[20];


void setup() {
  //Setup for RFM69 chipset
  Serial.begin(SERIAL_BAUD);
  while (!Serial) { delay(1); } // wait until serial console is open

  pinMode(RFM69_RST, OUTPUT);
  Serial.println("RFM69HW Transmission Test!");
  Serial.println();

  // manual reset
  digitalWrite(RFM69_RST, LOW);
  digitalWrite(RFM69_RST, HIGH);
  delay(10);
  digitalWrite(RFM69_RST, LOW);
  delay(10);

  if (!rf69.init()) {
    Serial.println("RFM69HW radio init failed");
    while (1);
  }
  Serial.println("RFM69HW radio init OK!");
  if (!rf69.setFrequency(RF69_FREQ)) {
    Serial.println("setFrequency failed");
  }
  else {Serial.println("Listening at 868 MHz.");}
  // RFM69HW *requires* that the Tx power flag is set!
  rf69.setTxPower(20); // range from 14-20 for power  
}

void loop() {
  // WE NEED A WHILE HERE THAT WAITS FOR MESSAGDE
  while(!rf69.available()){
    Serial.print("Waiting for signal");
    delay(2000);
  }
  
  //If signal is recheived
  if (rf69.available()) {
    Serial.print("The messagde is: ");
    // put messagde here
    char input[] = "01011"; //MAKE THIS THE INPUT OF RADIO PLZ
  }

  //Call correct simulation
  // Comment out the onces not in use
  RunSmallDFA(input);
  RunCarAlarmDFA(input);
  RunBrigdeDFA(input);
}


void SendTracePassed(){
  char radiopacket[20] = "Trace Passed";
  itoa(packetnum++, radiopacket+12, 10);
  Serial.print("Sending "); Serial.println(radiopacket);

  rf69.send((uint8_t *)radiopacket, strlen(radiopacket));
  rf69.waitPacketSent();
}

void SendTraceFailed(){
  char radiopacket[20] = "Trace Failed";
  itoa(packetnum++, radiopacket+12, 10);
  Serial.print("Sending "); Serial.println(radiopacket);

  rf69.send((uint8_t *)radiopacket, strlen(radiopacket));
  rf69.waitPacketSent();
}

void Foo(){
}