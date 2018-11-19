using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using Discord;

namespace TriggersTools.DiscordBots.Reactions {
	/// <summary>
	/// Information on a reaction that the bot may use.
	/// </summary>
	public class ReactionInfo {
		/// <summary>
		/// The property name of the reaction.
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// The emote related to the reaction.
		/// </summary>
		public IEmote Emote { get; }
		/// <summary>
		/// The description of the reaction.
		/// </summary>
		public string Description { get; }
		/// <summary>
		/// The category this reaction is classified in.
		/// </summary>
		public string Category { get; }

		internal ReactionInfo(FieldInfo field, object instance) {
			Name = field.Name;
			if (field.IsStatic)
				Emote = (IEmote) field.GetValue(null);
			else
				Emote = (IEmote) field.GetValue(instance);

			var desc = field.GetCustomAttribute<DescriptionAttribute>();
			Description = desc?.Description ?? "No description available";

			var cat = field.GetCustomAttribute<CategoryAttribute>();
			Category = cat?.Category ?? "Misc";
		}
		internal ReactionInfo(PropertyInfo prop, object instance) {
			Name = prop.Name;
			if (prop.GetMethod.IsStatic)
				Emote = (IEmote) prop.GetValue(null);
			else
				Emote = (IEmote) prop.GetValue(instance);

			var desc = prop.GetCustomAttribute<DescriptionAttribute>();
			Description = desc?.Description ?? "No description available";

			var cat = prop.GetCustomAttribute<CategoryAttribute>();
			Category = cat?.Category ?? "Misc";
		}
	}
}
