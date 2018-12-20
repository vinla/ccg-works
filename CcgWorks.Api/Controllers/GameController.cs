﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CcgWorks.Api.Payloads;
using CcgWorks.Core;
using GorgleDevs.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CcgWorks.Api.Controllers
{
    [Route("/api/games")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameStore _gameStore;
        private readonly IImageStore _imageStore;
        private readonly IUserContext _userContext;

        public GameController(IGameStore gameStore, IImageStore imageStore, IUserContext userContext)
        {
            _gameStore = gameStore ?? throw new ArgumentNullException(nameof(gameStore));
            _imageStore = imageStore ?? throw new ArgumentNullException(nameof(imageStore));
            _userContext = userContext;            
        }

        [HttpGet("{gameId}")]
        public async Task<IActionResult> Get(Guid gameId)
        {
            var game = await _gameStore.GetSingle(gameId);
            if (game == null)
                return NotFound();
            return new JsonResult(game);
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] GameSearchRequest searchRequest)
        {
            var gamesOut = (await _gameStore.Get()).Where(g => g.Name.ToLower().Contains(searchRequest.SearchText.ToLower()));
            
            var games = gamesOut.Select(g => new
			{
				id = g.Id.ToShortGuid(),
				ownerId = g.Owner.Id.ToShortGuid(),
				name = g.Name,
				userName = g.Owner.UserName,
				createdOn = g.CreatedOn.ToString("yyyy-MM-dd"),
				cardCount = g.CardCount,
				imageUrl = g.ImageUrl
			}).ToList();

			var pageIndex = searchRequest.Page - 1;
			var pageCount = games.Count() / searchRequest.ItemsPerPage;

			if (games.Count() % searchRequest.ItemsPerPage > 0)
				pageCount++;

			var result = new
			{
				games = games.Skip(pageIndex * searchRequest.ItemsPerPage).Take(searchRequest.ItemsPerPage),
				numberOfPages = pageCount
			};

			return new JsonResult(result);
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] string name, [FromForm] string description, IFormFile image)
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
                
            return Ok();
        }       
    }
}
