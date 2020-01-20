using System;
using System.Threading.Tasks;
using System.Linq;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CcgWorks.Core;
using CcgWorks.SimpleDbStore;
using System.IO;
using Newtonsoft.Json;

namespace CcgWorks.Lambda.UploadCards
{
    public class EntryPoint : APIGatewayEntryPoint<Handler, Configuration>
    {
        public EntryPoint() : base(new ConfigurationBuilder())
        {
        }

        public EntryPoint(IConfigurationBuilder configurationBuilder) : base(configurationBuilder)
        {
        }
    }

    public class Configuration : IConfigureServices
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var awsOptions = configuration.GetAWSOptions();            
            services.AddSimpleDbStore(awsOptions);
            services.AddS3ImageStore(awsOptions, configuration.GetValue<String>(""), configuration.GetValue<String>(""));
        }
    }

    public class Handler : IRequestHandler
    {
        private readonly IGameStore _gameStore;
        private readonly IImageStore _imageStore;
        private readonly ICardStore _cardStore;

        public Task<APIGatewayProxyResponse> HandleRequest(APIGatewayProxyRequest proxyRequest, ILambdaContext lambdaContext)
        {
            var gameId = Guid.Parse(proxyRequest.PathParameters["gameId"]);
            var request = JsonConvert.DeserializeObject<Request>(proxyRequest.Body);            
            return ResponseCreator.From(() => UploadCards(gameId, request) ).GetResponseAsync();
        }

        private async Task UploadCards(Guid gameId, Request request)
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

    internal class Request
    {
        public int CardsPerRow { get; set; }
        public int CardCount { get; set; }
        public string ImageData { get; set; }
    }
}
