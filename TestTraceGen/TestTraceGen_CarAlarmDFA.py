from datetime import datetime
import random as rnd
import re
import exrex

regex = 'c(l(u|au))*(l(a|o|u|au)|l|o)|(l|c(l(u|au))*lo)(c(o|(u|au)(l(u|au))*lo))*(c(a|(u|au)(l(u|au))*(l(a|o|u|au)|o|l)|u|au|o)|c|u)|(c(l(u|au))*o|(l|c(l(u|au))*lo)(c(o|(u|au)(l(u|au))*lo))*(u|c(u|au)(l(u|au))*o))(c(l(u|au))*o|(l|c(l(u|au))*lo)(c(o|(u|au)(l(u|au))*lo))*(u|c(u|au)(l(u|au))*o))*(c(l(u|au))*(l(a|o|u|au)|l|o)|(l|c(l(u|au))*lo)(c(o|(u|au)(l(u|au))*lo))*(c(a|(u|au)(l(u|au))*(l(a|o|u|au)|o|l)|u|au|o)|c|u)|c|l)|c|l'

def generate_test_traces_for_caralarmDFA():
    ''' writes traces generated to a file '''
    
    print("how many passing traces should be generated?")
    number_of_passing_traces: int = input()
    print("how many failing traces should be generated?")
    number_of_failing_traces: int = input()
    print("how many char long should it be?")
    length: int = input()
    
    #generate and write traces
    write_traces_to_file(generate_failing_traces(number_of_failing_traces,length),generate_passing_traces(number_of_passing_traces))
    
    print("script will now close, press enter to close")
    input()
    
def write_traces_to_file(failing_traces:list,passing_traces:list):
    ''' writes traces generated to a file '''
    #generate filename
    time = datetime.now().strftime("%Y-%m-%d %H-%M-%S")
    file_name: str = "CarAlarm-traces-made"+time
    
    #write passing traces
    text_to_write: str = "Passing: \n"
    for trace in passing_traces:
        text_to_write +=(trace+"\n")

    #write failing traces
    text_to_write += "Failing: \n"
    for trace in failing_traces:
        text_to_write += (trace+"\n")

    #write to file
    f = open(f"{file_name}.txt", "a")
    f.write(text_to_write)
    f.close()
    print(f"The traces generated have been written to file:{file_name}")

def generate_failing_traces(number_of_failing_traces:int,length:int):
    ''' generates list of failing traces '''
    failing_traces: list = []
    i: int = 0
    #generate input amount of traces
    while len(failing_traces) != int(number_of_failing_traces):
        i = i+1
        trace = ''.join(rnd.choices(["l","c","a","u","o"],k=int(length))) 
        if not re.match(regex,trace):
            if trace not in failing_traces:
                failing_traces.append(trace)
                
    print(f"it took {i} attempts at creating {number_of_failing_traces} failing")
    return failing_traces

def generate_passing_traces(number_of_passing_traces:int):
    ''' generates list of passing traces '''
    passing_traces: list = []
    i: int = 0
    #generate input amount of traces
    while len(passing_traces) != int(number_of_passing_traces):
        i= i+1
        trace = exrex.getone(regex, limit=2)
        if re.match(regex,trace):
            if trace not in passing_traces:
                passing_traces.append(trace)
                
    print(f"it took {i} attempts at creating {number_of_passing_traces} passing")
    return passing_traces


