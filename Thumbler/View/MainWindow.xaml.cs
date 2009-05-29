using System.Windows;
using Thumbler.ViewModel;

namespace Thumbler.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IImageResizerViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
