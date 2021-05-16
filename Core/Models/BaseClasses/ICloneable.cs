using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public interface ICloneable<T>
    {
        T Clone();
    }
}
