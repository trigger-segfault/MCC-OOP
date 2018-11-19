using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using TriggersTools.DiscordBots.Services;
using TriggersTools.DiscordBots.Utils;

namespace TriggersTools.DiscordBots.Reactions {
	/// <summary>
	/// An abstract service for defining all reactions a bot works with.
	/// </summary>
	public abstract class ReactionService : DiscordBotService {
		
		#region Constructors

		/// <summary>
		/// Constructs the <see cref="ReactionService"/>.
		/// </summary>
		public ReactionService(DiscordBotServiceContainer services) : base(services) {
			Client.Connected += OnFirstConnectedAsync;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the collection of categories of reactions.
		/// </summary>
		public IReadOnlyList<ReactionCategoryInfo> Categories { get; private set; }
		/// <summary>
		/// Gets the collection of all reactions.
		/// </summary>
		public IReadOnlyList<ReactionInfo> Reactions { get; private set; }

		#endregion

		#region Event Handlers

		private Task OnFirstConnectedAsync() {
			Client.Connected -= OnFirstConnectedAsync;
			Dictionary<string, List<ReactionInfo>> categories = new Dictionary<string, List<ReactionInfo>>();
			Dictionary<string, ReactionInfo> reactions = new Dictionary<string, ReactionInfo>();

			Type type = GetType();
			Type emoteType = typeof(IEmote);
			foreach (PropertyInfo prop in type.GetProperties()) {
				if (emoteType.IsAssignableFrom(prop.PropertyType)) {
					ReactionInfo reaction = new ReactionInfo(prop, this);
					if (!categories.TryGetValue(reaction.Category, out var categoryList)) {
						categoryList = new List<ReactionInfo>();
						categories.Add(reaction.Category, categoryList);
					}
					categoryList.Add(reaction);
					reactions.Add(reaction.Name, reaction);
				}
			}
			Categories = categories
				//.OrderBy(p => p.Key, StringComparer.InvariantCultureIgnoreCase)
				.Select(p => new ReactionCategoryInfo(p.Key, p.Value))
				.ToImmutableArray();
			Reactions = reactions
				//.OrderBy(p => p.Key, StringComparer.InvariantCultureIgnoreCase)
				.Select(p => p.Value)
				.ToImmutableArray();

			return Task.FromResult<object>(null);
		}

		#endregion

	}
}
