using System;
using System.IO;

namespace WJLCS {
	/// <summary>
	/// The class for running the program.
	/// </summary>
	class Program {
		/// <summary>
		/// The main entry point.
		/// </summary>
		/// <param name="args">The command line arguments.</param>
		static void Main(string[] args) {
			Console.Title = "Enigma Machine";

			// Even this upper limit is a pain.
			Console.SetIn(new StreamReader(Console.OpenStandardInput(1024)));

			MenuDriver driver = new MenuDriver();
			driver.Run();
		}
	}
}
