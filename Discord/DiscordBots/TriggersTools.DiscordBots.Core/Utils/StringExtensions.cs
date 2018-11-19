using System.Collections.Generic;
using System.Linq;

namespace TriggersTools.DiscordBots.Utils {

	public static class StringExtensions {


		public static bool IsSurroundedWith(this string str, string left, string right) {
			return str.StartsWith(left) && str.EndsWith(right);
		}

		public static string RemoveSurrounding(this string str, string left, string right) {
			if (IsSurroundedWith(str, left, right)) {
				return str.Substring(left.Length, str.Length - left.Length - right.Length);
			}
			return str;
		}
		
		public static int IndexOfUnescaped(this string str, IEnumerable<CodeBlock> codeBlocks,
			char value)
		{
			return IndexOfUnescaped(str, codeBlocks, value.ToString(), 0);
		}
		
		public static int IndexOfUnescaped(this string str, IEnumerable<CodeBlock> codeBlocks,
			char value, int startIndex)
		{
			return IndexOfUnescaped(str, codeBlocks, value.ToString(), startIndex);
		}
		
		public static int IndexOfUnescaped(this string str, IEnumerable<CodeBlock> codeBlocks,
			string value)
		{
			return IndexOfUnescaped(str, codeBlocks, value, 0);
		}

		public static int IndexOfUnescaped(this string str, IEnumerable<CodeBlock> codeBlocks,
			string value, int startIndex)
		{
			List<CodeBlock> blocks = codeBlocks as List<CodeBlock> ?? codeBlocks.ToList();

			bool escaped;
			int codeBlockIndex = 0;
			int index = startIndex - 1;

			do {
				if ((index = str.IndexOf(value, index + 1)) == -1)
					break;

				while (codeBlockIndex < blocks.Count &&
					index >= blocks[codeBlockIndex].End)
				{
					codeBlockIndex++;
				}

				if (codeBlockIndex < blocks.Count &&
					blocks[codeBlockIndex].Contains(index)) {
					escaped = true;
					continue;
				}

				escaped = false;
				for (int i = index - 1; i >= 0; i--) {
					if (str[i] == '\\')
						escaped = !escaped;
					else
						break;
				}
			} while (escaped);
			return index;
		}

		public static bool FindEscapedBacktickCount(this string str, ref int index,
			out int backticks)
		{
			backticks = 0;
			if (index < str.Length) {
				// Find the first unescapped backtick
				while ((index = str.IndexOf('`', index)) != -1 &&
					str.IsEscaped(index))
					;

				if (index != -1) {
					backticks++;
					for (int i = index + 1; i < str.Length && str[i] == '`';
						i++, backticks++)
						;
					return true;
				}
			}
			index = -1;
			return false;
		}

		public static bool FindBacktickCount(this string str, ref int index,
			out int backticks)
		{
			backticks = 0;
			if (index < str.Length && (index = str.IndexOf('`', index)) != -1) {
				backticks++;
				for (int i = index + 1; i < str.Length && str[i] == '`';
					i++, backticks++);
				return true;
			}
			index = -1;
			return false;
		}

		public static List<CodeBlock> GetAllCodeBlocks(this string str, int index = -1) {
			if (index == -1)
				index = str.Length;

			List<CodeBlock> blocks = new List<CodeBlock>();

#pragma warning disable IDE0018
			int backticks;
#pragma warning restore IDE0018
			int endBackticks;
			int start = 0;
			int end;

			while (start < index &&
				FindEscapedBacktickCount(str, ref start, out backticks))
			{
				// We skipped passed the desired index (start must be formatting)
				if (start > index)
					break;

				// SPECIAL: Full (containing one `)
				if (backticks == 7) {
					if (index <= start + 4) {
						blocks.Add(new CodeBlock() {
							Type = CodeBlockType.Full,
							Start = start + 3,
							Length = 1,
						});
						break;
					}
					start += backticks;
					continue;
				}
				else {
					end = start + backticks;

					if (!FindBacktickCount(str, ref end, out endBackticks)) {
						// RETURN: Never closed
						break;
					}
				}

				// Normal backtick checks
				if (backticks <= 2) {
					// Is this a valid Quote?
					if (endBackticks <= backticks) {
						if (index < end) {
							blocks.Add(new CodeBlock() {
								Type = CodeBlockType.Quote,
								Start = start + endBackticks,
								End = end,
							});
							break;
						}
						// We haven't reached index yet
					}
					else {
						// Inlines cannot be ended with a higher
						// backtick count than the start count.

						// Reuse the found backticks
						endBackticks = 0;
					}
				}
				else { // backticks >= 3 && backticks != 7
					if (endBackticks <= 2) {
						// We may be ending an inline
						int fullEnd = str.IndexOf("```", end + endBackticks);
						if (fullEnd != -1) {
							// It's a Full, are we inside?
							if (index < fullEnd) {
								blocks.Add(new CodeBlock() {
									Type = CodeBlockType.Quote,
									Start = start + 3,
									End = fullEnd,
								});
							}
							// We haven't reached index yet
							end = fullEnd;
							endBackticks = 3;
						}
						else {
							// It's an Quote, are we inside?
							if (index < end) {
								blocks.Add(new CodeBlock() {
									Type = CodeBlockType.Quote,
									Start = start + endBackticks,
									End = end,
								});
							}
							// We haven't reached index yet
						}
					}
					else {
						// It's a Full, are we inside?
						if (index < end) {
							blocks.Add(new CodeBlock() {
								Type = CodeBlockType.Quote,
								Start = start + 3,
								End = end,
							});
							endBackticks = 3;
						}
						// We haven't reached index yet
					}
				}

				start = end + endBackticks;
			}

			return blocks;
		}

