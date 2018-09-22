using Discord.Net.WebSockets;
using System;

namespace Discord.Net.Providers.WS4Net {
	public static class WS4NetProvider {
		public static readonly WebSocketProvider Instance = () => new WS4NetClient();

		public static bool IsRequiredByOS {
			get {
				return Environment.OSVersion.Platform == PlatformID.Win32NT &&
						Environment.OSVersion.Version.Major == 6 &&
						Environment.OSVersion.Version.Minor <= 1;
			}
		}
	}
}
