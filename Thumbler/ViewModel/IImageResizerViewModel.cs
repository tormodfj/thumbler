using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Thumbler.ViewModel
{
	/// <summary>
	/// Represents a view model for Thumbler
	/// </summary>
	public interface IImageResizerViewModel : INotifyPropertyChanged
	{
		/// <summary>
		/// Gets the name of the application.
		/// </summary>
		/// <value>The name of the application.</value>
		string AppName { get; }

		/// <summary>
		/// Gets or sets the source folder.
		/// </summary>
		/// <value>The source folder.</value>
		string SourceFolder { get; set; }

		/// <summary>
		/// Gets or sets the target folder.
		/// </summary>
		/// <value>The target folder.</value>
		string TargetFolder { get; set; }

		/// <summary>
		/// Gets or sets the new height.
		/// </summary>
		/// <value>The new height.</value>
		int NewHeight { get; set; }

		/// <summary>
		/// Gets or sets the new width.
		/// </summary>
		/// <value>The new width.</value>
		int NewWidth { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the aspect ratio of the
		/// images whould be preserved.
		/// </summary>
		/// <value><c>true</c> if the aspect ratio of the images whould be 
		/// preserved; otherwise, <c>false</c>.</value>
		bool PreserveAspectRatio { get; set; }

		/// <summary>
		/// Gets or sets the quality.
		/// </summary>
		/// <value>The quality.</value>
		int Quality { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the controls of the window
		/// should be enabled.
		/// </summary>
		/// <value><c>true</c> if the controls of the window should be enabled; 
		/// otherwise, <c>false</c>.</value>
		bool ControlsEnabled { get; set; }

		/// <summary>
		/// Gets or sets the progress of the resize operation.
		/// </summary>
		/// <value>The progress of the resize operation.</value>
		int Progress { get; set; }

		/// <summary>
		/// Gets or sets the progress bar visibility.
		/// </summary>
		/// <value>The progress bar visibility.</value>
		Visibility ProgressBarVisibility { get; set; }

		/// <summary>
		/// Gets or sets the status on the status bar.
		/// </summary>
		/// <value>The status on the status bar.</value>
		string Status { get; set; }

		/// <summary>
		/// Gets the "resize images" command.
		/// </summary>
		/// <value>The "resize images" command.</value>
		ICommand ResizeImagesCommand { get; }

		/// <summary>
		/// Gets the "cancel resizing" command.
		/// </summary>
		/// <value>The "cancel resizing" command.</value>
		ICommand CancelResizingCommand { get; }

		/// <summary>
		/// Gets the "set source folder" command.
		/// </summary>
		/// <value>The "set source folder" command.</value>
		ICommand SetSourceFolderCommand { get; }

		/// <summary>
		/// Gets the "set target folder" command.
		/// </summary>
		/// <value>The "set target folder" command.</value>
		ICommand SetTargetFolderCommand { get; }
	}
}
