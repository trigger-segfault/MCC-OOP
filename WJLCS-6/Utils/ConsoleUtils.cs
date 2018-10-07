using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WJLCS.Utils {
	public static class ConsoleUtils {


		/*public static string ReadMultiLine() {
			StringBuilder str = new StringBuilder();
			string line = Console.ReadLine();
			while (!string.IsNullOrEmpty(line)) {
				str.AppendLine(line);
				line = Console.ReadLine();
			}
			return str.ToString();
		}*/
		/// <summary>
		/// Don't use me. I'm broken as fuck.
		/// </summary>
		/// <returns>The read multiple lines.</returns>
		public static string ReadMultiLine() {
			StringBuilder str = new StringBuilder();
			while (Console.KeyAvailable)
				Console.ReadKey(true);
			var key = Console.ReadKey();
			while (key.Key != ConsoleKey.Enter || key.Modifiers != ConsoleModifiers.Shift) {
				if (key.Key == ConsoleKey.Enter) {
					//str.Append(Environment.NewLine);
					str.Append('\n');
					Console.WriteLine();
				}
				else {
					switch (key.KeyChar) {
					case '\0':
					case '\a':
					case '\b':
					case '\r':
					case '\v':
					case '\n':
						break;
					case '\t':
						str.Append("    ");
						break;
					default:
						str.Append(key.KeyChar);
						break;
					}
				}
				key = Console.ReadKey();
			}
			return str.ToString();//.Replace('\r', '\n');//.Replace("\0", "").Replace("\r\n", "\n").Replace('\r', '\n').Replace("\a", "");
			/*string line = Console.ReadLine();
			while (!string.IsNullOrEmpty(line)) {
				str.AppendLine(line);
				line = Console.ReadLine();
			}
			return str.ToString();*/
		}

		/// <summary>
		/// Reads a line with a larger buffersize instead of a measely 256 characters.
		/// </summary>
		/// <returns>The read line.</returns>
		public static string ReadLineLong() {
			//return Console.ReadLine();
			StringBuilder str = new StringBuilder();
			int index = 0;
			var key = Console.ReadKey(true);
			while (key.KeyChar != '\n' && key.KeyChar != '\r') {

				switch (key.Key) {
				case ConsoleKey.Backspace:
					if (str.Length > 0) {
						str.Remove(index - 1, 1);
						Console.Write($"{key.KeyChar} \b");
						index--;
					}
					break;
				case ConsoleKey.LeftArrow:
					if (index > 0) {
						index--;
						if (Console.CursorLeft > 0) {
							Console.CursorLeft--;
						}
						else if (Console.CursorTop > 0) {
							Console.CursorLeft = Console.BufferWidth - 1;
							Console.CursorTop--;
						}
					}
					break;
				case ConsoleKey.RightArrow:
					if (index < str.Length) {
						index++;
						if (Console.CursorLeft < Console.BufferWidth - 1) {
							Console.CursorLeft++;
						}
						else if (Console.CursorTop < Console.BufferHeight - 1) {
							Console.CursorLeft = 0;
							Console.CursorTop++;
						}
					}
					break;
				default:
					if (key.KeyChar != '\0') {
						str.Insert(index, key.KeyChar);
						Console.Write(key.KeyChar);
						index++;
					}
					break;
				}
				key = Console.ReadKey(true);
			}
			// Simiulate the newline that Console.ReadKey() doesn't.
			Console.WriteLine();
			return str.ToString();
		}

	}
}
