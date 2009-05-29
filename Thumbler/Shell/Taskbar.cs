using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using Thumbler.ViewModel;
using Platform = Microsoft.WindowsAPICodePack.Shell.Taskbar;

namespace Thumbler.Shell
{
    /// <summary>
    /// Contains interop logic for the Windows 7 taskbar.
    /// </summary>
    static class Taskbar
    {
        /// <summary>
        /// Registers the window handle of the specified window with the interop
        /// logic.
        /// </summary>
        /// <param name="window">The window whose handle to register.</param>
        internal static void RegisterWindowHandle(Window window)
        {
            Platform.Taskbar.OwnerHandle = new WindowInteropHelper(window).Handle;
        }

        /// <summary>
        /// Registers the view model which provides properties to reflect on
        /// the task bar.
        /// </summary>
        /// <param name="viewModel">The view model to register.</param>
        internal static void RegisterViewModel(IImageResizerViewModel viewModel)
        {
            viewModel.PropertyChanged += propertyChangedEventHandler;
        }

        /// <summary>
        /// Responds to changes in the properties of the view model.
        /// </summary>
        /// <param name="sender">The sender (view model).</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> 
        /// instance containing the event data.</param>
        private static void propertyChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Progress")
            {
                IImageResizerViewModel vm = sender as IImageResizerViewModel;
                if (vm != null)
                {
                    Platform.Taskbar.ProgressBar.MaxValue = 100;
                    Platform.Taskbar.ProgressBar.CurrentValue = vm.Progress;
                    Platform.Taskbar.ProgressBar.State = vm.Progress > 0
                        ? Platform.TaskbarButtonProgressState.Normal
                        : Platform.TaskbarButtonProgressState.NoProgress;
                }
            }
        }

    }
}
