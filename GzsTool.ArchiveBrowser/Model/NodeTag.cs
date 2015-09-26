using System.Collections.Generic;
using GzsTool.Common.Interfaces;

namespace GzsTool.ArchiveBrowser.Model
{
    public class NodeTag
    {
        public IFileSystemEntry Entry { get; set; }
        public List<IFileSystemEntry> References { get; set; }

        public NodeTag()
        {
            References = new List<IFileSystemEntry>();
        }
    }
}