		public static bool IsInsideCodeBlock(this IEnumerable<CodeBlock> blocks, int index) {
			foreach (CodeBlock block in blocks) {
				if (index < block.Start)
					return false;
				if (index >= block.Start && index < block.End)
					return true;
			}
			return false;
		}

		public static CodeBlock GetCodeBlockAt(this string str, int index) {
#pragma warning disable IDE0018
			int backticks;
#pragma warning restore IDE0018
			int endBackticks;
			int start = 0;
			int end;
			
			while (start < index && FindBacktickCount(str, ref start, out backticks)) {
				// We skipped passed the desired index (start must be formatting)
				if (start > index)
					break;

				// SPECIAL: Full (containing one `)
				if (backticks == 7) {
					if (index == start + 3) {
						// RETURN: Full
						return new CodeBlock() {
							Type = CodeBlockType.Full,
							Start = start + 3,
							Length = 1,
						};
					}
					else if (index >= start && index < start + backticks) {
						// RETURN: None (Inside Formatting)
						return CodeBlock.None;
					}
					start += backticks;
					continue;
				}
				else {
					end = start + backticks;

					if (!FindBacktickCount(str, ref end, out endBackticks)) {
						// RETURN: Never closed
						break;
					}
				}

				// Normal backtick checks
				if (backticks <= 2) {
					// Is this a valid Quote?
					if (endBackticks <= backticks) {
						if (index < end) {
							if (index >= start) {
								// RETURN: Quote
								return new CodeBlock() {
									Type = CodeBlockType.Quote,
									Start = start + endBackticks,
									End = end,
								};
							}
							// RETURN: None (Inside formatting)
							break;
						}
						// We haven't reached index yet
					}
					else {
						// Inlines cannot be ended with a higher
						// backtick count than the start count.

						// Reuse the found backticks
						endBackticks = 0;
					}
				}
				else { // backticks >= 3 && backticks != 7
					if (endBackticks <= 2) {
						int fullEnd = str.IndexOf("```", end + endBackticks);
						if (fullEnd != -1) {
							// It's a Full, are we inside?
							if (index < fullEnd + 3) {
								if (index > start && index < fullEnd) {
									// RETURN: Full
									return new CodeBlock() {
										Type = CodeBlockType.Quote,
										Start = start + 3,
										End = fullEnd,
									};
								}
								// RETURN: None (Inside formatting)
								break;
							}
							// We haven't reached index yet
						}
						else {
							// It's an Quote, are we inside?
							if (index < end) {
								if (index >= start) {
									// RETURN: Quote
									return new CodeBlock() {
										Type = CodeBlockType.Quote,
										Start = start + endBackticks,
										End = end,
									};
								}
								// RETURN: None (Inside formatting)
								break;
							}
							// We haven't reached index yet
						}
					}
				}

				start = end + endBackticks;
			}

			return CodeBlock.None;
		}

		public static bool IsEscaped(this string str, int index) {
			bool escaped = false;
			while (index > 0) {
				index--;
				if (str[index] == '\\')
					escaped = !escaped;
				else
					break;
			}
			return escaped;
		}

		public static bool IsEscaped(this string str, List<CodeBlock> blocks, int index) {
			bool escaped = false;
			while (index > 0) {
				index--;
				if (str[index] == '\\')
					escaped = !escaped;
				else
					break;
			}
			return escaped;
		}
	}
}
