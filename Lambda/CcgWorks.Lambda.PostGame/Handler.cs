using System;

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

        public Task<APIGatewayProxyResponse> HandleRequest(APIGatewayProxyRequest proxyRequest, ILambdaContext lambdaContext)
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

            if (image != null)
            {
                using (var ms = new MemoryStream())
                {
                    image.CopyTo(ms);                    
                    string imageType = Path.GetExtension(image.FileName).Replace(".", "");
                    newGame.ImageUrl = await _imageStore.Add("Game", newGame.Id, 1, imageType, ms.GetBuffer());
                }
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
