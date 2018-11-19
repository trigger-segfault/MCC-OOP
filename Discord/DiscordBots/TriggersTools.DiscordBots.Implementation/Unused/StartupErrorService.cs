using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TriggersTools.DiscordBots.Utils;

namespace TriggersTools.DiscordBots.Services {
	public class StartupErrorService : IDiscordBotService {
		//public DiscordBotServiceContainer Services { get; }

		public StartupErrorService() {
			AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
			TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;
		}


		public void Initialize() {
			AppDomain.CurrentDomain.UnhandledException -= OnAppDomainUnhandledException;
			TaskScheduler.UnobservedTaskException -= OnTaskSchedulerUnobservedTaskException;
		}

		private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e) {
			LogException(e.ExceptionObject as Exception);
		}
		private void OnTaskSchedulerUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e) {
			LogException(e.Exception as Exception);
		}

		private void LogException(Exception ex) {
			if (ex == null || ex is AggregateException)
				return;
			using (ConsoleLock.Lock(ConsoleColor.Red)) {
				Console.WriteLine($"STARTUP EXCEPTION: {ex}");
			}
		}
	}
}
