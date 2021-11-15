using System;
using System.Collections.Generic;
using System.Text;

namespace RanseiLink.Core.Models;

public interface ICloneable<T>
{
    T Clone();
}
