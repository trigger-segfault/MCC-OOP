using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TriggersTools.DiscordBots.Extensions;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// Provides a extended base class for a command module to inherit from.
	/// </summary>
	/// <typeparam name="TContext">A class that implements <see cref="ICommandContext"/>.</typeparam>
	public class ModuleBaseEx<TContext> : ModuleBase<TContext> where TContext : class, ICommandContext {

		#region ReplyFileAsync

		/// <summary>
		/// Sends a file to this message channel with an optional caption.
		/// </summary>
		/// <param name="filePath">The file path of the file.</param>
		/// <param name="text">The message to be sent.</param>
		/// <param name="isTTS">Whether the message should be read aloud by Discord or not.</param>
		/// <param name="embed">The <see cref="EmbedType.Rich"/> <see cref="Embed"/> to be send.</param>
		/// <param name="options">The options to be used when sending the request.</param>
		/// <returns>
		/// A task that represents an asynchronous send operation for delivering the message. The task result
		/// contains the sent message.
		/// </returns>
		/// <remarks>
		/// This method sends a file as if you are uploading an attachment directly from your Discord client.
		/// If you wish to upload an image and have it embedded in a <see cref="EmbedType.Rich"/> embed, you
		/// may upload the file and refer to the file with "attachment://filename.ext" in the
		/// <see cref="EmbedBuilder.ImageUrl"/>. See the example section for its usage.
		/// </remarks>
		protected Task<IUserMessage> ReplyFileAsync(string filePath, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null) {
			return Context.Channel.SendFileAsync(filePath, text, isTTS, embed, options);
		}
		/// <summary>
		/// Sends a file to this message channel with an optional caption.
		/// </summary>
		/// <param name="stream">The <see cref="Stream"/> of the file to be sent.</param>
		/// <param name="filename">The name of the attachment.</param>
		/// <param name="text">The message to be sent.</param>
		/// <param name="isTTS">Whether the message should be read aloud by Discord or not.</param>
		/// <param name="embed">The <see cref="EmbedType.Rich"/> <see cref="Embed"/> to be send.</param>
		/// <param name="options">The options to be used when sending the request.</param>
		/// <returns>
		/// A task that represents an asynchronous send operation for delivering the message. The task result
		/// contains the sent message.
		/// </returns>
		/// <remarks>
		/// This method sends a file as if you are uploading an attachment directly from your Discord client.
		/// If you wish to upload an image and have it embedded in a <see cref="EmbedType.Rich"/> embed, you
		/// may upload the file and refer to the file with "attachment://filename.ext" in the
		/// <see cref="EmbedBuilder.ImageUrl"/>. See the example section for its usage.
		/// </remarks>
		protected Task<IUserMessage> ReplyFileAsync(Stream stream, string filename, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null) {
			return Context.Channel.SendFileAsync(stream, filename, text, isTTS, embed, options);
		}
		/// <summary>
		/// Sends a file to this message channel with an optional caption.
		/// </summary>
		/// <param name="stream">The <see cref="Stream"/> of the file to be sent.</param>
		/// <param name="filename">The name of the attachment.</param>
		/// <param name="text">The message to be sent.</param>
		/// <param name="isTTS">Whether the message should be read aloud by Discord or not.</param>
		/// <param name="embed">The <see cref="EmbedType.Rich"/> <see cref="Embed"/> to be send.</param>
		/// <param name="options">The options to be used when sending the request.</param>
		/// <returns>
		/// A task that represents an asynchronous send operation for delivering the message. The task result
		/// contains the sent message.
		/// </returns>
		/// <remarks>
		/// This method sends a file as if you are uploading an attachment directly from your Discord client.
		/// If you wish to upload an image and have it embedded in a <see cref="EmbedType.Rich"/> embed, you
		/// may upload the file and refer to the file with "attachment://filename.ext" in the
		/// <see cref="EmbedBuilder.ImageUrl"/>. See the example section for its usage.
		/// </remarks>
		protected Task<IUserMessage> ReplyFileAsync(byte[] data, string filename, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null) {
			return Context.Channel.SendFileAsync(data, filename, text, isTTS, embed, options);
		}

		#endregion


		public Task<IUserMessage> ReplyBitmapAsync(Bitmap bitmap, string filename, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null) {
			return Context.Channel.SendBitmapAsync(bitmap, filename, text, isTTS, embed, options);
		}
		public Task<IUserMessage> ReplyBitmapAsync(Bitmap bitmap, ImageFormat format, string filename, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null) {
			return Context.Channel.SendBitmapAsync(bitmap, format, filename, text, isTTS, embed, options);
		}
	}
}
