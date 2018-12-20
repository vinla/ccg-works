﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CcgWorks.Core
{
	public class Game : BaseObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Member Owner { get; set; }
        public DateTime CreatedOn { get; set; }
        public JsonSize CardSize {get; set;}
		public int CardCount { get; set; }		
        public string ImageUrl { get; set; }
    }
}
