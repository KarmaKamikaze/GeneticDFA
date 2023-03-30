// brigde states
State AA = State(Foo);
State AW = State(Foo);
State WA = State(Foo);
State AB = State(Foo);
State WW = State(Foo);
State BA = State(Foo);
State WB = State(Foo);
State BW = State(Foo);
State BB = State(Foo);

void RunBrigdeDFA(char input[]) {
  FSM DFA = FSM(AA);
  bool stringComplete = false;
  DFA.update();
  delay(5000);
  for(int i = 0; i < strlen(input); i+=2) {
    if(input[i] == 'A'){
      if(input[i+1] == 'w'){
        if(DFA.isInState(AA)){
          DFA.transitionTo(AW);
        }
        else if(DFA.isInState(WA)){
          DFA.transitionTo(WW);
        }
        else if(DFA.isInState(BA)){
          DFA.transitionTo(BW);
        }
      }
      else if(input[i+1] == 'e'){
        if(DFA.isInState(AA)){
          DFA.transitionTo(WA);
        }
        else if(DFA.isInState(AW)){
          DFA.transitionTo(WW);
        }
        else if(DFA.isInState(AB)){
          DFA.transitionTo(WB);
        }
      }
      else{Serial.println("Failed because false state"); break;}
    }
    else if(input[i] == 'E'){
      if(input[i+1] == 'w'){
        if(DFA.isInState(AW)){
          DFA.transitionTo(AB);
        }
        else if(DFA.isInState(WW)){
          DFA.transitionTo(WB);
        }
        else if(DFA.isInState(BW)){
          DFA.transitionTo(BB);
        }
      }
      else if(input[i+1] == 'e'){
        if(DFA.isInState(WA)){
          DFA.transitionTo(BA);
        }
        else if(DFA.isInState(WW)){
          DFA.transitionTo(BW);
        }
        else if(DFA.isInState(WB)){
          DFA.transitionTo(BB);
        }
      }
      else{Serial.println("Failed because false state"); break;}
    }
    else if(input[i] == 'L'){
      if(input[i+1] == 'w'){
        if(DFA.isInState(WB)){
          DFA.transitionTo(WA);            
        }
        else if(DFA.isInState(AB)){
          DFA.transitionTo(AA);
        }
      }
      else if(input[i+1] == 'e'){
        if(DFA.isInState(BW)){
          DFA.transitionTo(AW);
        }
        else if(DFA.isInState(BA)){
          DFA.transitionTo(AA);            
        }
      }
      else{Serial.println("Failed because false state"); break;}
    }

    // if char of input is not 1 | 0 we break and check if we are in final state.
    else{
      stringComplete = false;
      DFA.transitionTo(AA);
      DFA.update();
      break;
      //Trash State when input is not 1 or 0
    }

    if(i == ((strlen(input) - 2))) {
      stringComplete = true;
    }

    DFA.update();
  }

  //if we finish in a final state report back that we have passed
  if(stringComplete && !DFA.isInState(BB)){
    Serial.println("Success \n");
    DFA.transitionTo(AA);
    DFA.update();     
    //SendTracePassed();
  }
  else{
    Serial.println("Failure \n");
    DFA.transitionTo(AA);
    DFA.update();
    //SendTraceFailed();
  }
}