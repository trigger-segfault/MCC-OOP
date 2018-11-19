using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace TriggersTools.DiscordBots.Reactions {
	/// <summary>
	/// A category of reactions related to the Discord bot.
	/// </summary>
	public class ReactionCategoryInfo {
		/// <summary>
		/// Gets the name of the category.
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// Gets the reactions in this category.
		/// </summary>
		public IReadOnlyList<ReactionInfo> Reactions { get; }

		internal ReactionCategoryInfo(string category, IEnumerable<ReactionInfo> reactions) {
			Name = category;
			Reactions = reactions.ToImmutableArray();
		}
	}
}
