using System;
using System.Linq;
using System.Collections.Generic;
using CcgWorks.Core;
using Model = Amazon.SimpleDB.Model;

namespace CcgWorks.SimpleDbStore
{
    public static class CardConverter
    {
        public static Card AttributesToCard(string itemName, IEnumerable<Model.Attribute> attributes)
        {
            var card = new Card
            {
                Id = Guid.Parse(itemName),
            };

            foreach (var attr in attributes)
            {
                switch(attr.Name)
                {
                    case nameof(card.CreatedOn):
                        card.CreatedOn = DateTime.ParseExact(attr.Value, "yyyy-mm-dd", null);                        
                        break; 
                    case nameof(card.GameId):
                        card.GameId = Guid.Parse(attr.Value);
                        break;
                    case nameof(card.ImageUrl):
                        card.ImageUrl = attr.Value;
                        break;
                    case nameof(card.Name):
                        card.Name = attr.Value;
                        break;
                    case nameof(card.Tags):
                        card.Tags = attr.Value.Split(',');
                        break;
                    case nameof(card.Type):
                        card.Type = attr.Value;
                        break;
                }   
            }

            return card;
        }

        public static List<Model.ReplaceableAttribute> CardToAttributes(Card card, bool isUpdate)
        {
            return new List<Model.ReplaceableAttribute>
            {
                new Model.ReplaceableAttribute
                {
                    Name = "Name",
                    Value = card.Name,
                    Replace = isUpdate
                },
                new Model.ReplaceableAttribute
                {
                    Name = "GameId",
                    Value = card.GameId.ToString(),
                    Replace = isUpdate
                },
                new Model.ReplaceableAttribute
                {
                    Name = "ImageUrl",
                    Value = card.ImageUrl,
                    Replace = isUpdate
                },
                new Model.ReplaceableAttribute
                {
                    Name = "Type",
                    Value = card.Type,
                    Replace = isUpdate
                },
                new Model.ReplaceableAttribute
                {
                    Name = "Tags",
                    Value = String.Join(",", card.Tags),
                    Replace = isUpdate
                }
            };
        }
    }
}