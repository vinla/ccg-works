﻿using System;

namespace CcgWorks.Core
{
    public class ImageData
    {
        public string Id { get; set; }
        public string OriginalFileName { get; set; }
        public byte[] Data { get; set; }

        public int Version { get; set; }
    }
}
