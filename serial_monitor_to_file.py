#imports
import serial.tools.list_ports
from datetime import datetime

#list out all ports in use
ports = serial.tools.list_ports.comports()
serial_instance = serial.Serial()
port_list = []
active_port= ""

for port in ports:
    #append each port to the list of ports
    port_list.append(port)
    #print out all ports
    print(port)


value = f'COM{input("Select Port: COM")}'
#searches the list of ports matching the one picked
for i in range(0, len(port_list)):
    if str(port_list[i]).startswith(value):
        active_port = value
        print(port_list[i])

#must be the same baudrate as arduino
serial_instance.baudrate = 9600
serial_instance.port = port_variable
serial_instance.open()

#Creat a new file to steam serial data to.
file = open(datetime.now()+".txt", "w")

while 1:
    arduino_data = serial_instance.readline()
    #if we read STOP we stop streaming data to file
    if 'STOP' in str(arduino_data.decode()):
        print("A stop have been read: The program will now close and save the file under name: ")
        k = input("press enter to close")
        break
    
    #save line to file, with decoded from byte to string.
    print(arduino_data.decode().rstrip())
    file.write(arduino_data.decode().rstrip())
    
#close file when arduino no longer connected
file.close()
