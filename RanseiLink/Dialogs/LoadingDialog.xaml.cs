using System.Windows;
using System.Windows.Input;

namespace RanseiLink.Dialogs
{
    /// <summary>
    /// Interaction logic for ModCreationDialog.xaml
    /// </summary>
    public partial class LoadingDialog : Window
    {
        public LoadingDialog(string headerText)
        {
            InitializeComponent();
            HeaderTextBlock.Text = headerText;
        }

        private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

    }
}