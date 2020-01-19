using System;
using System.Collections.Generic;
using CcgWorks.Core;
using Model = Amazon.SimpleDB.Model;

namespace CcgWorks.SimpleDbStore
{
    public static class GameConverter
    {
        public static Game AttributesToGame(string itemName, IEnumerable<Model.Attribute> attributes)
        {
            var game = new Game
            {
                Id = Guid.Parse(itemName),
            };

            foreach (var attr in attributes)
            {
                switch(attr.Name)
                {
                    case nameof(game.CardCount):
                        game.CardCount = int.Parse(attr.Value);
                        break;
                    case nameof(game.CardSize):
                        var parts = attr.Value.Split(':');
                        game.CardSize = new JsonSize
                        {
                            Height = int.Parse(parts[1]),
                            Width = int.Parse(parts[0])
                        };
                        break;
                    case nameof(game.CreatedOn):
                        game.CreatedOn = DateTime.ParseExact(attr.Value, "yyyy-mm-dd", null);
                        break;
                    case nameof(game.Description):
                        game.Description = attr.Value;
                        break;
                    case nameof(game.ImageUrl):
                        game.ImageUrl = attr.Value;
                        break;
                    case nameof(game.Name):
                        game.Name = attr.Value;
                        break;
                    case nameof(game.Owner):
                        game.Owner = Newtonsoft.Json.JsonConvert.DeserializeObject<Member>(attr.Value);
                        break;
                }   
            }

            return game;
        }

        public static List<Model.ReplaceableAttribute> GameToAttributes(Game game, bool isUpdate)
        {
            return new List<Model.ReplaceableAttribute>
            {
                new Model.ReplaceableAttribute
                {
                    Name = nameof(game.Name),
                    Value = game.Name,
                    Replace = isUpdate
                },
                new Model.ReplaceableAttribute
                {
                    Name = nameof(game.CardCount),
                    Value = game.CardCount.ToString(),
                    Replace = isUpdate
                },
                new Model.ReplaceableAttribute
                {
                    Name = nameof(game.CreatedOn),
                    Value = game.CreatedOn.ToString("yyyy-mm-dd"),
                    Replace = isUpdate
                },
                new Model.ReplaceableAttribute
                {
                    Name = nameof(game.Description),
                    Value = game.Description,
                    Replace = isUpdate
                },
                new Model.ReplaceableAttribute
                {
                    Name = nameof(game.ImageUrl),
                    Value = game.ImageUrl,
                    Replace = isUpdate
                },
                new Model.ReplaceableAttribute
                {
                    Name = nameof(game.Owner),
                    Value = Newtonsoft.Json.JsonConvert.SerializeObject(game.Owner),
                    Replace = isUpdate
                },
                new Model.ReplaceableAttribute
                {
                    Name = "OwnerId",
                    Value = game.Owner.Id.ToString(),
                    Replace = isUpdate
                }
            };
        }
    }
}