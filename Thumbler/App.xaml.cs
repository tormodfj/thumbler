using System.Windows;
using Thumbler.Model;
using Thumbler.Properties;
using Thumbler.Shell;
using Thumbler.View;
using Thumbler.ViewModel;

namespace Thumbler
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that 
        /// contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IImageResizer model = new JpegImageResizer();
            IImageResizerViewModel viewModel = new ImageResizerViewModel(model);
            Window window = new MainWindow(viewModel);
            window.Show();

            if (WindowsVersion.IsSevenOrNewer)
            {
                Taskbar.RegisterWindowHandle(window);
                Taskbar.RegisterViewModel(viewModel);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Exit"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.ExitEventArgs"/> that 
        /// contains the event data.</param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            Settings.Default.Save();
        }
    }
}
