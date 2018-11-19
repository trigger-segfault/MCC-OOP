using Microsoft.CodeAnalysis;

namespace TriggersTools.DiscordBots.Analyzer {
	internal sealed class PreconditionerResult {
		public bool NeedsRequires { get; }
		public Location Location { get; }
		//public string AttributeName { get; }
		//public string SuggestedName { get; }

		public PreconditionerResult(bool needsRequires, /*string attributeName,*/ Location location) {
			NeedsRequires = needsRequires;
			Location = location;
			/*if (needsRequires) {
				AttributeName = attributeName;
				SuggestedName = attributeName.Insert(Preconditioner.Require.Length, "s");
			}*/
		}
	}
}
