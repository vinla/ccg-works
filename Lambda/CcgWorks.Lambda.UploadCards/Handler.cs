using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using CcgWorks.Core;
using Newtonsoft.Json;

namespace CcgWorks.Lambda.UploadCards
{
     public class Handler : IRequestHandler
    {
        private readonly IGameStore _gameStore;
        private readonly IImageStore _imageStore;
        private readonly ICardStore _cardStore;

        public Handler(IGameStore gameStore, IImageStore imageStore, ICardStore cardStore)
        {
            _gameStore = gameStore;
            _imageStore = imageStore;
            _cardStore = cardStore;
        }

        public Task<APIGatewayProxyResponse> HandleRequest(APIGatewayProxyRequest proxyRequest, ILambdaContext lambdaContext)
        {
            var gameId = Guid.Parse(proxyRequest.PathParameters["gameId"]);
            var request = JsonConvert.DeserializeObject<Payloads.Request>(proxyRequest.Body);            
            return ResponseCreator.From(() => UploadCards(gameId, request) ).GetResponseAsync();
        }

        private async Task UploadCards(Guid gameId, Payloads.Request request)
        {
			var cards = new Card[] { };

            using (var imageSlicer = new ImageSlicer(request.CardsPerRow, request.CardCount, new MemoryStream(Convert.FromBase64String(request.ImageData))))
            {
                var game = ( await _gameStore.GetSingle(gameId));

                var index = 0;
				foreach (var imageData in imageSlicer.Slices)
                {                    
                 	var card = cards.Length > index ? cards[index] : new Card();
					card.Id = Guid.NewGuid();
					card.GameId = gameId;
					card.CreatedOn = DateTime.Now;					
                    card.ImageUrl = await _imageStore.Add("Card", card.Id, 1, "png", imageData);
                    await _cardStore.Add(card);                    
					index++;
                }

				await UpdateGameData(gameId);
            }
        }

		private async Task UpdateGameData(Guid gameId)
		{
			var allCards = await _cardStore.Get(gameId);
			await _gameStore.UpdateOne(gameId, g =>
			{
				g.CardCount = allCards.Count();
			});
		}
    }
}