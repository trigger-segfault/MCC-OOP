using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TriggersTools.DiscordBots.Database {
	public class EndUserDataContents : HashSet<string> {

		public static EndUserDataContents All => new EndUserDataContents();

		public EndUserDataContents() : base(StringComparer.InvariantCultureIgnoreCase) { }

		public EndUserDataContents(IEnumerable<string> collection) : base(collection, StringComparer.InvariantCultureIgnoreCase) { }

		public EndUserDataContents(IEnumerable<EndUserDataInfo> euds)
			: base(euds.Select(e => e.Key), StringComparer.InvariantCultureIgnoreCase)
		{
		}

		public bool EraseAll => Count == 0;
	}
}
