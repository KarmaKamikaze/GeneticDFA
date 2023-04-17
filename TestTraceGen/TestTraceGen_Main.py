from TestTraceGen_SmallDFA import generate_test_traces_for_smalldfa
from TestTraceGen_CarAlarmDFA import generate_test_traces_for_caralarmDFA
def test_trace_generation():
    print("Which dfa do you wish to generate for?")
    print("type 1, 2 or 3 to select")
    print("1. Small DFA")
    print("2. Car Alarm DFA")
    print("3. Brigde DFA")
    user_input: int = input()
    if int(user_input) == 1:
        generate_test_traces_for_smalldfa()
    elif int(user_input) == 2:
        generate_test_traces_for_caralarmDFA()
    elif int(user_input) == 3:
        print("Not implemented yet")
    else: 
        print("Wrong input")
        test_trace_generation()
    
test_trace_generation()