using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace TriggersTools.DiscordBots {
	public class HandleResultEventArgs {
		public CommandInfo Command { get; }
		public ICommandContext Context { get; }
		public IResult Result { get; }
		public IEmote Reaction { get; private set; }
		public int ReactionScore { get; private set; }

		public HandleResultEventArgs(CommandInfo command, ICommandContext context, IResult result) {
			Command = command;
			Context = context;
			Result = result;
			ReactionScore = int.MinValue;
		}
		
		public bool Set(IEmote emote, int score = 0) {
			if (ReactionScore < score) {
				Reaction = emote;
				ReactionScore = score;
				return true;
			}
			return false;
		}
		public bool Set(string unicode, int score = 0) {
			if (ReactionScore < score) {
				Reaction = new Emoji(unicode);
				ReactionScore = score;
				return true;
			}
			return false;
		}
	}

	//public delegate Task OverrideReactionEventHandler(object sender, OverrideReactionEventArgs e);
}
