// Bridge states
State AA = State(InStateAction);
State AW = State(InStateAction);
State WA = State(InStateAction);
State AB = State(InStateAction);
State WW = State(InStateAction);
State BA = State(InStateAction);
State WB = State(InStateAction);
State BW = State(InStateAction);
State BB = State(InStateAction);

void RunBridgeDFA(char input[]) {
  FSM DFA = FSM(AA);
  bool stringComplete = false;
  DFA.update();

  for (int i = 0; i < strlen(input); i++) {
    if (input[i] == 'A'){
      if (DFA.isInState(AA)){
        DFA.transitionTo(WA);
      } else if (DFA.isInState(AW)){
        DFA.transitionTo(WW);
      } else if (DFA.isInState(AB)){
        DFA.transitionTo(WB);
      } else{
        Serial.println("Failed because it enters the trash state");
      }
    } else if (input[i] == 'B'){
      if (DFA.isInState(WA)){
        DFA.transitionTo(BA);
      } else if (DFA.isInState(WW)){
        DFA.transitionTo(BW);
      } else if (DFA.isInState(WB)){
        DFA.transitionTo(BB);
      } else{
        Serial.println("Failed because it enters the trash state");
      }
    } else if (input[i] == 'C'){
      if (DFA.isInState(BW)){
        DFA.transitionTo(AW);
      } else if (DFA.isInState(BA)){
        DFA.transitionTo(AA);
      } else{
        Serial.println("Failed because it enters the trash state");
      }
    } else if (input[i] == 'X'){
      if (DFA.isInState(AA)){
        DFA.transitionTo(AW);
      } else if (DFA.isInState(WA)){
        DFA.transitionTo(WW);
      } else if (DFA.isInState(BA)){
        DFA.transitionTo(BW);
      } else{
        Serial.println("Failed because it enters the trash state");
      }
    } else if (input[i] == 'Y'){
      if (DFA.isInState(AW)){
        DFA.transitionTo(AB);
      } else if (DFA.isInState(WW)){
        DFA.transitionTo(WB);
      } else if (DFA.isInState(BW)){
        DFA.transitionTo(BB);
      } else{
        Serial.println("Failed because it enters the trash state");
      }
    } else if (input[i] == 'Z'){
      if (DFA.isInState(WB)){
        DFA.transitionTo(WA);
      } else if (DFA.isInState(AB)){
        DFA.transitionTo(AA);
      } else{
        Serial.println("Failed because it enters the trash state");
      }
    }

    // If char of input is not acceptable we break and check if we are in
    // accepting state.
    else {
      stringComplete = false;
      DFA.transitionTo(AA);
      DFA.update();
      break;
    }

    if (i == ((strlen(input) - 1))) {
      stringComplete = true;
    }
    DFA.update();
  }

  // If we finish in a final state report back that we have passed
  if (stringComplete && !DFA.isInState(BB)) {
    uint8_t reply[15] = "Trace Accepted!";
  } else {
    uint8_t reply[13] = "Trace Failed!";
  }
  TransmitVerdict(reply);

  DFA.transitionTo(AA);
  DFA.update();
}