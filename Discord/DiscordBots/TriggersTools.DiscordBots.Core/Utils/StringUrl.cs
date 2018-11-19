
namespace TriggersTools.DiscordBots.Utils {
	/// <summary>
	/// A string representation of a Url along with its location in text.
	/// </summary>
	public struct StringUrl {


		#region Constants

		/// <summary>
		/// Returns an empty non-url string url.
		/// </summary>
		public static readonly StringUrl None = new StringUrl() {
			Url = string.Empty,
			Start = -1,
			Length = 0,
		};

		#endregion

		#region Properties

		public bool IsSurrounded => Url.StartsWith("<");
		public string Url { get; set; }
		public string BaseUrl {
			get {
				if (IsSurrounded)
					return Url.Substring(1, Url.Length - 2);
				return Url;
			}
		}
		public int Start { get; set; }
		public int Length { get; set; }
		public int End {
			get => Start + Length;
			set => Length = value - Start;
		}

		#endregion

		public bool Contains(int index) {
			return (index >= Start && index < End);
		}

		public override bool Equals(object obj) {
			if (obj is StringUrl) {
				return this == ((StringUrl) obj);
			}
			return base.Equals(obj);
		}

		public override int GetHashCode() {
			return
				BitRotating.RotateLeft(Start, 2) ^
				BitRotating.RotateLeft(Length, 17);
		}
		
		public static bool operator ==(StringUrl a, StringUrl b) {
			return (a.Start == b.Start && a.Length == b.Length);
		}

		public static bool operator !=(StringUrl a, StringUrl b) {
			return (a.Start != b.Start || a.Length != b.Length);
		}
	}
}
