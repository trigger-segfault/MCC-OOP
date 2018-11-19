using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using TriggersTools.DiscordBots.Commands;

namespace TriggersTools.DiscordBots.Context {
	public enum ConfirmationType {
		Yes,
		Digit4,
		Repeat,
	}
	/// <summary>
	/// A wait context for confirming a question.
	/// </summary>
	public class ConfirmationUserWaitContext : DiscordBotUserWaitContext {

		#region Fields

		public ConfirmationType Type { get; }
		public string Confirmation { get; }
		public IUserMessage StartMessage { get; private set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="ConfirmationUserWaitContext"/>.
		/// </summary>
		/// <param name="context">The underlying command context.</param>
		public ConfirmationUserWaitContext(DiscordBotCommandContext context, string name, TimeSpan duration,
			ConfirmationType type, string confirmation = null)
			: base(context, name ?? "Confirmation", duration)
		{
			Type = type;
			switch (Type) {
			case ConfirmationType.Yes:
				Confirmation = "yes";
				break;
			case ConfirmationType.Digit4:
				Random rng = Services.GetService<Random>();
				Confirmation = rng.Next(10000).ToString();
				break;
			case ConfirmationType.Repeat:
				Confirmation = confirmation ?? throw new ArgumentNullException(nameof(confirmation));
				break;
			}
			HookEvents();
		}
		/// <summary>
		/// Constructs the <see cref="ConfirmationUserWaitContext"/>.
		/// </summary>
		/// <param name="context">The underlying command context.</param>
		public ConfirmationUserWaitContext(DiscordBotCommandContext context, string name, TimeSpan duration,
			string confirmation)
			: base(context, name ?? "Confirmation", duration)
		{
			Type = ConfirmationType.Repeat;
			Confirmation = confirmation ?? throw new ArgumentNullException(nameof(confirmation));
			HookEvents();
		}

		#endregion

		public void HookEvents() {
			Ended += OnEndedAsync;
			Started += OnStartedAsync;
			Expired += OnExpiredAsync;
			Canceled += OnCanceledAsync;
			MessageReceived += OnMessageReceivedAsync;
		}

		private async Task OnEndedAsync(SocketUserWaitContext arg) {
			await StartMessage.DeleteAsync().ConfigureAwait(false);
		}
		private async Task OnStartedAsync(SocketUserWaitContext arg) {
			StartMessage = await OutputChannel.SendMessageAsync($"**{Name}:** Type `{Format.Sanitize(Confirmation)}` to complete or type `cancel` to stop").ConfigureAwait(false);
		}
		private async Task OnExpiredAsync(SocketUserWaitContext arg) {
			await OutputChannel.SendMessageAsync($"**{Name}:** Timed out!").ConfigureAwait(false);
		}
		private async Task OnCanceledAsync(SocketUserWaitContext arg) {
			await OutputChannel.SendMessageAsync($"**{Name}:** Was canceled!").ConfigureAwait(false);
		}
		private async Task OnMessageReceivedAsync(SocketUserWaitContext context, IUserMessage msg) {
			if (OutputChannel.Id != msg.Channel.Id)
				return;
			string input = msg.Content.ToLower().Trim();
			if (input == "cancel") {
				await CancelAsync().ConfigureAwait(false);
			}
			else if (input == Confirmation) {
				await FinishAsync().ConfigureAwait(false);
			}
		}
	}
}
