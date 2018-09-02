using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WJLCS {
	/// <summary>
	/// The setup for the <see cref="IEnigmaMachine"/>.<para/>
	/// This is used for initial setup and resetting.
	/// </summary>
	public class EnigmaSetup {

		#region Properties

		/// <summary>
		/// Gets the steckering used for the <see cref="ISteckerboard"/>.
		/// </summary>
		public ReadOnlyDictionary<char, char> Steckering { get; }

		/// <summary>
		/// Gets the wheel setups used for the <see cref="ICentralWheelCollection"/>.
		/// </summary>
		public ReadOnlyCollection<WheelSetup> WheelSetups { get; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the immutable <see cref="IEnigmaMachine"/> setup.
		/// </summary>
		/// <param name="steckering">The steckering to use for the <see cref="ISteckerboard"/>.</param>
		/// <param name="wheels">The wheel setups used for the <see cref="ICentralWheelCollection"/>.</param>
		public EnigmaSetup(Dictionary<char, char> steckering, List<WheelSetup> wheels) {
			// Create a copies so that these collections are immutable
			Steckering = new ReadOnlyDictionary<char, char>(new Dictionary<char, char>(steckering));
			WheelSetups = new ReadOnlyCollection<WheelSetup>(new List<WheelSetup>(wheels));
		}

		#endregion
	}
}
