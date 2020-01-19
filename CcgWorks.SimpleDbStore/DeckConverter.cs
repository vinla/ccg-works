using System;
using System.Collections.Generic;
using System.Linq;
using CcgWorks.Core;
using Model = Amazon.SimpleDB.Model;

namespace CcgWorks.SimpleDbStore
{
    public static class DeckConverter
    {
        public static Deck AttributesToDeck(string itemName, IEnumerable<Model.Attribute> attributes)
        {
            var deck = new Deck
            {
                Id = Guid.Parse(itemName),
            };

            foreach (var attr in attributes)
            {
                switch(attr.Name)
                {
                    case nameof(deck.GameId):
                        deck.GameId = Guid.Parse(attr.Value);
                        break;
                    case nameof(deck.Name):
                        deck.Name = attr.Value;
                        break;
                    case nameof(deck.Items):
                        deck.Items = AttributeValueToDeckItems(attr.Value);
                        break;
                    case nameof(deck.Version):
                        deck.Version = int.Parse(attr.Value);
                        break;
                    case nameof(deck.Owner):
                        deck.Owner = Newtonsoft.Json.JsonConvert.DeserializeObject<Member>(attr.Value);
                        break;
                }   
            }

            return deck;
        }

        public static List<Model.ReplaceableAttribute> DeckToAttributes(Deck deck, bool isUpdate)
        {
            return new List<Model.ReplaceableAttribute>
            {
                new Model.ReplaceableAttribute
                {
                    Name = nameof(deck.Name),
                    Value = deck.Name,
                    Replace = isUpdate
                },
                new Model.ReplaceableAttribute
                {
                    Name = nameof(deck.GameId),
                    Value = deck.GameId.ToString(),
                    Replace = isUpdate
                },
                new Model.ReplaceableAttribute
                {
                    Name = nameof(deck.Version),
                    Value = deck.Version.ToString(),
                    Replace = isUpdate
                },
                new Model.ReplaceableAttribute
                {
                    Name = nameof(deck.Items),
                    Value = DeckItemsToAttributeValue(deck.Items),
                    Replace = isUpdate
                },
                new Model.ReplaceableAttribute
                {
                    Name = nameof(deck.Owner),
                    Value = Newtonsoft.Json.JsonConvert.SerializeObject(deck.Owner)
                },
                new Model.ReplaceableAttribute
                {
                    Name = "OwnerId",
                    Value = deck.Owner.Id.ToString()
                }
            };
        }

        private static string DeckItemsToAttributeValue(IEnumerable<DeckItem> items)
        {
            return String.Join(",", items.Select(i => $"{i.CardId}:{i.Amount}" ));
        }

        private static DeckItem[] AttributeValueToDeckItems(string attributeValue)
        {
            return attributeValue.Split(',').Select(v => {
                var parts = v.Split(':');
                return new DeckItem
                {
                    CardId = Guid.Parse(parts[0]),
                    Amount = int.Parse(parts[1])
                };
            }).ToArray();
        }
    }
}