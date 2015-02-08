﻿using System;
using System.IO;

namespace GzsTool.Common
{
    public class FileDataStreamContainer
    {
        public string FileName { get; set; }
        public Func<Stream> DataStream { get; set; }
    }
}
