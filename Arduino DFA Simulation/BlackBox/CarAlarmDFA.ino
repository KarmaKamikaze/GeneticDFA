// States for Car Alarm
State UO = State(InStateAction);
State LO = State(InStateAction);
State UC = State(InStateAction);
State LC = State(InStateAction);
State AC = State(InStateAction);
State AO = State(InStateAction);
FSM CarAlarmDFA = FSM(UO);

void RunCarAlarmDFA(char input[]) {
  CarAlarmDFA.update();
    if (input == "l") {
      if (CarAlarmDFA.isInState(UO)) {
        CarAlarmDFA.transitionTo(LO);
      } 
      else if (CarAlarmDFA.isInState(UC)) {
        CarAlarmDFA.transitionTo(LC);
      } 
      else {
        Serial.println("Failed because it enters the trash state");
      }
    } 
    else if (input == "u") {
      if (CarAlarmDFA.isInState(LO)) {
        CarAlarmDFA.transitionTo(UO);
      } 
      else if (CarAlarmDFA.isInState(LC)) {
        CarAlarmDFA.transitionTo(UC);
      } 
      else if (CarAlarmDFA.isInState(AC)) {
        CarAlarmDFA.transitionTo(UC);
      } 
      else {
        Serial.println("Failed because it enters the trash state");
      }
    } 
    else if (input == "c") {
      if (CarAlarmDFA.isInState(UO)) {
        CarAlarmDFA.transitionTo(UC);
      } 
      else if (CarAlarmDFA.isInState(LO)) {
        CarAlarmDFA.transitionTo(LC);
      } 
      else {
        Serial.println("Failed because it enters the trash state");
      }
    } 
    else if (input == "o") {
      if (CarAlarmDFA.isInState(UC)) {
        CarAlarmDFA.transitionTo(UO);
      } 
      else if (CarAlarmDFA.isInState(LC)) {
        CarAlarmDFA.transitionTo(LO);
      } 
      else if (CarAlarmDFA.isInState(AC)) {
        CarAlarmDFA.transitionTo(AO);
      } 
      else {
        Serial.println("Failed because it enters the trash state");
      }
    } 
    else if (input == "a") {
      if (CarAlarmDFA.isInState(LC)) {
        DFCarAlarmDFAA.transitionTo(AC);
      } 
      else {
        Serial.println("Failed because it enters the trash state");
      }
    }
    if input == "END"{
      if (!CarAlarmDFA.isInState(AO)) {
      uint8_t reply[15] = "Trace Accepted!";
      } 
      else {
        uint8_t reply[13] = "Trace Failed!";
      }
    //send verdict
    TransmitVerdict(reply);
    //reset
    DFA.transitionTo(UO);
    }
  DFA.update();
}
