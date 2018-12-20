using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CcgWorks.Core;
using Marten;

namespace CcgWorks.MartenStore
{
    public class GameStore : BaseStore<Game>, IGameStore
    {
        public GameStore(IDocumentStore documentStore) : base(documentStore)
        {            
            
        }        
    }
}
