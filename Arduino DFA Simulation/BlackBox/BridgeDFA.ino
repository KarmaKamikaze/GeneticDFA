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
FSM BridgeDFA = FSM(AA);

void RunBridgeDFA(char input) {
  bool stringComplete = false;
  BridgeDFA.update();

  //Let it be noted that: A = ArriveEast, B = EnterEast, C = LeaveEast, X = ArriveWest, Y = EnterWest and Z = LeaveWest
  switch(input)
  {
    case 'A':
      if (BridgeDFA.isInState(AA)){
        BridgeDFA.transitionTo(WA);
      } else if (BridgeDFA.isInState(AW)){
        BridgeDFA.transitionTo(WW);
      } else if (BridgeDFA.isInState(AB)){
        BridgeDFA.transitionTo(WB);
      } else{
        BridgeDFA.transitionTo(TRASH);
      }
      break;

      case 'B':
        if (BridgeDFA.isInState(WA)){
        BridgeDFA.transitionTo(BA);
      } else if (BridgeDFA.isInState(WW)){
        BridgeDFA.transitionTo(BW);
      } else if (BridgeDFA.isInState(WB)){
        BridgeDFA.transitionTo(BB);
      } else{
        BridgeDFA.transitionTo(TRASH);
      }
      break;

      case 'C':
        if (BridgeDFA.isInState(BW)){
        BridgeDFA.transitionTo(AW);
      } else if (BridgeDFA.isInState(BA)){
        BridgeDFA.transitionTo(AA);
      } else{
        BridgeDFA.transitionTo(TRASH);
      }

      case 'X':
        if (BridgeDFA.isInState(AA)){
        BridgeDFA.transitionTo(AW);
      } else if (BridgeDFA.isInState(WA)){
        BridgeDFA.transitionTo(WW);
      } else if (BridgeDFA.isInState(BA)){
        BridgeDFA.transitionTo(BW);
      } else{
        BridgeDFA.transitionTo(TRASH);
      }

      case 'Y':
        if (BridgeDFA.isInState(AW)){
        BridgeDFA.transitionTo(AB);
      } else if (BridgeDFA.isInState(WW)){
        BridgeDFA.transitionTo(WB);
      } else if (BridgeDFA.isInState(BW)){
        BridgeDFA.transitionTo(BB);
      } else{
        BridgeDFA.transitionTo(TRASH);
      }
      
      case 'Z':
        if (BridgeDFA.isInState(WB)){
        BridgeDFA.transitionTo(WA);
      } else if (BridgeDFA.isInState(AB)){
        BridgeDFA.transitionTo(AA);
      } else{
        BridgeDFA.transitionTo(TRASH);
      }

      case '$':
        if (!BridgeDFA.isInState(BB) && !BridgeDFA.isInState(TRASH)) {
        uint8_t reply[15] = "Trace Accepted!";
        TransmitVerdict(reply);
        BridgeDFA.transitionTo(AA);
        BridgeDFA.update();
        }
        else {
        uint8_t reply[13] = "Trace Failed!";
        TransmitVerdict(reply);
        BridgeDFA.transitionTo(AA);
        BridgeDFA.update();
        }

      default:
      BridgeDFA.transitionTo(TRASH);
  }
}