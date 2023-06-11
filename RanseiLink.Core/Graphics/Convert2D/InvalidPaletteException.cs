using System;

namespace RanseiLink.Core.Graphics;

public class InvalidPaletteException : Exception
{
    public InvalidPaletteException(string message) : base(message) { }
}
