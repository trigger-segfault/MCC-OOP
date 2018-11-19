using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace TriggersTools.DiscordBots.Extensions {
	public static class BitmapExtensions {

		/*public static Task<IUserMessage> ReplyBitmapAsync<T>(this ModuleBase<T> module, Bitmap bitmap, string filename, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null)
			where T : class, ICommandContext
		{
			return module.ReplyBitmapAsync(bitmap, ImageFormat.Png, filename, text, isTTS, embed, options);
		}
		public static Task<IUserMessage> ReplyBitmapAsync<T>(this ModuleBase<T> module, Bitmap bitmap, ImageFormat format, string filename, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null)
			where T : class, ICommandContext
		{
			using (var stream = new MemoryStream()) {
				bitmap.Save(stream, format);
				return module.Context.Channel.SendFileAsync(stream, filename, text, isTTS, embed, options);
			}
		}

		*/
		public static Task<IUserMessage> SendBitmapAsync(this IMessageChannel channel, Bitmap bitmap, string filename, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null) {
			return channel.SendBitmapAsync(bitmap, ImageFormat.Png, filename, text, isTTS, embed, options);
		}
		public static async Task<IUserMessage> SendBitmapAsync(this IMessageChannel channel, Bitmap bitmap, ImageFormat format, string filename, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null) {
			using (var stream = new MemoryStream()) {
				bitmap.Save(stream, format);
				stream.Position = 0;
				return await channel.SendFileAsync(stream, filename, text, isTTS, embed, options).ConfigureAwait(false);
			}
		}

	}
}
