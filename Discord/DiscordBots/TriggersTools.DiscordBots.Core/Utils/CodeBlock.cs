
namespace TriggersTools.DiscordBots.Utils {
	/// <summary>
	/// The type of block the code block is in.
	/// </summary>
	public enum CodeBlockType {
		/// <summary>
		/// There is no code block.
		/// </summary>
		None,
		/// <summary>
		/// The code block is an inline quote. (`)
		/// </summary>
		Quote,
		/// <summary>
		/// The code block is full size. (```)
		/// </summary>
		Full,
	}

	/// <summary>
	/// A Discord code block section.
	/// </summary>
	public struct CodeBlock {

		#region Constants

		/// <summary>
		/// Returns an empty code block.
		/// </summary>
		public static readonly CodeBlock None = new CodeBlock() {
			Start = -1,
			Length = 0,
		};

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the type of the code block.
		/// </summary>
		public CodeBlockType Type { get; set; }
		/// <summary>
		/// Gets or sets the start position of the code block.
		/// </summary>
		public int Start { get; set; }
		/// <summary>
		/// Gets or sets the start length of the code block.
		/// </summary>
		public int Length { get; set; }
		/// <summary>
		/// Gets or sets the start end of the code block.
		/// </summary>
		public int End {
			get => Start + Length;
			set => Length = value - Start;
		}

		#endregion

		#region Contains

		/// <summary>
		/// Returns true if the code block contains this index.
		/// </summary>
		/// <param name="index">The index to check for.</param>
		/// <returns>True if the index is within the code block's range.</returns>
		public bool Contains(int index) {
			if (Type != CodeBlockType.None)
				return (index >= Start && index < End);
			return false;
		}

		#endregion

		#region Object Overrides

		/// <summary>
		/// Returns true if the objects are equal.
		/// </summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns>True if the objects are equal.</returns>
		public override bool Equals(object obj) {
			if (obj is CodeBlock) {
				return this == ((CodeBlock) obj);
			}
			return base.Equals(obj);
		}
		/// <summary>
		/// Gets the hash code for the code block.
		/// </summary>
		public override int GetHashCode() {
			return (int) Type ^
				BitRotating.RotateLeft(Start, 2) ^
				BitRotating.RotateLeft(Length, 17);
		}
		/// <summary>
		/// Returns the string representation of the code block.
		/// </summary>
		public override string ToString() {
			return $"{Type}: {Start}, {Length}";
		}

		#endregion

		#region Casting

		/// <summary>
		/// Casts the <see cref="CodeBlock"/> to a <see cref="CodeBlockType"/>.
		/// </summary>
		public static implicit operator CodeBlockType(CodeBlock block) {
			return block.Type;
		}

		#endregion

		#region Operators

		public static bool operator ==(CodeBlock a, CodeBlock b) {
			return a.Type == b.Type && (a.Type == CodeBlockType.None ||
				(a.Start == b.Start && a.Length == b.Length));
		}

		public static bool operator !=(CodeBlock a, CodeBlock b) {
			return a.Type != b.Type || (a.Type != CodeBlockType.None &&
				(a.Start != b.Start || a.Length != b.Length));
		}

		#endregion
	}
}
