using RanseiLink.Core;
using RanseiLink.Core.Enums;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RanseiLink.Controls;

/// <summary>
/// Interaction logic for EvolutionConditionControl.xaml
/// </summary>
public partial class EvolutionConditionControl : UserControl
{
    private readonly KingdomId[] KingdomItems = EnumUtil.GetValuesExceptDefaults<KingdomId>().ToArray();
    private readonly GenderId[] GenderItems = EnumUtil.GetValues<GenderId>().ToArray();
    private readonly ItemId[] ItemItems = EnumUtil.GetValues<ItemId>().ToArray();
    public EvolutionConditionControl()
    {
        InitializeComponent();

        ConditionCombo.ItemsSource = EnumUtil.GetValues<EvolutionConditionId>().ToArray();
        KingdomComboBox.ItemsSource = KingdomItems;
        GenderComboBox.ItemsSource = GenderItems;
        ItemComboBox.ItemsSource = ItemItems;
    }

    public static DependencyProperty ConditionProperty = UserControlUtil.RegisterDependencyProperty<EvolutionConditionControl, EvolutionConditionId>(v => v.Condition, default, OnConditionPropertyChanged);

    public EvolutionConditionId Condition
    {
        get => (EvolutionConditionId)GetValue(ConditionProperty);
        set => SetValue(ConditionProperty, value);
    }

    private static void OnConditionPropertyChanged(EvolutionConditionControl target, DependencyPropertyChangedEventArgs<EvolutionConditionId> e)
    {
        target.ConditionCombo.SelectedItem = e.NewValue;
        target.QuantityNumberBox.Visibility = Visibility.Hidden;
        target.GenderComboBox.Visibility = Visibility.Hidden;
        target.KingdomComboBox.Visibility = Visibility.Hidden;
        target.ItemComboBox.Visibility = Visibility.Hidden;
        switch (e.NewValue)
        {
            case EvolutionConditionId.Hp:
            case EvolutionConditionId.Attack:
            case EvolutionConditionId.Defence:
            case EvolutionConditionId.Speed:
            case EvolutionConditionId.Link:
                target.QuantityNumberBox.Visibility = Visibility.Visible;
                break;
            case EvolutionConditionId.Kingdom:
                target.KingdomComboBox.Visibility = Visibility.Visible;
                break;
            case EvolutionConditionId.WarriorGender:
                target.GenderComboBox.Visibility = Visibility.Visible;
                break;
            case EvolutionConditionId.Item:
                target.ItemComboBox.Visibility = Visibility.Visible;
                break;
            case EvolutionConditionId.JoinOffer:
            case EvolutionConditionId.NoCondition:
                break;
            default:
                throw new Exception($"Invalid Enum Value of {e.NewValue} in {nameof(EvolutionConditionControl)} {nameof(OnQuantityPropertyChanged)}");
        }

        target.QuantityName.Text = e.NewValue switch
        {
            EvolutionConditionId.Hp => "Required HP",
            EvolutionConditionId.Attack => "Required Atk",
            EvolutionConditionId.Defence => "Required Def",
            EvolutionConditionId.Speed => "Required Spe",
            EvolutionConditionId.Link => "Required Link",
            EvolutionConditionId.Kingdom => "Required Kingdom",
            EvolutionConditionId.WarriorGender => "Required Gender",
            EvolutionConditionId.Item => "Required Item",
            EvolutionConditionId.JoinOffer => "N/A",
            EvolutionConditionId.NoCondition => "N/A",
            _ => throw new Exception($"Invalid Enum Value of {e.NewValue} in {nameof(EvolutionConditionControl)} {nameof(OnConditionPropertyChanged)}"),
        };
    }

    public static DependencyProperty QuantityProperty = UserControlUtil.RegisterDependencyProperty<EvolutionConditionControl, int>(v => v.Quantity, default, OnQuantityPropertyChanged);

    public int Quantity
    {
        get => (int)GetValue(QuantityProperty);
        set => SetValue(QuantityProperty, value);
    }

