using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CcgWorks.Core
{
	public interface IDeckStore
	{
		Task Save(Deck deck);
		Task<Deck> Get(Guid deckId);
		Task<IEnumerable<Deck>> ByGameAndOwner(Guid gameId, Guid ownerId);

		Task Delete(Guid deckId);
	}
}