﻿using RanseiLink.Core.Enums;
using RanseiLink.Core.Services;
using System.Windows;
using System.Windows.Controls;

namespace RanseiLink.Windows.Controls;

/// <summary>
/// Interaction logic for ModListItemView.xaml
/// </summary>
public partial class ModInfoControl : UserControl
{
    public ModInfoControl()
    {
        InitializeComponent();
    }

    public static DependencyProperty ModInfoProperty = UserControlUtil.RegisterDependencyProperty<ModInfoControl, ModInfo>(
        v => v.ModInfo, new ModInfo(), OnModInfoPropertyChanged);

    public ModInfo ModInfo
    {
        get => (ModInfo)GetValue(ModInfoProperty);
        set => SetValue(ModInfoProperty, value);
    }

    private static void OnModInfoPropertyChanged(ModInfoControl target, DependencyPropertyChangedEventArgs<ModInfo> e)
    {
        var mi = e.NewValue;
        if (e.NewValue != null)
        {
            target.NameTextBlock.Text = mi.Name;

            var bottomTextItems = new List<string>();
            if (!string.IsNullOrEmpty(mi.Version))
            {
                bottomTextItems.Add($"v{mi.Version}");
            }
            if (!string.IsNullOrEmpty(mi.Author))
            {
                bottomTextItems.Add(mi.Author);
            }
            bottomTextItems.Add(mi.GameCode.UserFriendlyName());

            var bottomText = string.Join(" | ", bottomTextItems);

            foreach (var tag in mi.Tags)
            {
                bottomText += " • " + tag;
            }

            target.BottomTextBlock.Text = bottomText;
        }
    }
}
