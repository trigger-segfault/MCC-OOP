using System;
using System.Collections.Generic;
using System.Text;

namespace TriggersTools.DiscordBots.Database {
	public struct EndUserDataInfo {

		#region Fields

		public string Key { get; }
		public string Description { get; }

		#endregion

		#region Constructors

		public EndUserDataInfo(string key, string description) {
			Key = key;
			Description = description;
		}

		public EndUserDataInfo(KeyValuePair<string, string> pair) {
			Key = pair.Key;
			Description = pair.Value;
		}

		#endregion

		#region Object Overrides

		public override int GetHashCode() => Key.GetHashCode();
		public override bool Equals(object obj) {
			if (obj is EndUserDataInfo eud) {
				return Key.Equals(eud.Key);
			}
			else if (obj is string s) {
				return Key == s;
			}
			return base.Equals(obj);
		}
		public override string ToString() => $"{Key}: {Description}";

		#endregion

		#region Boolean Operators

		public static bool operator ==(EndUserDataInfo a, EndUserDataInfo b) {
			return a.Key == b.Key;
		}
		public static bool operator !=(EndUserDataInfo a, EndUserDataInfo b) {
			return a.Key != b.Key;
		}

		#endregion

		#region Casting

		public static implicit operator string(EndUserDataInfo eud) {
			return eud.Key;
		}

		#endregion
	}
}
