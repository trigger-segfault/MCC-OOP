using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Configuration;
using TriggersTools.DiscordBots.Extensions;
using TriggersTools.DiscordBots.Services;

namespace TriggersTools.DiscordBots.Services {
	public class StatusRotationService : DiscordBotService {

		#region Private structs

		private struct DiscordStatus {
			/// <summary>
			/// Gets the activity type of the status.
			/// </summary>
			public ActivityType Activity { get; }
			/// <summary>
			/// Gets the name of the status.
			/// </summary>
			public string Name { get; }

			public DiscordStatus(ActivityType activity, string name) {
				Activity = activity;
				Name = name;
			}

			public override string ToString() => $"{Activity}: {Name}";
			public override bool Equals(object obj) {
				if (obj is DiscordStatus status)
					return this == status;
				return base.Equals(obj);
			}
			public override int GetHashCode() {
				return Activity.GetHashCode() ^ Name.GetHashCode();
			}

			public static bool operator ==(DiscordStatus a, DiscordStatus b) {
				return a.Activity == b.Activity && a.Name == b.Name;
			}
			public static bool operator !=(DiscordStatus a, DiscordStatus b) {
				return a.Activity != b.Activity || a.Name != b.Name;
			}
		}

		#endregion

		#region Fields

		private DiscordStatus currentStatus;
		private Timer timer;
		/// <summary>
		/// The random number generator for interval and status changes.
		/// </summary>
		private readonly Random rng;
		/// <summary>
		/// The array of statuses.
		/// </summary>
		private DiscordStatus[] statuses;
		/// <summary>
		/// The minimum interval between status changes.
		/// </summary>
		private TimeSpan minInterval;
		/// <summary>
		/// The maximum interval between status changes.
		/// </summary>
		private TimeSpan maxInterval;
		/// <summary>
		/// True if the statuses can change.
		/// </summary>
		private bool hasMultipleStatuses;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="StatusRotationService"/>.
		/// </summary>
		public StatusRotationService(DiscordBotServiceContainer services,
									 Random rng)
			: base(services)
		{
			this.rng = rng;

			Reload();
			DiscordBot.ConfigReloaded += OnReloadAsync;

			Client.Connected += OnConnected;
		}
		
		private Task OnReloadAsync() {
			Reload();
			return Task.FromResult<object>(null);
		}

		private void Reload() {
			timer?.Dispose();

			IConfigurationSection section = Config.GetSection("status");
			const string format = @"h\:mm";

			minInterval = TimeSpan.ParseExact(section["interval_min"], format, null);
			maxInterval = TimeSpan.ParseExact(section["interval_max"], format, null);
			if (minInterval <= TimeSpan.Zero)
				throw new ArgumentOutOfRangeException("status:interval_min", "Interval is too low!");
			if (maxInterval < minInterval)
				throw new ArgumentOutOfRangeException("status:interval_max", "Max interval is less than min interval!");

			string[] statusArray = section.GetArray("release");
			if (Debugger.IsAttached) {
				string[] debugStatusArray = section.GetArray("debug");
				if (debugStatusArray.Length != 0)
					statusArray = debugStatusArray;
			}

			ActivityType activity = ActivityType.Playing;
			List<DiscordStatus> statuses = new List<DiscordStatus>();
			foreach (string status in statusArray) {
				if (status.StartsWith("$") && status.EndsWith("$"))
					activity = (ActivityType) Enum.Parse(typeof(ActivityType), status.Substring(1, status.Length - 2), true);
				else
					statuses.Add(new DiscordStatus(activity, status));
			}
			this.statuses = statuses.ToArray();

			DiscordStatus firstStatus = statuses[0];
			hasMultipleStatuses = statuses.Skip(1).Any(s => s != firstStatus);
			OnTimerEllapsed();
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Called when the timer has ellapsed to update the current status.
		/// </summary>
		/// <param name="state"></param>
		private void OnTimerEllapsed(object state = null) {
			ChooseNewStatus();
			try {
				SetStatus(currentStatus).GetAwaiter().GetResult();
			}
			catch { }
			if (hasMultipleStatuses)
				RestartTimer();
		}
		/// <summary>
		/// Called when the bot reconnects to Discord.
		/// </summary>
		/// <returns></returns>
		private async Task OnConnected() {
			await SetStatus(currentStatus).ConfigureAwait(false);
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Restarts the timer with a new random interval.
		/// </summary>
		private void RestartTimer() {
			timer?.Dispose();
			double diffMinutes = maxInterval.TotalMinutes - minInterval.TotalMinutes;
			double x = rng.NextDouble();
			double newMinutes = minInterval.TotalMinutes + x * diffMinutes;
			TimeSpan newDuration = TimeSpan.FromMinutes(newMinutes);
			timer = new Timer(OnTimerEllapsed, null, newDuration, newDuration);
		}
		/// <summary>
		/// Chooses a new *different* status to use.
		/// </summary>
		private void ChooseNewStatus() {
			if (!hasMultipleStatuses) {
				currentStatus = statuses[0];
				return;
			}
			DiscordStatus newStatus = currentStatus;
			do {
				newStatus = statuses[rng.Next(statuses.Length)];
			} while (newStatus == currentStatus);

			currentStatus = newStatus;
		}
		/// <summary>
		/// Updates the current status.
		/// </summary>
		/// <returns></returns>
		private async Task SetStatus(DiscordStatus status) {
			await Client.SetGameAsync(status.Name, type: status.Activity).ConfigureAwait(false);
		}

		#endregion
	}
}
