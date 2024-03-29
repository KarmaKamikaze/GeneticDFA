// States for Small DFA
State A = State(InStateAction);
State B = State(InStateAction);
State C = State(InStateAction);
// Machine
FSM SmallDFA = FSM(A);

void RunSmallDFA(char input) {
  SmallDFA.update();
  switch (input)
  {
    case '0':
      if (SmallDFA.isInState(A)) {
        SmallDFA.transitionTo(A);
      }
      if (SmallDFA.isInState(B)) {
        SmallDFA.transitionTo(A);
      }
      if (SmallDFA.isInState(C)) {
        SmallDFA.transitionTo(A);
      }
      break;

    case '1':
      if (SmallDFA.isInState(A)) {
        SmallDFA.transitionTo(B);
      }
      if (SmallDFA.isInState(B)) {
        SmallDFA.transitionTo(C);
      }
      if (SmallDFA.isInState(C)) {
        SmallDFA.transitionTo(A);
      }
      break;

    case '$':
      if (SmallDFA.isInState(C)) {
      uint8_t reply[] = "A"; // Accept
      TransmitVerdict(reply);
    } else {
      SmallDFA.transitionTo(TRASH);
      uint8_t reply[] = "F"; // Failed
      TransmitVerdict(reply);
    }
      
      SmallDFA.transitionTo(A);
      SmallDFA.update();
    break;

    default:
      SmallDFA.transitionTo(TRASH);
      break;
    
  }
}
