using System.Text;

namespace WJLCS.Enigma {
	/// <summary>
	/// The Enigma Machine used to encipher and decipher test.
	/// </summary>
	public class Machine : IStringEncipherer {

		#region Fields

		/// <summary>
		/// The plugboard managing enciphering.
		/// </summary>
		private readonly Plugboard plugboard;
		/// <summary>
		/// The rotor collection managing enciphering.
		/// </summary>
		private readonly RotorCollection rotors;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="Machine"/> with the specified arguments.
		/// </summary>
		/// <param name="args">The setup args to use.</param>
		public Machine(SetupArgs args) {
			args.Validate();
			plugboard = new Plugboard(args.LetterSet, args.Steckering);
			rotors = new RotorCollection(args.LetterSet, args.RotorKeys);

			LetterSet = args.LetterSet;
			Steckering = args.Steckering;
			UnmappedHandling = args.UnmappedHandling;
			InvalidCharacter = args.InvalidCharacter;
			RotateOnInvalid = args.RotateOnInvalid;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the letterset used in the Enigma Machine.
		/// </summary>
		public LetterSet LetterSet { get; }
		/// <summary>
		/// Gets the steckering used in the Enigma Machine.
		/// </summary>
		public Steckering Steckering { get; }
		/// <summary>
		/// Gets how unmapped characters are treated when enciphering and deciphering.
		/// </summary>
		public UnmappedHandling UnmappedHandling { get; }
		/// <summary>
		/// Gets the invalid character used when <see cref="UnmappedHandling"/> is set to
		/// <see cref="UnmappedHandling.MakeInvalid"/>.
		/// </summary>
		public char InvalidCharacter { get; }
		/// <summary>
		/// Gets if the rotor collection should rotote the rotors when it encounters invalid characters.
		/// </summary>
		public bool RotateOnInvalid { get; }

		#endregion

		#region Enciphering

		/// <summary>
		/// Enciphers the specified text.
		/// </summary>
		/// <param name="text">The text to encipher.</param>
		/// <returns>The enciphered text.</returns>
		public string Encipher(string text) {
			StringBuilder str = new StringBuilder();
			for (int i = 0; i < text.Length; i++) {
				char c = Encipher(text[i]);
				if (c != '\0')
					str.Append(c);
			}
			Reset();
			return str.ToString();
		}
		/// <summary>
		/// Deciphers the specified text.
		/// </summary>
		/// <param name="text">The text to decipher.</param>
		/// <returns>The deciphered text.</returns>
		public string Decipher(string text) {
			StringBuilder str = new StringBuilder();
			for (int i = 0; i < text.Length; i++) {
				char c = Decipher(text[i]);
				if (c != '\0')
					str.Append(c);
			}
			Reset();
			return str.ToString();
		}
		/// <summary>
		/// Enciphers the specified character.
		/// </summary>
		/// <param name="c">The character to encipher.</param>
		/// <param name="peek">True if the rotors should not be rotated.</param>
		/// <returns>The enciphered character.</returns>
		public char Encipher(char c, bool peek = false) {
			int index = LetterSet.IndexOf(c);
			if (index == -1)
				return HandleInvalid(c, peek);

			index = plugboard.Encipher(index);
			index = rotors.Encipher(index, peek);
			return LetterSet[index];
		}
		/// <summary>
		/// Deciphers the specified character.
		/// </summary>
		/// <param name="c">The character to decipher.</param>
		/// <param name="peek">True if the rotors should not be rotated.</param>
		/// <returns>The deciphered character.</returns>
		public char Decipher(char c, bool peek = false) {
			int index = LetterSet.IndexOf(c);
			if (index == -1)
				return HandleInvalid(c, peek);

			index = rotors.Decipher(index, peek);
			index = plugboard.Decipher(index);
			return LetterSet[index];
		}
		/// <summary>
		/// Resets the rotors on the Enigma Machine.
		/// </summary>
		public void Reset() {
			rotors.Reset();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Handles enciphering for an invalid character.
		/// </summary>
		/// <param name="c">The invalid character to handle.</param>
		/// <param name="peek">True if the rotors should not be rotated at all.</param>
		/// <returns>The handled character.</returns>
		private char HandleInvalid(char c, bool peek) {
			if (UnmappedHandling == UnmappedHandling.Remove)
				return '\0';
			if (!peek && RotateOnInvalid)
				rotors.Rotate();
			if (UnmappedHandling == UnmappedHandling.MakeInvalid)
				return InvalidCharacter;
			return c;
		}

		#endregion
	}
}
