using RanseiLink.Core.Nds;
using System.Collections.Generic;

namespace RanseiLink.Tests.Mocks
{
    internal class MockNds : INds
    {
        public int DisposeCallCount = 0;
        public void Dispose()
        {
            DisposeCallCount++;
        }

        public Queue<(string path, string destinationFolder)> ExtractDirectoryCalls = new Queue<(string path, string source)>();
        public void ExtractCopyOfDirectory(string path, string destinationFolder)
        {
            ExtractDirectoryCalls.Enqueue((path, destinationFolder));
        }

        public Queue<(string path, string destinationFolder)> ExtractFileCalls = new Queue<(string path, string source)>();
        public void ExtractCopyOfFile(string path, string destinationFolder)
        {
            ExtractFileCalls.Enqueue((path, destinationFolder));
        }

        public Queue<(string path, string source)> InsertCalls = new Queue<(string path, string source)>();
        public void InsertFixedLengthFile(string path, string source)
        {
            InsertCalls.Enqueue((path, source));
        }
    }
}
