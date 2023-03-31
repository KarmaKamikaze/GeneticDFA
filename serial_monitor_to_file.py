#imports
import serial.tools.list_ports

#list out all ports in use
ports = serial.tools.list_ports.comports()
serial_instance = serial.Serial()
port_list = []
port_variable = ""


for port in ports:
    #append each port to the list of ports
    port_list.append(port)
    #print out all ports
    print(port)

value = input("select Port: COM")
#searches the list of ports matching the one picked
for i in range(0, len(port_list)):
    if str(port_list[i]).startswith("COM" +value):
        port_variable = "COM" + value
        print(port_list[i])

#must be the same baudrate as arduino
serial_instance.baudrate = 9600
serial_instance.port = port_variable
serial_instance.open()

#Create a new file to steam serial data to.
file = open("test.txt", "w")

while 1:
    arduino_data = serial_instance.readline()

    #Save line to txt file
    file.write(arduino_data.decode())

    #if we read STOP we stop streaming data to file
    if 'STOP' in str(arduino_data.decode()):
        break
    
    #print line
    print(arduino_data)

#close file when arduino no longer connected
file.close()
