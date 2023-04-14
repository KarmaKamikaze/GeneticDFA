from datetime import datetime
import random as rnd
import rstr
import re

def generate_test_traces_for_caralarmDFA():
    ''' writes traces generated to a file '''
    
    print("how many passing traces should be generated?")
    number_of_passing_traces: int = input()
    print("how many failing traces should be generated?")
    number_of_failing_traces: int = input()
    print("how many char long should it be?")
    length: int = input()
    
    #generate and write traces
    write_traces_to_file(generate_failing_traces(number_of_failing_traces,length),generate_passing_traces(number_of_passing_traces,length))
    
    print("script will now close, press enter to close")
    input()
    
def write_traces_to_file(failing_traces:list,passing_traces:list):
    ''' writes traces generated to a file '''
    #generate filename
    time = datetime.now().strftime("%Y-%m-%d %H-%M-%S")
    file_name: str = "CarAlarm-traces made: "+time
    
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
    pattern = re.compile("^0*1(00*1)*1((1|0)0*1(00*1)*1)*$") #MUST BE THE NEW REGEX
    i: int =0
    #generate input amount of traces
    while len(failing_traces) != int(number_of_failing_traces):
        i = i+1
        random_num = ''.join(rnd.choices(["UO","LO","UC","LC","AC","AO","T"],k=int(length))) 
        if not re.match(pattern,random_num):
            failing_traces.append(random_num)
    print(f"it took {i} attempts at creating {number_of_failing_traces} failing")
    return failing_traces

def generate_passing_traces(number_of_passing_traces:int, length:int):
    ''' generates list of passing traces '''
    passing_traces: list = []
    pattern = re.compile("^0*1(00*1)*1((1|0)0*1(00*1)*1)*$") #MUST BE THE NEW REGEX
    i: int =0
    #generate input amount of traces
    while len(passing_traces) != int(number_of_passing_traces):
        i = i+1
        random_num = ''.join(rnd.choices(["UO","LO","UC","LC","AC","AO","T"],k=int(length)))
        if re.match(pattern,random_num):
            passing_traces.append(random_num) 
    print(f"it took {i} attempts at creating {number_of_passing_traces} passing")
    return passing_traces

#Run the script
generate_test_traces_for_caralarmDFA()


