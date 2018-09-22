using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnigmaBot.Executable {
	class Program {
		static void Main(string[] args) {
			Console.Title = "EnigmaBot";
			ProcessStartInfo start = new ProcessStartInfo() {
				FileName = "dotnet",
				Arguments = "EnigmaBot.dll",
				UseShellExecute = false,
			};
			Process.Start(start).WaitForExit();
		}
	}
}
