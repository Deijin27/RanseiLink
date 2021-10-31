using RanseiLink.Core.Randomization;
using System.Windows;
using System.Windows.Input;

namespace RanseiLink.Dialogs
{
    /// <summary>
    /// Interaction logic for ModCreationDialog.xaml
    /// </summary>
    public partial class RandomizeDialog : Window
    {
        public RandomizeDialog(IRandomizer randomizer)
        {
            InitializeComponent();
            ListItems.ItemsSource = randomizer.Options;
        }

        private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();

        }
    }
}