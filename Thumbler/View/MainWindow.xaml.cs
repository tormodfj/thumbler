using System.Windows;
using Thumbler.ViewModel;

namespace Thumbler.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IImageResizerViewModel _vm;

        public MainWindow(IImageResizerViewModel viewModel)
        {
            InitializeComponent();

            _vm = viewModel;
            DataContext = _vm;
        }
    }
}
