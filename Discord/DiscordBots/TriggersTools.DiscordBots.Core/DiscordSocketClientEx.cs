using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;

namespace TriggersTools.DiscordBots {
	public class DiscordSocketClientEx : DiscordSocketClient {

		#region Events

		/// <summary>
		/// Called for the first <see cref="DiscordSocketClient.Connected"/> event only.
		/// </summary>
		public event AsyncEventHandler FirstConnected {
			add => firstConnectedEvent.Add(value);
			remove => firstConnectedEvent.Remove(value);
		}
		private readonly AsyncEvent firstConnectedEvent = new AsyncEvent();
		/// <summary>
		/// Called for the first <see cref="DiscordSocketClient.Ready"/> event only.
		/// </summary>
		public event AsyncEventHandler FirstReady {
			add => firstReadyEvent.Add(value);
			remove => firstReadyEvent.Remove(value);
		}
		private readonly AsyncEvent firstReadyEvent = new AsyncEvent();

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new advanced REST/WebSocket-based Discord client.
		/// </summary>
		public DiscordSocketClientEx() : this(new DiscordSocketConfig()) { }
		/// <summary>
		/// Initializes a new advanced REST/WebSocket-based Discord client with the provided configuration.
		/// </summary>
		/// <param name="config">The configuration to be used with the client.</param>
		public DiscordSocketClientEx(DiscordSocketConfig config) : base(config) {
			Connected += OnConnectedAsync;
			Ready += OnReadyAysnc;
		}

		#endregion

		#region Properties


		#endregion

		#region Event Handlers

		/// <summary>
		/// Called when connected to Discord. Needed to raise the <see cref="FirstConnected"/> event.
		/// </summary>
		private async Task OnConnectedAsync() {
			Connected -= OnConnectedAsync;
			await firstConnectedEvent.InvokeAsync();
		}
		/// <summary>
		/// Called when connected to Discord. Needed to raise the <see cref="FirstReady"/> event.
		/// </summary>
		private async Task OnReadyAysnc() {
			Ready -= OnReadyAysnc;
			await firstReadyEvent.InvokeAsync();
		}

		#endregion

		/// <summary>
		/// The one-method call to startup the socket client and login.
		/// </summary>
		/// <param name="token">The secret token needed for login.</param>
		public async Task StartupAsync(string token) {
			await LoginAsync(TokenType.Bot, token).ConfigureAwait(false); // Login to discord
			await StartAsync().ConfigureAwait(false);                            // Connect to the websocket
		}
	}
}
