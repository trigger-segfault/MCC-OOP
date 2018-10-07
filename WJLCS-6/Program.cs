
using System;
using System.IO;

namespace WJLCS {
	class Program {
		static void Main(string[] args) {
			// Even this upper limit is a pain.
			Console.SetIn(new StreamReader(Console.OpenStandardInput(1024)));
			MenuDriver driver = new MenuDriver();
			driver.Run();
		}
	}
}
