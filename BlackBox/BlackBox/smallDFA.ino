//States for Small DFA
State A = State(Foo);
State B = State(Foo);
State C = State(Foo);


void RunSmallDFA(char input[]){
  bool stringComplete = false;
  FSM DFA = FSM(A);
  DFA.update();
  Serial.begin(SERIAL_BAUD);
  while (!Serial) { delay(1); }

    for(int i = 0; i < strlen(input); i++) {
      if(input[i] == '1'){
        if(DFA.isInState(A)){
          DFA.transitionTo(B);
        }
        if(DFA.isInState(B)){
          DFA.transitionTo(C);
        }
        if(DFA.isInState(C)){
          DFA.transitionTo(A);
        }
      }
      else if(input[i] == '0'){
        if(DFA.isInState(A)){
          DFA.transitionTo(A);
          }
        if(DFA.isInState(B)){
          DFA.transitionTo(A);
        }
        if(DFA.isInState(C)){
          DFA.transitionTo(A);
        }
      }
      // If char of input is not 1 | 0 we break and check if we are in final state.
      else{        
        stringComplete = false;
        Serial.println("Failed because it enters the trash state");
        break;
      };
      //If we are at end of string
      if(i == strlen(input) - 1){
        stringComplete = true;
      };
      // Moves state in dfa
      DFA.update();
    }

    //if we finish in a final state report back that we have passed
    if(stringComplete && DFA.isInState(C)){
      SendTracePassed();
    }
    else{
      SendTraceFailed();
    }

}
