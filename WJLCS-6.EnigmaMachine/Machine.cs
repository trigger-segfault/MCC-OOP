using System;
using System.Text;

namespace WJLCS.Enigma {
	public class Machine {
		
		#region Fields

		private readonly Plugboard plugboard;
		private readonly RotorCollection rotors;

		#endregion

		#region Constructors

		public Machine(SetupArgs args) {
			args.Validate();
			plugboard = new Plugboard(args.LetterSet, args.Steckering);
			rotors = new RotorCollection(args.LetterSet, args.RotorCount);

			LetterSet = args.LetterSet;
			UnmappedHandling = args.UnmappedHandling;
			InvalidCharacter = args.InvalidCharacter;
			RotateOnInvalid = args.RotateOnInvalid;
			ResetAfter = args.ResetAfter;
		}

		#endregion
		
		public LetterSet LetterSet { get; }
		public UnmappedHandling UnmappedHandling { get; }
		public char InvalidCharacter { get; }
		public bool RotateOnInvalid { get; }
		public int ResetAfter { get; }

		#region Enciphering

		public string Encipher(string text) {
			StringBuilder str = new StringBuilder();
			for (int i = 0; i < text.Length; i++) {
				if (i % ResetAfter == 0)
					Reset();
				char c = Encipher(text[i]);
				if (c != '\0')
					str.Append(c);
			}
			Reset();
			return str.ToString();
		}

		public string Decipher(string text) {
			StringBuilder str = new StringBuilder();
			for (int i = 0; i < text.Length; i++) {
				if (i % ResetAfter == 0)
					Reset();
				char c = Decipher(text[i]);
				if (c != '\0')
					str.Append(c);
			}
			Reset();
			return str.ToString();
		}

		public char Encipher(char c, bool peek = false) {
			int index = LetterSet.IndexOf(c);
			if (index == -1)
				return HandleInvalid(c, peek);

			index = plugboard.Encipher(index);
			index = rotors.Encipher(index, peek);
			return LetterSet[index];
		}
		public char Decipher(char c, bool peek = false) {
			int index = LetterSet.IndexOf(c);
			if (index == -1)
				return HandleInvalid(c, peek);

			index = rotors.Decipher(index, peek);
			index = plugboard.Decipher(index);
			return LetterSet[index];
		}

		public void Reset() {
			rotors.Reset();
		}

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
