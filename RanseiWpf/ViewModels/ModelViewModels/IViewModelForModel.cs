using System;
using System.Collections.Generic;
using System.Text;

namespace RanseiWpf.ViewModels
{
    public interface IViewModelForModel<TModel>
    {
        TModel Model { get; set; }
    }
}
