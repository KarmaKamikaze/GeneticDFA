from datetime import datetime
from DFA.small import generate_test_traces_for_smalldfa
from DFA.car_alarm import generate_test_traces_for_caralarmDFA
from DFA.brigde import generate_test_traces_for_brigdeDFA

def write_traces_to_file(failing_traces:list,passing_traces:list,user_input: str):
    ''' writes traces generated to a file '''
    #generate filename
    time = datetime.now().strftime("%Y-%m-%d %H-%M-%S")
    if user_input == "1":
        file_name: str = "small-dfa-traces-"+time
    elif user_input == "2":
        file_name: str = "car-alarm-dfa-traces-"+time
    elif user_input == "3":
        file_name: str = "brigde-dfa-traces-"+time
    
    #write passing traces
    text_to_write: str = "passing = ["
    for trace in passing_traces:
        text_to_write +=(trace+",")
    text_to_write +=(trace+"]\n")
    
    #write failing traces
    text_to_write += "failing = ["
    for trace in failing_traces:
        text_to_write += (trace+",")
    text_to_write +=(trace+"]")

    #write to file
    f = open(f"{file_name}.txt", "a")
    f.write(text_to_write)
    f.close()
    print(f"The traces generated have been written to file:{file_name}")

def test_trace_generation():
    print("Which dfa do you wish to generate for? \n type 1, 2 or 3 to select \n 1. Small DFA \n 2. Car Alarm DFA \n 3. Brigde DFA")
    user_input: int = input()
    print("how many passing traces should be generated?")
    number_of_passing_traces: int = input()
    print("how many failing traces should be generated?")
    number_of_failing_traces: int = input()
    print("how many char long should it be?")
    length_of_trace: int = input()
    
    if user_input == "1":
        traces = generate_test_traces_for_smalldfa(number_of_failing_traces,number_of_passing_traces,length_of_trace)
    elif user_input == "2":
        traces =generate_test_traces_for_caralarmDFA(number_of_failing_traces,number_of_passing_traces,length_of_trace)
    elif user_input == "3":
        traces = generate_test_traces_for_brigdeDFA(number_of_failing_traces,number_of_passing_traces,length_of_trace)
    else: 
        print("Wrong input")
        test_trace_generation()
    write_traces_to_file(traces[0],traces[1],user_input)
    print("script will now close, press enter to close")
    input()

#run test trace generation
test_trace_generation()