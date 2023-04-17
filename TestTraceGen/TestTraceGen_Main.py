import rstr
from TestTraceGen_SmallDFA import generate_test_traces_for_smalldfa
from TestTraceGen_CarAlarmDFA import generate_test_traces_for_caralarmDFA
def test_trace_generation():
    print("Which dfa do you wish to generate for? \n type 1, 2 or 3 to select \n 1. Small DFA \n 2. Car Alarm DFA \n 3. Brigde DFA")
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

def test():
    import exrex
    import re
    regex = 'c(l(u|au))*(l(a|o|u|au)|l|o)|(l|c(l(u|au))*lo)(c(o|(u|au)(l(u|au))*lo))*(c(a|(u|au)(l(u|au))*(l(a|o|u|au)|o|l)|u|au|o)|c|u)|(c(l(u|au))*o|(l|c(l(u|au))*lo)(c(o|(u|au)(l(u|au))*lo))*(u|c(u|au)(l(u|au))*o))(c(l(u|au))*o|(l|c(l(u|au))*lo)(c(o|(u|au)(l(u|au))*lo))*(u|c(u|au)(l(u|au))*o))*(c(l(u|au))*(l(a|o|u|au)|l|o)|(l|c(l(u|au))*lo)(c(o|(u|au)(l(u|au))*lo))*(c(a|(u|au)(l(u|au))*(l(a|o|u|au)|o|l)|u|au|o)|c|u)|c|l)|c|l'
    for i in range(25):
        trace = exrex.getone(regex, limit=2)
        if re.match(regex,trace):
            print("passed: ",trace)
#test()