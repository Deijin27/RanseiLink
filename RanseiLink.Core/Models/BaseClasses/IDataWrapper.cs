

namespace RanseiLink.Core.Models;

public interface IDataWrapper
{
    byte[] Data { get; }

    string Serialize();
    bool TryDeserialize(string serialized);
}