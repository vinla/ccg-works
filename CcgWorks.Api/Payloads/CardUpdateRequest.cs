using System;
using GorgleDevs.Mvc;

namespace CcgWorks.Api.Payloads
{
    public class CardUpdateRequest
    {					
		public string Name { get; set; }
		public string Type { get; set; }		
    }
}