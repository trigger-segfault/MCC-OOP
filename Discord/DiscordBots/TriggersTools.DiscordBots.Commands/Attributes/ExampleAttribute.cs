using System;

namespace TriggersTools.DiscordBots.Commands {
	/// <summary>
	/// A struct representing a command example.
	/// </summary>
	public struct Example {
		/// <summary>
		/// Gets the example usage of the command parameters.
		/// </summary>
		public string Execution { get; }
		/// <summary>
		/// Gets an explanation of the example.
		/// </summary>
		public string Explanation { get; }

		/// <summary>
		/// Constructs the <see cref="Example"/>.
		/// </summary>
		/// <param name="execution">The example usage of the command parameters.</param>
		/// <param name="explanation">An explanation of the example.</param>
		public Example(string execution, string explanation = null) {
			Execution = execution ?? string.Empty;
			Explanation = explanation ?? throw new ArgumentNullException(nameof(explanation));
		}

		public override string ToString() {
			return $"{(Execution ?? "No parameters")} - {(Explanation ?? "No explanation")}";
		}

		/// <summary>
		/// Combines the command example with the command's alias to make a fully formed example.
		/// </summary>
		/// <param name="alias">The command alias.</param>
		/// <returns>The example combined with the alias.</returns>
		public Example AddAlias(string alias) {
			if (!string.IsNullOrWhiteSpace(Execution))
				return new Example($"{alias} {Execution}", Explanation);
			return new Example(alias, Explanation);
		}
	}

	/// <summary>
	/// An attribute showing an example usage of the command parameters.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class ExampleAttribute : Attribute {

		#region Properties

		/// <summary>
		/// Gets the example usage of the command parameters. 
		/// </summary>
		public string Execution { get; }
		/// <summary>
		/// Gets an explanation of the example.
		/// </summary>
		public string Explanation { get; }
		/// <summary>
		/// Gets the <see cref="Example"/> struct.
		/// </summary>
		public Example Example => new Example(Execution, Explanation);

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the <see cref="ExampleAttribute"/> with empty execution parameters the specifiedand
		/// example explanation.
		/// </summary>
		/// <param name="execution">
		/// The example usage of the command parameters. Use null when there are no parameters.
		/// </param>
		/// <param name="explanation">An explanation of the example.</param>
		public ExampleAttribute(string explanation) : this(string.Empty, explanation) { }
		/// <summary>
		/// Constructs the <see cref="ExampleAttribute"/> with the specified example execution parameters and
		/// explanation.
		/// </summary>
		/// <param name="execution">
		/// The example usage of the command parameters. Use null when there are no parameters.
		/// </param>
		/// <param name="explanation">An explanation of the example.</param>
		public ExampleAttribute(string execution, string explanation) {
			Execution = execution ?? string.Empty;
			Explanation = explanation ?? throw new ArgumentNullException(nameof(explanation));
		}

		#endregion
	}
}
