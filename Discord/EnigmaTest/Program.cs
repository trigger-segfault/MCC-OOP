using System;
using EnigmaMachine;

namespace EnigmaTest {
	class Program {
		static void Main(string[] args) {
			char[] letters = new char[127 - 32];
			for (int i = 0; i < 127 - 32; i++) {
				letters[i] = (char) (i + 32);
			}
			SetupArgs setup = new SetupArgs(new LetterSet(letters)) {
				ResetAfter = 50,
				RotateOnInvalid = true,
				InvalidCharacter = '?',
				RotorCount = 3,
				UnmappedHandling = UnmappedHandling.MakeInvalid,
			};
			setup.Steckering = setup.LetterSet.RandomizeSteckering();
			Machine machine = new Machine(setup);
			bool running = true;
			WriteTitle();
			while (running) {
				string command = ReadCommand();
				switch (command) {
				case "exit":
					running = false;
					break;
				case "clear":
					Clear();
					break;
				default:
					string enciphered = machine.Encipher(command);
					Console.WriteLine($"Enciphered: {enciphered}");

					string deciphered = machine.Decipher(enciphered);
					Console.WriteLine($"Deciphered: {deciphered}");
					Console.WriteLine();
					break;
				}
			}
		}

		static string ReadCommand() {
			Console.Write("> ");
			return Console.ReadLine();
		}

		static void WriteTitle() {
			Console.WriteLine("==== Enigma Machine ====");
		}

		static void Clear() {
			Console.Clear();
			WriteTitle();
		}
	}
}
