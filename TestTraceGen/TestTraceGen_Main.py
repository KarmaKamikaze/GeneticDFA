from TestTraceGen_SmallDFA import generate_test_traces_for_smalldfa
from TestTraceGen_CarAlarmDFA import generate_test_traces_for_caralarmDFA
from TestTraceGen_BrigdeDFA import generate_test_traces_for_brigdeDFA
def test_trace_generation():
    print("Wellcome to epic Trace generation 9000, which for a smeckel and a dime, can generate all the traces u want")
    print("Which dfa do you wish to generate for? \n type 1, 2 or 3 to select \n 1. Small DFA \n 2. Car Alarm DFA \n 3. Brigde DFA")
    user_input: int = input()
    if int(user_input) == 1:
        generate_test_traces_for_smalldfa()
    elif int(user_input) == 2:
        generate_test_traces_for_caralarmDFA()
    elif int(user_input) == 3:
        generate_test_traces_for_brigdeDFA()
    else: 
        print("Wrong input")
        test_trace_generation()
    
test_trace_generation()
