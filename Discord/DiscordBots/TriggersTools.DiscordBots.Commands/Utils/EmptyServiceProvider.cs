using System;

namespace TriggersTools.DiscordBots.Commands.Utils {
	public class EmptyServiceProvider : IServiceProvider {
		public static readonly EmptyServiceProvider Instance = new EmptyServiceProvider();

		public object GetService(Type serviceType) => null;
	}
}
