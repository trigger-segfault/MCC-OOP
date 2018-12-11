using System;
using System.IO;
using WJLCS.Utils;

namespace WJLCS {
	/// <summary>
	/// A static class for writing logs to a file.
	/// </summary>
	public static class Log {

		#region Constants

		/// <summary>
		/// The error log file.
		/// </summary>
		public const string FilePath = "ErrorLog.log";

		#endregion

		#region Logging

		/// <summary>
		/// Logs the raw text to the file.
		/// </summary>
		/// <param name="text">The line to log.</param>
		public static void LogRaw(string text) {
			try {
				// Make sure the file exists first
				if (!File.Exists(FilePath))
					File.Create(FilePath).Dispose();
				File.AppendAllText(FilePath, text);
			}
			catch {
				// Well, shit
			}
		}
		/// <summary>
		/// Logs the raw text to the file.
		/// </summary>
		/// <param name="line">The line to log.</param>
		public static void LogLine(string line) {
			LogRaw($"[{DateTime.UtcNow.ToString("HH:mm:ss")}] {line}{Environment.NewLine}");
		}
		/// <summary>
		/// Logs the info text to the file.
		/// </summary>
		/// <param name="line">The line to log.</param>
		public static void LogInfo(string line) {
			LogLine($"INFO: {line}");
		}
		/// <summary>
		/// Logs the warning text to the file.
		/// </summary>
		/// <param name="line">The line to log.</param>
		public static void LogWarning(string line) {
			LogLine($"WARN: {line}");
		}
		/// <summary>
		/// Logs the error text to the file.
		/// </summary>
		/// <param name="line">The line to log.</param>
		public static void LogError(string line) {
			LogLine($"ERR: {line}");
		}

		/// <summary>
		/// Logs the exception text to the file.
		/// </summary>
		/// <param name="exception">The exception to log.</param>
		public static void LogException(Exception exception) {
			LogError($"Unhandled Exception!{Environment.NewLine}{exception.ToStringWithInner()}");
		}
		/// <summary>
		/// Logs the missing files text to the file.
		/// </summary>
		/// <param name="missingFiles">The missing files to log.</param>
		public static void LogMissingFiles(string[] missingFiles) {
			const string Prefix = "- ";
			LogError($"Missing Runtime Files!{Environment.NewLine}" +
					 $"{Prefix}{string.Join($"{Environment.NewLine}{Prefix}", missingFiles)}");
		}
		
		#endregion
	}
}
