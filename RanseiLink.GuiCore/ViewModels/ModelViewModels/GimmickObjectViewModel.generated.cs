﻿// This file is automatically generated

#nullable enable

using RanseiLink.Core.Enums;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;

public partial class GimmickObjectViewModel : ViewModelBase
{
    private GimmickObject _model = new();
    private GimmickObjectId _id;
    public int Id => (int)_id;

}