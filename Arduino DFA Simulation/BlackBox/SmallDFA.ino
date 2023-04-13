// States for Small DFA
State A = State(InStateAction);
State B = State(InStateAction);
State C = State(InStateAction);
//mashine
FSM SmallDFA = FSM(A);

void RunSmallDFA(char input[]) {
  SmallDFA.update();
    if (input == "1") {
      if (SmallDFA.isInState(A)) {
        SmallDFA.transitionTo(B);
      }
      if (SmallDFA.isInState(B)) {
        SmallDFA.transitionTo(C);
      }
      if (SmallDFA.isInState(C)) {
        SmallDFA.transitionTo(A);
      }
    } 
    else if (input == "0") {
      if (SmallDFA.isInState(A)) {
        SmallDFA.transitionTo(A);
      }
      if (SmallDFA.isInState(B)) {
        SmallDFA.transitionTo(A);
      }
      if (SmallDFA.isInState(C)) {
        SmallDFA.transitionTo(A);
      }
    }
    else if(input == "END"){
      if (DFA.isInState(C)) {
      uint8_t reply[15] = "Trace Accepted!";
      TransmitVerdict(reply);
      }
      else {
      uint8_t reply[13] = "Trace Failed!";
      TransmitVerdict(reply);
      }
    //reset
    SmallDFA.transitionTo(A);
    };
  DFA.update();
}
