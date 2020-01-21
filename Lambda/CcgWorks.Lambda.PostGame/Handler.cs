using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using CcgWorks.Core;
using Newtonsoft.Json;
using SixLabors.ImageSharp;

namespace CcgWorks.Lambda.PostGame
{
    public class Handler : IRequestHandler
    {
        private readonly IGameStore _gameStore;
        private readonly IImageStore _imageStore;
        private readonly IUserContext _userContext;

        public Handler(IUserContext userContext, IGameStore gameStore, IImageStore imageStore)
        {
            _gameStore = gameStore;
            _imageStore = imageStore;
            _userContext = userContext;
        }

        public async Task<APIGatewayProxyResponse> HandleRequest(APIGatewayProxyRequest proxyRequest, ILambdaContext lambdaContext)
        {
            var request = JsonConvert.DeserializeObject<Request>(proxyRequest.Body);
            return await ResponseCreator
                .From(() => CreateGame(request.Name, request.Description, Convert.FromBase64String(request.ImageData)))
                .GetResponseAsync();
        }

        private async Task CreateGame(string name, string description, byte[] imageData)
        {
            Game newGame = new Game
			{
				Id = Guid.NewGuid(),
				Name = name,
                Description = description,
				CreatedOn = DateTime.Now,
				Owner = _userContext.SignedInUser,
				CardCount = 0
            };

            if (imageData.Length > 0)
            {
                var imageFormat = Image.DetectFormat(imageData);
                newGame.ImageUrl = await _imageStore.Add("Game", newGame.Id, 1, imageFormat.FileExtensions.FirstOrDefault(), imageData);                
            }

            await _gameStore.Add(newGame);
        }
    }

    internal class Request
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public String ImageData { get; set; }
    }
}
