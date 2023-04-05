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

// Singleton instance of the radio driver
RH_RF69 rf69(RFM69_CS, RFM69_INT);

void setup() {
  // Setup for RFM69 chipset
  Serial.begin(SERIAL_BAUD);
  // Uncomment the following line, if used without serial connection
  while (!Serial) { delay(1); } // Wait until serial console is open

  pinMode(RFM69_RST, OUTPUT);
  Serial.println("RFM69HW Arduino DFA Blackbox System!");
  Serial.println();

  // Manual reset
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
  
  while(!HandShake())
    HandShake();
  }

  /*SEND AN ENTIRE TRACE OVER RADIO*/
  /*WAIT FOR ACK THAT TRACE WAS RECHIEVED*/
  /*IF ACK SAYS ACK: SUCESS WE SERIAL PRINT TRACE: SUCESS, ELSE TRACE:FAILED*/
  /*LOOP TO NEXT TRACE TO TEST*/
  /*WHEN NO MORE TRACES SERIAL PRINT "STOP" TO STOP PYTHON*/


void HandShake(){

  //Setup for sending ACK Request
  char radiopacket[20] = "ACK?";
  Serial.print("Sending handshake"); Serial.println(radiopacket);
  rf69.send((uint8_t *)radiopacket, strlen(radiopacket));
  rf69.waitPacketSent();

  //While we dont get ACK we delay
  while(!rf69.available()){

    Serial.print("Waiting for handshake ack");
    delay(1000);

  }

  //If ACK recieved we return true
  if (rf69.available() && /*INSERT MESSAGE HERE*/ == "ACK!") {
    Serial.print("ACK recieved");
  }

  return true;
}
