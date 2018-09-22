using System;
using System.Collections.Generic;
using System.Text;

namespace EnigmaBot.Info {
	[AttributeUsage(AttributeTargets.Method)]
	public class IsDuplicateAttribute : Attribute {

		public bool DuplicateFunctionality { get; }

		public IsDuplicateAttribute(bool duplicateFunctionality) {
			DuplicateFunctionality = duplicateFunctionality;
		}
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class ExampleAttribute : Attribute {
		public string Example { get; }

		public ExampleAttribute(string example) {
			Example = example;
		}
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class ParametersAttribute : Attribute {
		public string Parameters { get; }

		public ParametersAttribute(string parameters) {
			Parameters = parameters;
		}
	}
}
