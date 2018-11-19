using System;
using System.Collections.Generic;
using System.Text;

namespace TriggersTools.DiscordBots.Database.Model {
	/// <summary>
	/// The base interface for all End User Data types.
	/// </summary>
	public interface IDbEndUserDataBase {
		/// <summary>
		/// Checks if the user has asked this information to be deleted.<para/>
		/// This method is only called once per table. The result should NOT change based on the data.
		/// </summary>
		/// <param name="euds">The info to that the user requested to be deleted.</param>
		/// <returns>True if the data should be deleted.</returns>
		bool ShouldKeep(EndUserDataContents euds, EndUserDataType type);
	}
}
