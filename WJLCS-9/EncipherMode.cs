
namespace WJLCS {
	/// <summary>
	/// The mode for how the encipherer or decipherer receives its text.
	/// </summary>
	public enum EncipherMode {
		/// <summary>Ask the user for input.</summary>
		Input,
		/// <summary>Paste the contents of the clipboard.</summary>
		Paste,
		/// <summary>Read or write to an HTML file. Only supported with Decipher.</summary>
		HTML,
	}
}
