using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CcgWorks.Core;
using Amazon.SimpleDB;
using Model = Amazon.SimpleDB.Model;

namespace CcgWorks.SimpleDbStore
{
    public class GameStore : BaseStore<Game>, IGameStore
    {
        public GameStore(IAmazonSimpleDB simpleDB) : base(simpleDB) {}
        protected override string Domain => "Games";
        public async Task<IEnumerable<Game>> Get()
        {
            return await LoadItems($"select * from {Domain}");
        }

        protected override Game FromAttributes(string item, List<Amazon.SimpleDB.Model.Attribute> attributes) => throw new NotImplementedException();

        protected override List<Model.ReplaceableAttribute> ToAttributes(Game game, bool isUpdate) => throw new NotImplementedException();
    }
}