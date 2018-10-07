using System;

namespace WJLCS.Setup {
	/// <summary>
	/// The class for managing rotor setup.
	/// </summary>
	public class RotorConfigurer {

		#region Properties

		/// <summary>
		/// Gets the number of rotors in the Enigma Machine.
		/// </summary>
		public int RotorCount { get; private set; } = 3;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the Enigma Machine <see cref="RotorConfigurer"/>.
		/// </summary>
		public RotorConfigurer() { }

		#endregion

		#region Configure

		/// <summary>
		/// Runs the rotor count configurer.
		/// </summary>
		/// 
		/// <exception cref="FormatException">
		/// An input rotor count is not an integer.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The input rotor count is less than one.
		/// </exception>
		public void ConfigureRotorCount(string input) {
			if (!int.TryParse(input, out int count))
				throw new FormatException("Input is not a valid  integer!");
			if (count < 1)
				throw new ArgumentOutOfRangeException("rotorCount", "Input is less than 1!");
			RotorCount = count;
		}

		#endregion
	}
}
