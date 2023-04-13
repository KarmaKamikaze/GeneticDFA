// States for Small DFA
State A = State(InStateAction);
State B = State(InStateAction);
State C = State(InStateAction);
State TRASH = State(InStateAction);
// Machine
FSM SmallDFA = FSM(A);

void RunSmallDFA(char input) {
  SmallDFA.update();

  switch (input)
  {
    case 0:
      if (SmallDFA.isInState(A)) {
        SmallDFA.transitionTo(A);
      }
      if (SmallDFA.isInState(B)) {
        SmallDFA.transitionTo(A);
      }
      if (SmallDFA.isInState(C)) {
        SmallDFA.transitionTo(A);
      }

    case 1:
      if (SmallDFA.isInState(A)) {
        SmallDFA.transitionTo(B);
      }
      if (SmallDFA.isInState(B)) {
        SmallDFA.transitionTo(C);
      }
      if (SmallDFA.isInState(C)) {
        SmallDFA.transitionTo(A);
      }

    default:
      SmallDFA.transitionTo(TRASH);
      uint8_t reply[13] = "Trace Failed!";
      TransmitVerdict(reply);
      break;
  }

    if (SmallDFA.isInState(C)) {
      uint8_t reply[15] = "Trace Accepted!";
      TransmitVerdict(reply);
    }

    // Reset
    SmallDFA.transitionTo(A);
    SmallDFA.update();
}
