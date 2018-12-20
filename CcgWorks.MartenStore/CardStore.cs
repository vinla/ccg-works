using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using CcgWorks.Core;
using Marten;

namespace CcgWorks.MartenStore
{
    public class CardStore : BaseStore<Card>, ICardStore
    {    
        public CardStore(IDocumentStore documentStore) : base(documentStore)
        {    
        }
        

        public async Task<IEnumerable<Card>> Get(Guid gameId)
        {
            return await Query(c => c.GameId == gameId);
        }        
    }
}