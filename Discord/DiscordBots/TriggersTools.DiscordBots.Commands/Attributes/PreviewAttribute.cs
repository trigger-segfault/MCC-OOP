using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// An attribute to add an image preview to a command to dispaly in the help menu
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
	public class PreviewAttribute : Attribute {
		/// <summary>
		/// Gets the Url of the image to dispaly in the help menu.
		/// </summary>
		public string ImageUrl { get; }

		/// <summary>
		/// Constructs the <see cref="PreviewAttribute"/> to display the specified image.
		/// </summary>
		/// <param name="imageUrl">The url of the image.</param>
		public PreviewAttribute(string imageUrl) {
			ImageUrl = imageUrl ?? throw new ArgumentNullException(nameof(imageUrl));
		}
	}
}