    private static void OnQuantityPropertyChanged(EvolutionConditionControl target, DependencyPropertyChangedEventArgs<int> e)
    {
        switch (target.Condition)
        {
            case EvolutionConditionId.Hp:
            case EvolutionConditionId.Attack:
            case EvolutionConditionId.Defence:
            case EvolutionConditionId.Speed:
            case EvolutionConditionId.Link:
                target.QuantityNumberBox.Value = e.NewValue;
                break;
            case EvolutionConditionId.Kingdom:
                target.KingdomComboBox.SelectedItem = (KingdomId)e.NewValue;
                break;
            case EvolutionConditionId.WarriorGender:
                target.GenderComboBox.SelectedItem = (GenderId)e.NewValue;
                break;
            case EvolutionConditionId.Item:
                target.ItemComboBox.SelectedItem = (ItemId)e.NewValue;
                break;
            case EvolutionConditionId.JoinOffer:
            case EvolutionConditionId.NoCondition:
                break;
            default:
                throw new Exception($"Invalid Enum Value of {e.NewValue} in {nameof(EvolutionConditionControl)} {nameof(OnQuantityPropertyChanged)}");
        }
    }

    private void ConditionCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var item = (sender as ComboBox)?.SelectedItem as EvolutionConditionId?;

        if (item != null)
        {
            Condition = (EvolutionConditionId)item;
            switch (Condition)
            {
                case EvolutionConditionId.Hp:
                case EvolutionConditionId.Attack:
                case EvolutionConditionId.Defence:
                case EvolutionConditionId.Speed:
                case EvolutionConditionId.Link:
                    if (Quantity > 511)
                    {
                        Quantity = 511;
                    }
                    QuantityNumberBox.Value = Quantity;
                    break;
                case EvolutionConditionId.Kingdom:
                    var kingdom = (KingdomId)Quantity;
                    if (!KingdomItems.Contains(kingdom))
                    {
                        Quantity = 0;
                    }
                    KingdomComboBox.SelectedItem = (KingdomId)Quantity;
                    break;
                case EvolutionConditionId.WarriorGender:
                    var gender = (GenderId)Quantity;
                    if (!GenderItems.Contains(gender))
                    {
                        Quantity = 0;
                    }
                    GenderComboBox.SelectedItem = (GenderId)Quantity;
                    break;
                case EvolutionConditionId.Item:
                    var itemVal = (ItemId)Quantity;
                    if (!ItemItems.Contains(itemVal))
                    {
                        Quantity = 0;
                    }
                    ItemComboBox.SelectedItem = (ItemId)Quantity;
                    break;
                case EvolutionConditionId.JoinOffer:
                case EvolutionConditionId.NoCondition:
                    break;
                default:
                    throw new Exception($"Invalid Enum Value of {Condition} in {nameof(EvolutionConditionControl)} {nameof(ConditionCombo_SelectionChanged)}");
            }
        }
    }

    private void QuantityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        switch (Condition)
        {
            case EvolutionConditionId.Hp:
            case EvolutionConditionId.Attack:
            case EvolutionConditionId.Defence:
            case EvolutionConditionId.Speed:
            case EvolutionConditionId.Link:
            case EvolutionConditionId.JoinOffer:
            case EvolutionConditionId.NoCondition:
                throw new Exception($"Should not be accessible");
            case EvolutionConditionId.Kingdom:
            case EvolutionConditionId.WarriorGender:
            case EvolutionConditionId.Item:
                Quantity = (int)((ComboBox)sender).SelectedItem;
                break;
            default:
                throw new Exception($"Invalid Enum Value of {Condition} in {nameof(EvolutionConditionControl)} {nameof(QuantityComboBox_SelectionChanged)}");
        }
    }

    private void QuantityNumberBox_ValueChanged(object sender, EventArgs e)
    {
        Quantity = ((NumberBox)sender).Value;
    }
}
