#imports
import serial.tools.list_ports
from datetime import datetime

#list out all ports in use
ports: list = serial.tools.list_ports.comports()
serial_instance = serial.Serial()
active_port: str = ""
received_messages: str = ""
messages_read: int = 0


for port in ports:
    #print out all ports
    print(port)


value = f'COM{input("Select Port: COM")}'
#searches the list of ports matching the one picked
for i in range(0, len(ports)):
    if str(ports[i]).startswith(value):
        active_port = value
        print(ports[i])

#must be the same baudrate as arduino
serial_instance.baudrate = 9600
serial_instance.port = active_port
serial_instance.open()

#Create a new file for the received serial data.
file = open(datetime.now()+".txt", "w")

while 1:
    arduino_data = serial_instance.readline()
    #add read message to list
    received_messages += str(arduino_data.decode().rstrip()+"\n")
    #increase number of messages stored
    messages_read +=1
    
    #if we read STOP we stop streaming data to file
    if 'STOP' in str(arduino_data.decode()):
        file.write(list_of_recieved_messagdes)
        print("A STOP token has been received. The program will now close and save the file.")
        input("Press any key to close.")
        break
    
    #if we have more than 9 messagdes appended by newline, we write them to file. And clears
    if messages_read > 9:
        file.write(received_messages)
        received_messages = ""
        messages_read = 0
    
#close file when arduino is no longer connected
serial_instance.close()
file.close()
