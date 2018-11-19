using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriggersTools.DiscordBots.Database.Attributes {
	/// <summary>
	/// States a property is ignored by the converter checks and is handled other ways.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class ConverterIgnoreAttribute : Attribute { }
}
