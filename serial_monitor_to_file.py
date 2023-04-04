#imports.
import serial.tools.list_ports
from datetime import datetime

#variables.
ports: list = serial.tools.list_ports.comports()
serial_instance = serial.Serial()
active_port: str = ""
received_messages: list = []

#print a list of active ports to user.
for port in ports:
    print(port)

#user selects a port.
value = f'COM{input("Select Port: COM")}'

#searches the list of ports matching the one selected by the user.
for i in range(0, len(ports)):
    if str(ports[i]).startswith(value):
        active_port = value
        print(ports[i])

#must be the same baudrate as arduino
serial_instance.baudrate = 9600
serial_instance.port = active_port
serial_instance.open()

file_name = datetime.now()

#create a new file for the received serial data
file = open(file_name+".txt", "w")

while 1:
    #message is read from serial
    arduino_data = serial_instance.readline()
    
    #add read message to list
    received_messages.append(arduino_data.decode().rstrip())
    
    #if we read STOP we streaming all data to file.
    if 'STOP' in str(arduino_data.decode()):
        for message in received_messages:
            file.write(message)
            
        print(f"A STOP token has been received. And all messages have been written to file: {file_name}")
        input("Press any key to close.")
        break
    
#close file and serial stream when arduino is no longer connected.
serial_instance.close()
file.close()
