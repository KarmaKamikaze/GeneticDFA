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

for i in range(0, len(port_list)):
    if str(port_list[i]).startswith("COM" +value):
        port_variable = "COM" + value
        print(port_list[i])

#MUST BE THE SAME AS ARDUINO
serial_instance.baudrate = 9600
serial_instance.port = port_variable
serial_instance.open()


while 1:
    arduino_data = serial_instance.readline()
    print(arduino_data)
