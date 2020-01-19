using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CcgWorks.Core;
using Amazon.SimpleDB;
using Model = Amazon.SimpleDB.Model;

namespace CcgWorks.SimpleDbStore
{
    public class CardStore : BaseStore<Card>, ICardStore
    {
        public CardStore(IAmazonSimpleDB simpleDBClient) : base(simpleDBClient)
        {
        }

        protected override string Domain => "Cards";

        public async Task<IEnumerable<Card>> Get(Guid gameId)
        {
            var cards = new List<Card>();
            var query = $"select * from {Domain} where GameId = '{gameId}'";
            return await LoadItems(query);            
        }

        protected override List<Model.ReplaceableAttribute> ToAttributes(Card card, bool isUpdate) =>
            CardConverter.CardToAttributes(card, isUpdate);
        

        protected override Card FromAttributes(string item, List<Model.Attribute> attributes) => 
            CardConverter.AttributesToCard(item, attributes);
    }
}
