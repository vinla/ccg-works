namespace CcgWorks.Lambda.UploadCards
{
    public class Payloads
    {
        public class Request
        {
            public int CardsPerRow { get; set; }
            public int CardCount { get; set; }
            public string ImageData { get; set; }
        }
    }
}