using System;
using System.Collections.Generic;
using System.Text;

namespace EnigmaMachine {
	public enum UnmappedHandling {
		Keep,
		MakeInvalid,
		Remove,
	}
	public class SetupArgs {

		#region Constructors

		public SetupArgs(LetterSet letterSet) {
			LetterSet = letterSet ?? throw new ArgumentNullException(nameof(letterSet));
			Steckering = new int[LetterSet.Count];
			for (int i = 0; i < LetterSet.Count; i++)
				Steckering[i] = i;
		}

		#endregion

		#region Properties

		public LetterSet LetterSet { get; set; }
		public int[] Steckering { get; set; }
		public int ResetAfter { get; set; } = 50;
		public UnmappedHandling UnmappedHandling { get; set; } = UnmappedHandling.Keep;
		public char InvalidCharacter { get; set; } = '�';
		public int RotorCount { get; set; } = 3;
		public bool RotateOnInvalid { get; set; } = false;

		#endregion

		#region Validation

		public void Validate() {
			if (LetterSet == null)
				throw new ArgumentNullException(nameof(LetterSet));
			if (Steckering == null)
				throw new ArgumentNullException(nameof(Steckering));
			if (Steckering.Length != LetterSet.Count)
				throw new ArgumentException(nameof(Steckering));
			if (!Enum.IsDefined(typeof(UnmappedHandling), UnmappedHandling))
				throw new ArgumentException(nameof(UnmappedHandling));
			if (RotorCount < 1)
				throw new ArgumentException(nameof(RotorCount));
			if (ResetAfter < 1)
				throw new ArgumentException(nameof(RotorCount));
		}

		#endregion
	}
}
