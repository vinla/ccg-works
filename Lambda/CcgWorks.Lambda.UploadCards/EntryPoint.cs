using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CcgWorks.SimpleDbStore;

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

    internal class Request
    {
        public int CardsPerRow { get; set; }
        public int CardCount { get; set; }
        public string ImageData { get; set; }
    }
}
