# GeneticDFA

The GeneticDFA project uses genetic programming to reverse engineer blackbox systems modeled in DFA form.

To simulate a
natural blackbox system, an Arduino, has been set up to act as an unknown system, similarly modeled as a DFA. Another
Arduino communicates with the blackbox system through radio frequency, generating test traces by firing sequences of
words at the blackbox. By the end of a sequence, the blackbox will notify the learning Arduino of the traces' passing
grade.

Once enough test traces have been accumulated; they can be used to learn the DFA using the GeneticDFA program.
Test traces can additionally be generated through the provided Python script in case you have no Arduino with RF
communication capabilities (it is also a LOT quicker).

<!-- GETTING STARTED -->
## Getting started

To get a local copy up and running, follow these steps.

<!-- DEPENDENCIES -->
### Dependencies

The project is dependant on [GeneticSharp](https://github.com/giacomelli/GeneticSharp), 
[AvaloniaUI](https://github.com/AvaloniaUI/Avalonia), and 
the [Graphviz.NetWrapper](https://github.com/Rubjerg/Graphviz.NetWrapper).

### Installation

The project can either be run using the CLI or a sparse GUI, which simplifies browsing the DFA visualizations after each 
generation.

1. Clone the repository
```sh
git clone https://github.com/KarmaKamikaze/GeneticDFA.git
```

2. Navigate to the CLI or the UI project folder and build the project

CLI version:
```sh
cd GeneticDFA/GeneticDFA && dotnet build --configuration Release
```

GUI version:
```sh
cd GeneticDFA/GeneticDFAUI && dotnet build --configuration Release
```

3. Navigate to the TestTraceGen folder and run the python script to generate test traces
```sh
cd ../TestTraceGen/ && py -3.10 ./main.py
```
Once the script has been initiated, it will prompt you to select a specific test model for which it will generate 
traces.
You can also choose the amount of passing vs. failing traces it should generate. The failing traces require a limit
to how long they can become, which the script will also prompt you for.

The script will produce a `json` file named, e.g., `small-dfa-traces-[date-and-time].json`.

4. Move the test traces to the build folder and rename them

CLI version:
```sh
mv small-dfa-traces-[date-and-time].json ../GeneticDFA/bin/Release/net6.0/traces.json
```

GUI version:
```sh
mv small-dfa-traces-[date-and-time].json ../GeneticDFAUI/bin/Release/net6.0/traces.json
```

5. Navigate to the build folder and run the program

CLI version:
```sh
cd ../GeneticDFA/bin/Release/net6.0/ && ./GeneticDFA
```

GUI version:
```sh
cd ../GeneticDFAUI/bin/Release/net6.0/ && ./GeneticDFAUI
```

## Using the program

### Tweak the settings for the genetic algorithm and run the GA

The GUI makes tweaking the settings ease while also preserving previously used settings between runs and executions.
To tweak the settings using the CLI, you must manually change them in the source code and then rebuild the project, as 
described in point 2.

![Picture of the settings GUI](https://github.com/KarmaKamikaze/GeneticDFA/blob/master/.github/images/GeneticDFAUI-1.png)

### Browse the DFA visualizations

![Picture of the DFA visualization GUI](https://github.com/KarmaKamikaze/GeneticDFA/blob/master/.github/images/GeneticDFAUI-2.png)

<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE` for more information.

<!-- CONTACT --> 
## Contact

Project Link: [https://github.com/KarmaKamikaze/GeneticDFA](https://github.com/KarmaKamikaze/GeneticDFA)
