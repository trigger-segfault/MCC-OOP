# WJLCS-1

* **Author:** Robert Jordan
* **Language:** C# (.NET Framework 4.6.2)
* **Requires:** Visual Studio 2017

## Enigma Machine Interfaces

The Enigma Machine project has been outlined into a set of interfaces needed to implement the inner workings of the machine.

### `interface IWheel`

This interface defines the physical wheel used for scrambling letters. This is not the final object that is used for the actual wheels, simulated in the machine. It only contains immutable information on which letters go to which other letters. `IWheel` contains the property `string Label` which is used to differentiate it from other wheels. The `Encipher(...)` method is used by `ICentralWheel` to get the output at the specified position (rotation).

### `interface ICentralWheel`

This interface defines a wheel that is in-use within the current Enigma Machine setup. This class keeps track of `Wheel` (the `IWheel` type to use), `Position` (rotation), and `TurnoverPosition` (what position to turn the next `ICentralWheel` at). The `Encipher(...)` method implements `IWheel`'s `Encipher(...)` without the need of passing the `position` parameter. It will also return `true` when the turnover was passed and the next `ICentralWheel` should be turned.

### `interface ICentralWheelCollection`

This interface defines the functionality for managing input into the group of `ICentralWheel`'s as well as modifying their `Position`. The `Setup(...)` method must be called to pass the ordered collection of `WheelSetup`'s that define what `IWheel`'s to use as well as `InitialPosition` and `TurnoverPosition`. The `Encipher(...)` method runs the input through each stored `ICentralWheel` and outputs the results. It will automatically handle the position (rotation) changes of the contained `ICentralWheel`'s. The `Peek(...)` method returns the same results as `Encipher(...)`, except the wheel positions are not modified.

### `class WheelSetup` (Immutable)

This class contains information on the setup for a single `ICentralWheel`. This class is passed as an `IEnumerable` to the `ICentralWheelCollection` when performing initial setup, or resetting the machine's state.

### `interface ISteckerboard`

This interface defines the steckerboard wiring of each letter and what letter it ends up corresponding to. The `Setup(...)` method must be called once to pass the mapping of input to output characters to the object. The mapping is passed as an `IEnumerable<KeyValuePair<char, char>>` where the first `char` represents the **input** character when enciphering, and the second `char` represents the **output** character when enciphering. If a character is not passed in the map, then it is assumed that the missing character is self-steckered (wired to itself). Like the other classes, the `Encipher(...)` method takes the input character, and presents the caller with the output `enciphered` and `deciphered` character. In this case, based on the steckering instead of the wheel.

### `class EnigmaSetupBuilder` (Builder)

This builder class is used to construct an initial state for the Enigma Machine to use when starting out or resetting. Call `AddStecker(...)` and `AddSteckers(...)` to add character mappings that have *not* been defined in the builder yet. Call `AddWheel(...)` and `AddWheels(...)` to add `WheelState`'s to be used by the `ICentralWheelCollection`. The `IWheel`'s passed to these methods (or `WheelState` constructors) should be acquired from the `IEnigmaMachine.AvailableWheels` property. The `Steckering` and `WheelSetups` properties can be used to access information on the current `EnigmaSetup` that will be built. The `Build()` method is the last thing to call after calling the `Add` methods. This will create an `EnigmaState` that the `IEnigmaMachine` will use to initialize and reset the `IWheelCollection` and `ISteckerboard` objects.

### `class EnigmaSetup` (Immutable)

The output result of the `EnigmaSetupBuilder.Build()` method. This is passed to the `IEnigmaMachine.Setup(...) method to perform initial setup on the machine's state.

### `interface IEnigmaMachine`

This interface defines the Enigma Machine that contains all other objects. Once `Setup(...)` is called once, the object can use the `Encipher(...)` and `Peek(...)` methods. These methoeds function the same as in the `ICentralWheelCollection`, except that the input is passed through the contained `ISteckerboard` object first. The `Encipher(string)` overload in `IEnigmaMachine` is used to encipher or decipher an entire message. After the function call, the machine will reset its state. `Reset()` can also be called to manually reset the state if the `Encipher(char)` method overload was used. The `AvailableWheels` property, as mentioned in `class EnigmaSetupBuilder`, is the collection of all defined `IWheel` types, each `IWheel` should contain a unique label to identify it. Other properties can be used to gauge the state of the machine, the output text, etc.

### Video Presentation

