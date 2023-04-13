// States for Car Alarm
State UO = State(InStateAction);
State LO = State(InStateAction);
State UC = State(InStateAction);
State LC = State(InStateAction);
State AC = State(InStateAction);
State AO = State(InStateAction);
FSM CarAlarmDFA = FSM(UO);

void RunCarAlarmDFA(char input) {
  CarAlarmDFA.update();

  switch (input)
  {
    case 'l':
      if (CarAlarmDFA.isInState(UO)) {
        CarAlarmDFA.transitionTo(LO);
      } 
      else if (CarAlarmDFA.isInState(UC)) {
        CarAlarmDFA.transitionTo(LC);
      } 
      else {
        CarAlarmDFA.transitionTo(TRASH);
      }

    case 'u':
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
        CarAlarmDFA.transitionTo(TRASH);
      }

    case 'c':
      if (CarAlarmDFA.isInState(UO)) {
        CarAlarmDFA.transitionTo(UC);
      } 
      else if (CarAlarmDFA.isInState(LO)) {
        CarAlarmDFA.transitionTo(LC);
      } 
      else {
        CarAlarmDFA.transitionTo(TRASH);
      }

    case 'o':
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
        CarAlarmDFA.transitionTo(TRASH);
      }

    case 'a':
      if (CarAlarmDFA.isInState(LC)) {
        CarAlarmDFA.transitionTo(AC);
      } 
      else {
        CarAlarmDFA.transitionTo(TRASH);
      }

    case 'q':
      if (!CarAlarmDFA.isInState(AO) && !CarAlarmDFA.isInState(TRASH)) {
      uint8_t reply[15] = "Trace Accepted!";
      TransmitVerdict(reply);
      CarAlarmDFA.transitionTo(UO);
      CarAlarmDFA.update();
      } else {
      uint8_t reply[13] = "Trace Failed!";
      TransmitVerdict(reply);
      CarAlarmDFA.transitionTo(UO);
      CarAlarmDFA.update();
      }
  }
}
