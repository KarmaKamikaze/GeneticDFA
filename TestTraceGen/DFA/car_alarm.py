import random as rnd
import re
import exrex

regex = 'c(l(u|au))*(l(a|o|u|au)|l|o)|(l|c(l(u|au))*lo)(c(o|(u|au)(l(u|au))*lo))*(c(a|(u|au)(l(u|au))*(l(' \
        'a|o|u|au)|o|l)|u|au|o)|c|u)|(c(l(u|au))*o|(l|c(l(u|au))*lo)(c(o|(u|au)(l(u|au))*lo))*(u|c(u|au)(l(' \
        'u|au))*o))(c(l(u|au))*o|(l|c(l(u|au))*lo)(c(o|(u|au)(l(u|au))*lo))*(u|c(u|au)(l(u|au))*o))*(c(l(u|au))*(l(' \
        'a|o|u|au)|l|o)|(l|c(l(u|au))*lo)(c(o|(u|au)(l(u|au))*lo))*(c(a|(u|au)(l(u|au))*(l(' \
        'a|o|u|au)|o|l)|u|au|o)|c|u)|c|l)|c|l'


def generate_test_traces_for_caralarmDFA(number_of_failing_traces: int,
                                         number_of_passing_traces: int, length: int):
    """ Returns traces """
    return generate_failing_traces(number_of_failing_traces, length), generate_passing_traces(number_of_passing_traces)


def generate_failing_traces(number_of_failing_traces: int, length: int):
    """ Generates list of failing traces """
    failing_traces: list = []
    i: int = 0
    # Generate input amount of traces
    while len(failing_traces) != number_of_failing_traces:
        i = i + 1
        trace = ''.join(rnd.choices(["l", "c", "a", "u", "o"], k = rnd.randint(1, length)))
        if not re.match(regex, trace):
            if trace not in failing_traces:
                failing_traces.append(trace)

    print(f"It took {i} attempts to create {number_of_failing_traces} failing traces.")
    return failing_traces


def generate_passing_traces(number_of_passing_traces: int):
    """ Generates list of passing traces """
    passing_traces: list = []
    i: int = 0
    # Generate input amount of traces
    while len(passing_traces) != number_of_passing_traces:
        i = i + 1
        trace = exrex.getone(regex, limit=2)
        if re.match(regex, trace):
            if trace not in passing_traces:
                if len(trace) < 20:
                    passing_traces.append(trace)

    print(f"It took {i} attempts to create {number_of_passing_traces} passing traces.")
    return passing_traces
