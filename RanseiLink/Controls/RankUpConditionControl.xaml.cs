using RanseiLink.Core;
using RanseiLink.Core.Enums;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RanseiLink.Controls
{
    /// <summary>
    /// Interaction logic for EvolutionConditionControl.xaml
    /// </summary>
    public partial class RankUpConditionControl : UserControl
    {
        private readonly EpisodeId[] EpisodeItems = EnumUtil.GetValues<EpisodeId>().ToArray();
        private readonly TypeId[] TypeItems = EnumUtil.GetValues<TypeId>().ToArray();
        private readonly WarriorLineId[] WarriorLineItems = EnumUtil.GetValues<WarriorLineId>().ToArray();
        public RankUpConditionControl()
        {
            InitializeComponent();

            ConditionCombo.ItemsSource = EnumUtil.GetValues<RankUpConditionId>().ToArray();
            EpisodeComboBox1.ItemsSource = EpisodeItems;
            TypeComboBox1.ItemsSource = TypeItems;
            WarriorLineComboBox1.ItemsSource = WarriorLineItems;
            EpisodeComboBox2.ItemsSource = EpisodeItems;
            TypeComboBox2.ItemsSource = TypeItems;
            WarriorLineComboBox2.ItemsSource = WarriorLineItems;
        }

        public static DependencyProperty ConditionProperty = UserControlUtil.RegisterDependencyProperty<RankUpConditionControl, RankUpConditionId>(v => v.Condition, default, OnConditionPropertyChanged);

        public RankUpConditionId Condition
        {
            get => (RankUpConditionId)GetValue(ConditionProperty);
            set => SetValue(ConditionProperty, value);
        }

        private static void OnConditionPropertyChanged(RankUpConditionControl target, DependencyPropertyChangedEventArgs<RankUpConditionId> e)
        {
            target.ConditionCombo.SelectedItem = e.NewValue;
            target.QuantityNumberBox1.Visibility = Visibility.Hidden;
            target.EpisodeComboBox1.Visibility = Visibility.Hidden;
            target.TypeComboBox1.Visibility = Visibility.Hidden;
            target.WarriorLineComboBox1.Visibility = Visibility.Hidden;
            target.QuantityNumberBox2.Visibility = Visibility.Hidden;
            target.EpisodeComboBox2.Visibility = Visibility.Hidden;
            target.TypeComboBox2.Visibility = Visibility.Hidden;
            target.WarriorLineComboBox2.Visibility = Visibility.Hidden;

            switch (e.NewValue)
            {
                case RankUpConditionId.Unknown:
                case RankUpConditionId.NoCondition:
                case RankUpConditionId.Unused_1:
                case RankUpConditionId.Unused_2:
                case RankUpConditionId.Unused_3:
                case RankUpConditionId.Unused_4:
                    target.QuantityNumberBox1.Visibility = Visibility.Visible;
                    target.QuantityNumberBox2.Visibility = Visibility.Visible;
                    break;

                case RankUpConditionId.AtLeastNFemaleWarlordsInSameKingdom:
                case RankUpConditionId.AtLeastNGalleryPokemon:
                case RankUpConditionId.AtLeastNGalleryWarriors:
                    target.QuantityNumberBox1.Visibility = Visibility.Visible;
                    target.QuantityNumberBox2.Visibility = Visibility.Visible;
                    break;

                case RankUpConditionId.AfterCompletingEpisode:
                case RankUpConditionId.DuringEpisode:
                    target.EpisodeComboBox1.Visibility = Visibility.Visible;
                    target.EpisodeComboBox2.Visibility = Visibility.Visible;
                    break;

                case RankUpConditionId.MonotypeGallery:
                    target.TypeComboBox1.Visibility = Visibility.Visible;
                    target.TypeComboBox2.Visibility = Visibility.Visible;
                    break;

                case RankUpConditionId.WarriorInSameArmyNotNearby:
                case RankUpConditionId.WarriorInSameKingdom:
                    target.WarriorLineComboBox1.Visibility = Visibility.Visible;
                    target.WarriorLineComboBox2.Visibility = Visibility.Visible;
                    break;
                default:
                    throw new Exception($"Invalid Enum Value of {e.NewValue} in {nameof(EvolutionConditionControl)} {nameof(OnConditionPropertyChanged)}");
            }

            string text = e.NewValue switch
            {
                RankUpConditionId.Unknown => "Required Quantity",
                RankUpConditionId.AfterCompletingEpisode => "Required Episode",
                RankUpConditionId.DuringEpisode => "Required Episode",
                RankUpConditionId.Unused_1 => "Required Quantity",
                RankUpConditionId.WarriorInSameArmyNotNearby => "Required Warrior",
                RankUpConditionId.WarriorInSameKingdom => "Required Warrior",
                RankUpConditionId.AtLeastNFemaleWarlordsInSameKingdom => "Required Number",
                RankUpConditionId.Unused_2 => "Required Quantity",
                RankUpConditionId.AtLeastNGalleryPokemon => "Required Number",
                RankUpConditionId.MonotypeGallery => "Required Type",
                RankUpConditionId.AtLeastNGalleryWarriors => "Required Number",
                RankUpConditionId.Unused_3 => "Required Quantity",
                RankUpConditionId.Unused_4 => "Required Quantity",
                RankUpConditionId.NoCondition => "Required Quantity",
                _ => throw new Exception($"Invalid Enum Value of {e.NewValue} in {nameof(EvolutionConditionControl)} {nameof(OnConditionPropertyChanged)}"),
            };

            target.QuantityName1.Text = text;
            target.QuantityName2.Text = text;
        }

        public static DependencyProperty Quantity1Property = UserControlUtil.RegisterDependencyProperty<RankUpConditionControl, uint>(v => v.Quantity1, default, OnQuantity1PropertyChanged);

        public uint Quantity1
        {
            get => (uint)GetValue(Quantity1Property);
            set => SetValue(Quantity1Property, value);
        }

        public static DependencyProperty Quantity2Property = UserControlUtil.RegisterDependencyProperty<RankUpConditionControl, uint>(v => v.Quantity2, default, OnQuantity2PropertyChanged);

        public uint Quantity2
        {
            get => (uint)GetValue(Quantity2Property);
            set => SetValue(Quantity2Property, value);
        }

        private static void OnQuantity1PropertyChanged(RankUpConditionControl target, DependencyPropertyChangedEventArgs<uint> e)
        {
            switch (target.Condition)
            {
                case RankUpConditionId.Unknown:
                case RankUpConditionId.NoCondition:
                case RankUpConditionId.Unused_1:
                case RankUpConditionId.Unused_2:
                case RankUpConditionId.Unused_3:
                case RankUpConditionId.Unused_4:
                    target.QuantityNumberBox1.Value = e.NewValue;
                    break;

                case RankUpConditionId.AtLeastNFemaleWarlordsInSameKingdom:
                case RankUpConditionId.AtLeastNGalleryPokemon:
                case RankUpConditionId.AtLeastNGalleryWarriors:
                    target.QuantityNumberBox1.Value = e.NewValue;
                    break;

                case RankUpConditionId.AfterCompletingEpisode:
                case RankUpConditionId.DuringEpisode:
                    target.EpisodeComboBox1.SelectedItem = (EpisodeId)e.NewValue;
                    break;

                case RankUpConditionId.MonotypeGallery:
                    target.TypeComboBox1.SelectedItem = e.NewValue == 511u ? TypeId.NoType : (TypeId)e.NewValue;
                    break;

                case RankUpConditionId.WarriorInSameArmyNotNearby:
                case RankUpConditionId.WarriorInSameKingdom:
                    target.WarriorLineComboBox1.SelectedItem = (WarriorLineId)e.NewValue;
                    break;
                default:
                    throw new Exception($"Invalid Enum Value of {e.NewValue} in {nameof(EvolutionConditionControl)} {nameof(OnQuantity1PropertyChanged)}");
            }
        }

        private static void OnQuantity2PropertyChanged(RankUpConditionControl target, DependencyPropertyChangedEventArgs<uint> e)
        {
            switch (target.Condition)
            {
                case RankUpConditionId.Unknown:
                case RankUpConditionId.NoCondition:
                case RankUpConditionId.Unused_1:
                case RankUpConditionId.Unused_2:
                case RankUpConditionId.Unused_3:
                case RankUpConditionId.Unused_4:
                    target.QuantityNumberBox2.Value = e.NewValue;
                    break;

                case RankUpConditionId.AtLeastNFemaleWarlordsInSameKingdom:
                case RankUpConditionId.AtLeastNGalleryPokemon:
                case RankUpConditionId.AtLeastNGalleryWarriors:
                    target.QuantityNumberBox2.Value = e.NewValue;
                    break;

                case RankUpConditionId.AfterCompletingEpisode:
                case RankUpConditionId.DuringEpisode:
                    target.EpisodeComboBox2.SelectedItem = (EpisodeId)e.NewValue;
                    break;

                case RankUpConditionId.MonotypeGallery:
                    target.TypeComboBox2.SelectedItem = e.NewValue == 511u ? TypeId.NoType : (TypeId)e.NewValue;
                    break;

                case RankUpConditionId.WarriorInSameArmyNotNearby:
                case RankUpConditionId.WarriorInSameKingdom:
                    target.WarriorLineComboBox2.SelectedItem = (WarriorLineId)e.NewValue;
                    break;
                default:
                    throw new Exception($"Invalid Enum Value of {e.NewValue} in {nameof(EvolutionConditionControl)} {nameof(OnQuantity2PropertyChanged)}");
            }
        }

        private void ConditionCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (sender as ComboBox)?.SelectedItem as RankUpConditionId?;

            if (item != null)
            {
                Condition = (RankUpConditionId)item;
                switch (Condition)
                {
                    case RankUpConditionId.Unknown:
                    case RankUpConditionId.NoCondition:
                    case RankUpConditionId.Unused_1:
                    case RankUpConditionId.Unused_2:
                    case RankUpConditionId.Unused_3:
                    case RankUpConditionId.Unused_4:
                        if (Quantity1 > 511)
                        {
                            Quantity1 = 511;
                        }
                        if (Quantity2 > 511)
                        {
                            Quantity2 = 511;
                        }
                        QuantityNumberBox1.Value = Quantity1;
                        QuantityNumberBox2.Value = Quantity2;
                        break;

                    case RankUpConditionId.AtLeastNFemaleWarlordsInSameKingdom:
                    case RankUpConditionId.AtLeastNGalleryPokemon:
                    case RankUpConditionId.AtLeastNGalleryWarriors:
                        if (Quantity1 > 511)
                        {
                            Quantity1 = 511;
                        }
                        if (Quantity2 > 511)
                        {
                            Quantity2 = 511;
                        }
                        QuantityNumberBox1.Value = Quantity1;
                        QuantityNumberBox2.Value = Quantity2;
                        break;

                    case RankUpConditionId.AfterCompletingEpisode:
                    case RankUpConditionId.DuringEpisode:
                        if (!EpisodeItems.Contains((EpisodeId)Quantity1))
                        {
                            Quantity1 = 0;
                        }
                        if (!EpisodeItems.Contains((EpisodeId)Quantity2))
                        {
                            Quantity2 = 0;
                        }
                        EpisodeComboBox1.SelectedItem = (EpisodeId)Quantity1;
                        EpisodeComboBox2.SelectedItem = (EpisodeId)Quantity2;
                        break;

                    case RankUpConditionId.MonotypeGallery:
                        if (!TypeItems.Contains((TypeId)Quantity1))
                        {
                            Quantity1 = 0;
                        }
                        if (!TypeItems.Contains((TypeId)Quantity2))
                        {
                            Quantity2 = 0;
                        }
                        TypeComboBox1.SelectedItem = (TypeId)Quantity1;
                        TypeComboBox2.SelectedItem = (TypeId)Quantity2;
                        break;

                    case RankUpConditionId.WarriorInSameArmyNotNearby:
                    case RankUpConditionId.WarriorInSameKingdom:
                        if (!WarriorLineItems.Contains((WarriorLineId)Quantity1))
                        {
                            Quantity1 = 0;
                        }
                        if (!WarriorLineItems.Contains((WarriorLineId)Quantity2))
                        {
                            Quantity2 = 0;
                        }
                        WarriorLineComboBox1.SelectedItem = (WarriorLineId)Quantity1;
                        WarriorLineComboBox2.SelectedItem = (WarriorLineId)Quantity2;
                        break;
                    default:
                        throw new Exception($"Invalid Enum Value of {item} in {nameof(EvolutionConditionControl)} {nameof(ConditionCombo_SelectionChanged)}");
                }
            }
        }

        private void QuantityComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (Condition)
            {
                case RankUpConditionId.Unknown:
                case RankUpConditionId.NoCondition:
                case RankUpConditionId.Unused_1:
                case RankUpConditionId.Unused_2:
                case RankUpConditionId.Unused_3:
                case RankUpConditionId.Unused_4:
                case RankUpConditionId.AtLeastNFemaleWarlordsInSameKingdom:
                case RankUpConditionId.AtLeastNGalleryPokemon:
                case RankUpConditionId.AtLeastNGalleryWarriors:
                    throw new Exception("Should not be accessible");

                case RankUpConditionId.MonotypeGallery:
                    var val = (uint)((ComboBox)sender).SelectedItem;
                    Quantity1 = val == (uint)TypeId.NoType ? 511u : val;
                    break;

                case RankUpConditionId.AfterCompletingEpisode:
                case RankUpConditionId.DuringEpisode:
                case RankUpConditionId.WarriorInSameArmyNotNearby:
                case RankUpConditionId.WarriorInSameKingdom:
                    Quantity1 = (uint)((ComboBox)sender).SelectedItem;
                    break;
                default:
                    throw new Exception($"Invalid Enum Value of {Condition} in {nameof(EvolutionConditionControl)} {nameof(QuantityComboBox1_SelectionChanged)}");
            }
        }

        private void QuantityComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (Condition)
            {
                case RankUpConditionId.Unknown:
                case RankUpConditionId.NoCondition:
                case RankUpConditionId.Unused_1:
                case RankUpConditionId.Unused_2:
                case RankUpConditionId.Unused_3:
                case RankUpConditionId.Unused_4:
                case RankUpConditionId.AtLeastNFemaleWarlordsInSameKingdom:
                case RankUpConditionId.AtLeastNGalleryPokemon:
                case RankUpConditionId.AtLeastNGalleryWarriors:
                    throw new Exception("Should not be accessible");

                case RankUpConditionId.MonotypeGallery:
                    var val = (uint)((ComboBox)sender).SelectedItem;
                    Quantity2 = val == (uint)TypeId.NoType ? 511u : val;
                    break;

                case RankUpConditionId.AfterCompletingEpisode:
                case RankUpConditionId.DuringEpisode:
                case RankUpConditionId.WarriorInSameArmyNotNearby:
                case RankUpConditionId.WarriorInSameKingdom:
                    Quantity2 = (uint)((ComboBox)sender).SelectedItem;
                    break;
                default:
                    throw new Exception($"Invalid Enum Value of {Condition} in {nameof(EvolutionConditionControl)} {nameof(QuantityComboBox2_SelectionChanged)}");
            }
        }

        private void QuantityNumberBox1_ValueChanged(object sender, EventArgs e)
        {
            Quantity1 = ((NumberBox)sender).Value;
        }

        private void QuantityNumberBox2_ValueChanged(object sender, EventArgs e)
        {
            Quantity2 = ((NumberBox)sender).Value;
        }
    }
}
