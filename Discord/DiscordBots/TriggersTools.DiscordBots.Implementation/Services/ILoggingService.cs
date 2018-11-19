using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace TriggersTools.DiscordBots.Services {
	/// <summary>
	/// A service interface for logging messages.
	/// </summary>
	public interface ILoggingService {
		#region Logging

		/// <summary>
		/// Logs the log message.
		/// </summary>
		/// <param name="msg">The log message to log.</param>
		/// <param name="logFile">True if the log file should be written to.</param>
		/// <param name="errorFile">True if the error file should be written to.</param>
		/// <param name="noticeFile">True if the notice file should be written to.</param>
		void Log(LogMessage msg, bool logFile = false, bool errorFile = false, bool noticeFile = false);
		/// <summary>
		/// Asyncronously logs the log message.
		/// </summary>
		/// <param name="msg">The log message to log.</param>
		/// <param name="logFile">True if the log file should be written to.</param>
		/// <param name="errorFile">True if the error file should be written to.</param>
		Task LogAsync(LogMessage msg, bool logFile = true, bool errorFile = false, bool noticeFile = false);

		/// <summary>
		/// Writes the debug line to the console.
		/// </summary>
		/// <param name="line">The text to write.</param>
		void Debug(string line);
		/// <summary>
		/// Asynchornously writes the debug line to the console.
		/// </summary>
		/// <param name="line">The text to write.</param>
		Task DebugAsync(string line);
		/// <summary>
		/// Writes the debug trace to the console.
		/// </summary>
		/// <param name="line">The text to write.</param>
		void Trace(string line);
		/// <summary>
		/// Asynchornously writes the trace line to the console.
		/// </summary>
		/// <param name="line">The text to write.</param>
		Task TraceAsync(string line);
		/// <summary>
		/// Writes the line to the console.
		/// </summary>
		/// <param name="line">The text to write.</param>
		void Write(string line);
		/// <summary>
		/// Asynchornously writes the line to the console.
		/// </summary>
		/// <param name="line">The text to write.</param>
		Task WriteAsync(string line);

		#endregion
	}
}
