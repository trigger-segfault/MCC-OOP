using System;
using System.Collections.Generic;
using System.Text;

namespace TriggersTools.DiscordBots.Database.Model.Special {
	public interface IInclusionSet<T> : IEnumerable<T> {
		
		int Count { get; }

		bool Add(T item, Inclusion inclusion);

		bool Remove(T item, Inclusion inclusion);

		bool Contains(T item, Inclusion inclusion);

	}
}
