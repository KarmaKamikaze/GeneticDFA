from datetime import datetime
from DFA.small import generate_test_traces_for_smalldfa
from DFA.car_alarm import generate_test_traces_for_caralarmDFA
from DFA.brigde import generate_test_traces_for_brigdeDFA

def get_input():
    while True:
        try:
            user_input = int(input("> "))
            return user_input
        except ValueError:
            print("Error in input. Please try again.")

def write_traces_to_file(failing_traces:list,passing_traces:list,user_input: str):
    ''' Writes traces generated to a file '''
    # Generate filename
    time = datetime.now().strftime("%Y-%m-%d %H-%M-%S")
    if user_input == 1:
        file_name: str = "small-dfa-traces-"+time
    elif user_input == 2:
        file_name: str = "car-alarm-dfa-traces-"+time
    elif user_input == 3:
        file_name: str = "brigde-dfa-traces-"+time
    
    # Write passing traces
    text_to_write: str = "passing = ["
    for trace in passing_traces:
        text_to_write +=(trace+",")
    text_to_write +=(trace+"]\n")
    
    # Write failing traces
    text_to_write += "failing = ["
    for trace in failing_traces:
        text_to_write += (trace+",")
    text_to_write +=(trace+"]")

    # Write to file
    f = open(f"{file_name}.txt", "a")
    f.write(text_to_write)
    f.close()
    print(f"The generated traces have been written to file: {file_name}")

def test_trace_generation():
    print("Which DFA do you wish to generate traces for? \nType 1, 2, or 3 to select. \n1. Small DFA.\n2. Car Alarm DFA.\n3. Brigde DFA.")
    user_input = get_input()
    print("How many passing traces should be generated?")
    number_of_passing_traces: int = input()
    print("How many failing traces should be generated?")
    number_of_failing_traces: int = input()
    print("How many char long should it be?")
    length_of_trace: int = input()
    
    if user_input == 1:
        traces = generate_test_traces_for_smalldfa(number_of_failing_traces,
    number_of_passing_traces, length_of_trace)
    elif user_input == 2:
        traces =generate_test_traces_for_caralarmDFA(number_of_failing_traces,
    number_of_passing_traces, length_of_trace)
    elif user_input == 3:
        traces = generate_test_traces_for_brigdeDFA(number_of_failing_traces,
    number_of_passing_traces, length_of_trace)
    else: 
        print("Wrong input\n")
        test_trace_generation()
    write_traces_to_file(traces[0],traces[1],user_input)
    print("Press enter to close the script.")
    input()

#run test trace generation
test_trace_generation()
