
namespace WJLCS {
	/// <summary>
	/// The mode for how an interpreter screen receives its text.
	/// </summary>
	public enum EnterMode {
		/// <summary>Ask the user for input.</summary>
		Input,
		/// <summary>Paste the contents of the clipboard.</summary>
		Paste,
		/// <summary>Read or write to a file.</summary>
		File,
	}
}
