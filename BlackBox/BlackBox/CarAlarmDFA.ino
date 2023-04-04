// States for Car Alarm
State UO = State(Foo);
State LO = State(Foo);
State UC = State(Foo);
State LC = State(Foo);
State AC = State(Foo);
State AO = State(Foo);


void RunCarAlarmDFA(char input[]) {
  bool stringComplete = false;
  FSM DFA = FSM(UO);
  DFA.update();
  Serial.begin(SERIAL_BAUD);
  while (!Serial) { delay(1); }
    for(int i = 0; i < strlen(input); i++) {
      if(input[i] == 'l'){
        if(DFA.isInState(UO)){
          DFA.transitionTo(LO);
        }
        else if(DFA.isInState(UC)){
          DFA.transitionTo(LC);
        }
        else{Serial.println("Failed because it enters the trash state"); break;}
      }
      else if(input[i] == 'u'){
        if(DFA.isInState(LO)){
          DFA.transitionTo(UO);
          }
        else if(DFA.isInState(LC)){
          DFA.transitionTo(UC);
        }
        else if(DFA.isInState(AC)){
          DFA.transitionTo(UC);
        }
        else{Serial.println("Failed because it enters the trash state"); break;}
      }
      else if(input[i] == 'c'){
        if(DFA.isInState(UO)){
          DFA.transitionTo(UC);
        }
        else if(DFA.isInState(LO)){
          DFA.transitionTo(LC);
        }
        else{Serial.println("Failed because it enters the trash state"); break;}
      }
      else if(input[i] == 'o'){
        if(DFA.isInState(UC)){
          DFA.transitionTo(UO);
        }
        else if(DFA.isInState(LC)){
          DFA.transitionTo(LO);
        }
        else if(DFA.isInState(AC)){
          DFA.transitionTo(AO);
        }
        else{Serial.println("Failed because it enters the trash state"); break;}
      }
      else if(input[i] == 'a'){
        if(DFA.isInState(LC)){
          DFA.transitionTo(AC);
        }
        else{Serial.println("Failed because it enters the trash state"); break;}
      }
      // If char of input is not acceptable we break and check if we are in final state.
      else{        
        stringComplete = false;
        break;
      }
      // if we are at end of input
      if(i == strlen(input) - 1){
        stringComplete = true;
      }

      DFA.update();
    }

    //If we finish in a final state report back that we have passed
    if(stringComplete && !DFA.isInState(AO)){
      Serial.println("Success \n");
      SendTracePassed();
      DFA.transitionTo(UO);
      DFA.update();     
    }
    else{
      Serial.println("Failure \n");
      SendTraceFailed();
      DFA.transitionTo(UO);
      DFA.update();
    }
  }
