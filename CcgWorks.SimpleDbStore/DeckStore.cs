using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CcgWorks.Core;
using Amazon.SimpleDB;
using Model = Amazon.SimpleDB.Model;

namespace CcgWorks.SimpleDbStore
{
    public class DeckStore : BaseStore<Deck>, IDeckStore
    {
        public DeckStore(IAmazonSimpleDB simpleDB) : base(simpleDB) {}
        protected override string Domain => "Decks";

        public async Task<IEnumerable<Deck>> ByGameAndOwner(Guid gameId, Guid ownerId)
        {
            return await LoadItems($"select * from {Domain} where GameId = '{gameId}' and OwnerId = '{ownerId}'");
        }

        public async Task<Deck> Get(Guid deckId)
        {
            return await GetSingle(deckId);
        }

        public async Task Save(Deck deck)
        {
            await Add(deck);
        }

        protected override Deck FromAttributes(string item, List<Amazon.SimpleDB.Model.Attribute> attributes) => DeckConverter.AttributesToDeck(item, attributes);

        protected override List<Model.ReplaceableAttribute> ToAttributes(Deck deck, bool isUpdate) => DeckConverter.DeckToAttributes(deck, isUpdate);
    }
}