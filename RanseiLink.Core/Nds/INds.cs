using System;

namespace RanseiLink.Core.Nds
{
    public interface INds : IDisposable
    {
        void ExtractCopyOfDirectory(string path, string destinationFolder);
        void ExtractCopyOfFile(string path, string destinationFolder);
        void InsertFixedLengthFile(string path, string source);
    }
}