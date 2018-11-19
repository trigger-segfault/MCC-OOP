using System;
using System.IO;
using System.Xml;

namespace WJLCS.Enigma.IO {
	/// <summary>
	/// A class for reading and writing enciphered HTML messages in the format layed out by the specifications.
	/// </summary>
	public class HtmlIO {

		#region Constants
		
		/// <summary>
		/// The number of characters to consume before starting a new row.
		/// </summary>
		public const int CharactersPerRow = 50;

		#endregion

		#region Read/Write

		/// <summary>
		/// Reads the enciphered message from the HTML file path.
		/// </summary>
		/// <param name="htmlFile">The path of an enciphered message HTML file.</param>
		/// <returns>The read enciphered message.</returns>
		/// 
		/// <exception cref="FileNotFoundException">
		/// The HTML file was not found.
		/// </exception>
		/// <exception cref="LoadFailedException">
		/// An error occurred while loading the file.
		/// </exception>
		public static string Read(string htmlFile) {
			try {
				XmlDocument doc = new XmlDocument();
				doc.Load(htmlFile);
				return ReadInternal(doc);
			}
			catch (FileNotFoundException) {
				throw;
			}
			catch (Exception ex) {
				throw new LoadFailedException($"Failed to load the HTML file!\n{ex.Message}");
			}
		}
		/// <summary>
		/// Reads the enciphered message from the HTML file path.
		/// </summary>
		/// <param name="html">The path of an enciphered message HTML file.</param>
		/// <returns>The read enciphered message.</returns>
		/// 
		/// <exception cref="LoadFailedException">
		/// An error occurred while loading the text.
		/// </exception>
		public static string ReadText(string html) {
			try {
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(html);
				return ReadInternal(doc);
			}
			catch (Exception ex) {
				throw new LoadFailedException($"Failed to load the HTML text!\n{ex.Message}");
			}
		}
		/// <summary>
		/// Writes the enciphered message to the HTML file.
		/// </summary>
		/// <param name="text">The enciphered message.</param>
		/// <param name="path">The path of the enciphered message HTML file to write.</param>
		/// 
		/// <exception cref="SaveFailedException">
		/// An error occurred while saving the file.
		/// </exception>
		public static void Write(string text, string htmlFile, string htmlTemplateFile) {
			try {
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(File.ReadAllText(htmlTemplateFile));
				WriteInternal(doc, text);
				doc.Save(htmlFile);
			}
			catch (Exception ex) {
				throw new SaveFailedException($"Failed to save the HTML file!\n{ex.Message}");
			}
		}
		/// <summary>
		/// Writes the enciphered message to the HTML file.
		/// </summary>
		/// <param name="text">The enciphered message.</param>
		/// <param name="path">The path of the enciphered message HTML file to write.</param>
		/// 
		/// <exception cref="SaveFailedException">
		/// An error occurred while saving the text.
		/// </exception>
		public static string WriteText(string text, string htmlTemplateFile) {
			try {
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(File.ReadAllText(htmlTemplateFile));
				WriteInternal(doc, text);
				return doc.OuterXml;
			}
			catch (Exception ex) {
				throw new SaveFailedException($"Failed to save the HTML text!\n{ex.Message}");
			}
		}

		#endregion

		#region Private File IO

		/// <summary>
		/// Reads the enciphered message from the XML Document after it has been loaded from HTML.
		/// </summary>
		/// <param name="doc">The loaded XML Document to read from.</param>
		/// <returns>The enciphered message.</returns>
		private static string ReadInternal(XmlDocument doc) {
			var tds = doc.SelectNodes("//html/body/table/tbody/tr/td");
			string text = string.Empty;
			for (int i = 0; i < tds.Count; i++) {
				if (!(tds[i] is XmlElement td)) continue;
				text += td.InnerText;
			}
			return text;
		}
		/// <summary>
		/// Write to the XML Document after the HTML Template has been loaded.
		/// </summary>
		/// <param name="doc">The template-loaded XML Document to write to.</param>
		/// <param name="text">The enciphered message.</param>
		private static void WriteInternal(XmlDocument doc, string text) {
			var tbody = doc.SelectSingleNode("//html/body/table/tbody") as XmlElement;
			for (int i = 0; i < text.Length;) {
				int length = Math.Min(text.Length - i, CharactersPerRow);
				string innerText = text.Substring(i, length);
				var tr = doc.CreateElement("tr");
				var td = doc.CreateElement("td");
				tbody.AppendChild(tr);
				tr.AppendChild(td);
				td.InnerText = innerText;
				i += length;
			}
		}

		#endregion
	}
}
