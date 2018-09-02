using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WJLCS {
	/// <summary>
	/// A builder class for the <see cref="EnigmaSetup"/> state.
	/// </summary>
	public class EnigmaSetupBuilder {

		#region Fields

		/// <summary>
		/// The steckering used for the <see cref="ISteckerboard"/>.
		/// </summary>
		private readonly Dictionary<char, char> steckering = new Dictionary<char, char>();
		/// <summary>
		/// The wheel setups used for the <see cref="ICentralWheelCollection"/>.
		/// </summary>
		private readonly List<WheelSetup> wheels = new List<WheelSetup>();

		#endregion

		#region Properties

		/// <summary>
		/// Gets the steckering used for the <see cref="ISteckerboard"/>.
		/// </summary>
		public ReadOnlyDictionary<char, char> Steckering => new ReadOnlyDictionary<char, char>(steckering);

		/// <summary>
		/// Gets the wheel setups used for the <see cref="ICentralWheelCollection"/>.
		/// </summary>
		public ReadOnlyCollection<WheelSetup> WheelSetups => new ReadOnlyCollection<WheelSetup>(wheels);

		#endregion

		#region Setup

		/// <summary>
		/// Adds a stecker to the <see cref="ISteckerboard"/> setup.
		/// </summary>
		/// <param name="input">The input character to map to.</param>
		/// <param name="output">The output character to map to.</param>
		public void AddStecker(char input, char output) {
			steckering.Add(input, output);
		}

		/// <summary>
		/// Adds many steckers to the <see cref="ISteckerboard"/> setup.
		/// </summary>
		/// <param name="steckers">The steckers to add.</param>
		public void AddSteckers(IEnumerable<KeyValuePair<char, char>> steckers) {
			foreach (var pair in steckers)
				steckering.Add(pair.Key, pair.Value);
		}

		/// <summary>
		/// Adds a wheel setup to the <see cref="ICentralWheelCollection"/> setup.
		/// </summary>
		/// <param name="wheel">The wheel to use.</param>
		/// <param name="initialPosition">The initial position of the wheel.</param>
		/// <param name="turnoverPosition">The turnover position of the wheel.</param>
		public void AddWheel(IWheel wheel, int initialPosition = 0, int turnoverPosition = 0) {
			wheels.Add(new WheelSetup(wheel, initialPosition, turnoverPosition));
		}

		/// <summary>
		/// Adds a wheel setup to the <see cref="ICentralWheelCollection"/> setup.
		/// </summary>
		/// <param name="wheelSetup">The wheel setup to use.</param>
		public void AddWheel(WheelSetup wheelSetup) {
			wheels.Add(wheelSetup);
		}

		/// <summary>
		/// Adds many wheel setups to the <see cref="ICentralWheelCollection"/> setup.
		/// </summary>
		/// <param name="wheels">The wheel setups to add.</param>
		public void AddWheels(IEnumerable<WheelSetup> wheels) {
			this.wheels.AddRange(wheels);
		}

		#endregion

		#region Build

		/// <summary>
		/// Builds the <see cref="EnigmaSetup"/> state.
		/// </summary>
		/// <returns>The newly built <see cref="EnigmaSetup"/>.</returns>
		public EnigmaSetup Build() {
			return new EnigmaSetup(steckering, wheels);
		}

		#endregion
	}
}
