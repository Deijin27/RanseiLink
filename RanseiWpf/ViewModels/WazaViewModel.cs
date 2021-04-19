using System;
using System.Collections.Generic;
using System.Text;
using Core.Models;

namespace RanseiWpf.ViewModels
{
    public class WazaViewModel : ViewModelBase, IViewModelForModel<Move>
    {
        public Move Model { get; set; }
    }
}
