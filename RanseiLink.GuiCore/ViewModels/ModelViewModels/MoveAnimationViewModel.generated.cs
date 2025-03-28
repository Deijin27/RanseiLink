﻿// This file is automatically generated

#nullable enable

using RanseiLink.Core.Enums;
using RanseiLink.Core.Graphics;
using RanseiLink.Core.Models;
using RanseiLink.Core.Services;
using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;

public partial class MoveAnimationViewModel : ViewModelBase
{
    private MoveAnimation _model = new();
    private MoveAnimationId _id;
    public int Id => (int)_id;


    public TrueMoveAnimationId Animation
    {
        get => _model.Animation;
        set => SetProperty(_model.Animation, value, v => _model.Animation = v);
    }

    public int Sound_Max => 255;
    public int Sound
    {
        get => _model.Sound;
        set => SetProperty(_model.Sound, value, v => _model.Sound = v);
    }
}