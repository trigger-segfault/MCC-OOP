using System;
using System.Collections.Generic;
using System.Text;

namespace WJLCS.Enigma {
	/// <summary>
	/// Handling of unmapped characters.
	/// </summary>
	public enum UnmappedHandling {
		/// <summary>
		/// Characters are kept as they are in the message.
		/// </summary>
		Keep,
		/// <summary>
		/// Characters are switched to a specified invalid character.
		/// </summary>
		MakeInvalid,
		/// <summary>
		/// Characters are removed.
		/// </summary>
		Remove,
	}
	/// <summary>
	/// Setup arguments used to configure the Enigma Machine.
	/// </summary>
	public class SetupArgs {
		
		#region Constructors

		/// <summary>
		/// Constructs an empty <see cref="SetupArgs"/>.
		/// </summary>
		public SetupArgs() { }
		/// <summary>
		/// Constructs a <see cref="SetupArgs"/> with the specified letterset.
		/// </summary>
		/// <param name="letterSet">The letterset to use.</param>
		public SetupArgs(LetterSet letterSet) {
			LetterSet = letterSet ?? throw new ArgumentNullException(nameof(letterSet));
			int[] steckering = new int[LetterSet.Count];
			for (int i = 0; i < LetterSet.Count; i++)
				steckering[i] = i;
			Steckering = new Steckering(steckering);
		}

		#endregion

		#region Properties

		/// <summary>
		/// The machine letterset character range.
		/// </summary>
		public LetterSet LetterSet { get; set; }
		/// <summary>
		/// The plugboard steckering.
		/// </summary>
		public Steckering Steckering { get; set; }
		/// <summary>
		/// The prime numbers keys used for the rotors.
		/// </summary>
		public RotorKeys RotorKeys { get; set; }

		/// <summary>
		/// How to handle unmapped characters.
		/// </summary>
		public UnmappedHandling UnmappedHandling { get; set; } = UnmappedHandling.Keep;
		/// <summary>
		/// The invalid character to use if <see cref="UnmappedHandling"/> is set to
		/// <see cref="UnmappedHandling.MakeInvalid"/>.
		/// </summary>
		public char InvalidCharacter { get; set; } = '�';
		/// <summary>
		/// True if the rotors should rotate when encountering invalid characters.
		/// </summary>
		public bool RotateOnInvalid { get; set; } = false;

		#endregion

		#region Validation

		/// <summary>
		/// Validates the setup args to make sure they can be safely used.
		/// </summary>
		public void Validate() {
			if (LetterSet == null)
				throw new ArgumentNullException(nameof(LetterSet));
			if (Steckering == null)
				throw new ArgumentNullException(nameof(Steckering));
			if (RotorKeys == null)
				throw new ArgumentNullException(nameof(RotorKeys));
			if (Steckering.Count != LetterSet.Count)
				throw new ArgumentException($"Letterset and Steckering count do not match!\n" +
											$"Letterset: {LetterSet.Count}, Steckering: {Steckering.Count}",
											nameof(Steckering));
			if (!Enum.IsDefined(typeof(UnmappedHandling), UnmappedHandling))
				throw new ArgumentException("Enum value is undefined!", nameof(UnmappedHandling));
		}

		#endregion
	}
}
