# imports
import json
import time
import serial.tools.list_ports
from datetime import datetime

# variables
baud_rate = 9600
ports: list = serial.tools.list_ports.comports()
serial_instance = serial.Serial()
active_port: str = ""
received_messages: list = []


def save_to_file(traces: list) -> None:
    file_name = datetime.now().strftime("%d-%m-%Y-%H-%M-%S")
    traces_dict = {
        'PASSED': list(),
        'FAILED': list()
    }

    for message in traces:
        parts = message.split(":")
        trace, verdict = parts[0], parts[1].upper()

        if verdict == 'PASSED':
            traces_dict['PASSED'].append(trace)
        elif verdict == 'FAILED':
            traces_dict['FAILED'].append(trace)

        print(message)

    # create a new file for the received serial data
    with open(f'{file_name}.json', "w") as file:
        json.dump(traces_dict, file, ensure_ascii=False, indent=4)


# print a list of active ports to user
for port in ports:
    print(port)

# user selects a port
value = f'COM{input("Select Port: COM")}'

# searches the list of ports matching the one selected by the user
for i in range(0, len(ports)):
    if str(ports[i]).startswith(value):
        active_port = value
        print(ports[i])

# must be the same baud rate as arduino
serial_instance.baudrate = baud_rate
serial_instance.port = active_port
serial_instance.open()

while 1:
    # message is read from serial
    arduino_data = serial_instance.readline()
    # add read message to list
    if 'STOP' not in str(arduino_data):
        received_messages.append(arduino_data.decode().strip())

    # if we read STOP, we stream all data to file
    if 'STOP' in str(arduino_data):
        save_to_file(received_messages)

        print(f'A STOP token has been received. All messages have been written to file.')
        input("Press any key to close.")
        break

# close serial stream when arduino is no longer connected
serial_instance.close()
5