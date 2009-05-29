using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Thumbler.Model;
using Thumbler.Properties;
using Thumbler.ViewModel.Dialogs;

namespace Thumbler.ViewModel
{
	/// <summary>
	/// The default view model for Thumbler.
	/// </summary>
	public class ImageResizerViewModel : IImageResizerViewModel
	{
		private IImageResizer _resizer;
		private string _sourceFolder;
		private string _targetFolder;
		private int _quality;
		private int _newWidth;
		private int _newHeight;
		private bool _preserveAspectRatio;
		private string _status;
		private int _progress;
		private Visibility _progressBarVisibility;
		private bool _controlsEnabled;
		private bool _currentlyCancelling;

		private RelayCommand _resizeImagesCommand;
		private RelayCommand _cancelResizingCommand;
		private RelayCommand _setSourceFolderCommand;
		private RelayCommand _setTargetFolderCommand;

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Called when a property changed.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Gets the name of the application.
		/// </summary>
		/// <value>The name of the application.</value>
		public string AppName
		{
			get { return "Thumbler"; }
		}

		/// <summary>
		/// Gets or sets the target folder.
		/// </summary>
		/// <value>The target folder.</value>
		public string TargetFolder
		{
			get { return _targetFolder; }
			set
			{
				_targetFolder = Settings.Default.TargetFolder = value;
				OnPropertyChanged("TargetFolder");
			}
		}

		/// <summary>
		/// Gets or sets the source folder.
		/// </summary>
		/// <value>The source folder.</value>
		public string SourceFolder
		{
			get { return _sourceFolder; }
			set
			{
				_sourceFolder = Settings.Default.SourceFolder = value;
				OnPropertyChanged("SourceFolder");
			}
		}

		/// <summary>
		/// Gets or sets the quality.
		/// </summary>
		/// <value>The quality.</value>
		public int Quality
		{
			get { return _quality; }
			set
			{
				_quality = Settings.Default.Quality = value;
				OnPropertyChanged("Quality");
			}
		}

		/// <summary>
		/// Gets or sets the new width.
		/// </summary>
		/// <value>The new width.</value>
		public int NewWidth
		{
			get { return _newWidth; }
			set
			{
				_newWidth = Settings.Default.NewWidth = value;
				OnPropertyChanged("NewWidth");
			}
		}

		/// <summary>
		/// Gets or sets the new height.
		/// </summary>
		/// <value>The new height.</value>
		public int NewHeight
		{
			get { return _newHeight; }
			set
			{
				_newHeight = Settings.Default.NewHeight = value;
				OnPropertyChanged("NewHeight");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the aspect ratio of the
		/// images whould be preserved.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the aspect ratio of the images whould be
		/// preserved; otherwise, <c>false</c>.
		/// </value>
		public bool PreserveAspectRatio
		{
			get { return _preserveAspectRatio; }
			set
			{
				_preserveAspectRatio = Settings.Default.PreserveAspectRatio = value;
				OnPropertyChanged("PreserveAspectRatio");
			}
		}

		/// <summary>
		/// Gets or sets the status on the status bar.
		/// </summary>
		/// <value>The status on the status bar.</value>
		public string Status
		{
			get { return _status; }
			set
			{
				_status = value;
				OnPropertyChanged("Status");
			}
		}

		/// <summary>
		/// Gets or sets the progress of the resize operation.
		/// </summary>
		/// <value>The progress of the resize operation.</value>
		public int Progress
		{
			get { return _progress; }
			set
			{
				_progress = value;
				OnPropertyChanged("Progress");
				ProgressBarVisibility = _progress > 0 ? Visibility.Visible : Visibility.Hidden;
			}
		}

		/// <summary>
		/// Gets or sets the progress bar visibility.
		/// </summary>
		/// <value>The progress bar visibility.</value>
		public Visibility ProgressBarVisibility
		{
			get { return _progressBarVisibility; }
			set
			{
				_progressBarVisibility = value;
				OnPropertyChanged("ProgressBarVisibility");
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the controls of the window
		/// should be enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the controls of the window should be enabled;
		/// otherwise, <c>false</c>.
		/// </value>
		public bool ControlsEnabled
		{
			get { return _controlsEnabled; }
			set
			{
				_controlsEnabled = value;
				OnPropertyChanged("ControlsEnabled");
			}
		}

		/// <summary>
		/// Gets the "resize images" command.
		/// </summary>
		/// <value>The "resize images" command.</value>
		public ICommand ResizeImagesCommand
		{
			get
			{
				if (_resizeImagesCommand == null)
				{
					_resizeImagesCommand = new RelayCommand(
						param => ResizeImages(),
						param => Directory.Exists(_sourceFolder));
				}
				return _resizeImagesCommand;
			}
		}

		/// <summary>
		/// Gets the "set source folder" command.
		/// </summary>
		/// <value>The "set source folder" command.</value>
		public ICommand SetSourceFolderCommand
		{
			get
			{
				if (_setSourceFolderCommand == null)
				{
					_setSourceFolderCommand = new RelayCommand(
						param => SetSourceFolder());
				}
				return _setSourceFolderCommand;
			}
		}

		/// <summary>
		/// Gets the "set target folder" command.
		/// </summary>
		/// <value>The "set target folder" command.</value>
		public ICommand SetTargetFolderCommand
		{
			get
			{
				if (_setTargetFolderCommand == null)
				{
					_setTargetFolderCommand = new RelayCommand(
						param => SetTargetFolder());
				}
				return _setTargetFolderCommand;
			}
		}

		/// <summary>
		/// Gets the "cancel resizing" command.
		/// </summary>
		/// <value>The "cancel resizing" command.</value>
		public ICommand CancelResizingCommand
		{
			get
			{
				if (_cancelResizingCommand == null)
				{
					_cancelResizingCommand = new RelayCommand(
						param => CancelResizing(),
						param => !_currentlyCancelling);
				}
				return _cancelResizingCommand;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageResizerViewModel"/> 
		/// class.
		/// </summary>
		/// <param name="resizer">The resizer.</param>
		public ImageResizerViewModel(IImageResizer resizer)
		{
			_resizer = resizer;

			Initialize();
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		private void Initialize()
		{
			_sourceFolder = Settings.Default.SourceFolder;
			if (string.IsNullOrEmpty(_sourceFolder))
				_sourceFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

			_targetFolder = Settings.Default.TargetFolder;
			if (string.IsNullOrEmpty(_targetFolder))
				_targetFolder = _resizer.TargetFolder;

			_quality = Settings.Default.Quality;
			_newWidth = Settings.Default.NewWidth;
			_newHeight = Settings.Default.NewHeight;
			_preserveAspectRatio = Settings.Default.PreserveAspectRatio;

			_status = "Ready";
			_progress = 0;
			_progressBarVisibility = Visibility.Hidden;
			_controlsEnabled = true;
			_currentlyCancelling = false;

			_resizer.ProgressChanged += ProgressChanged;
			_resizer.FileNameColission += FileNameColission;
		}

		/// <summary>
		/// Shows a folder selection dialog and allow the user to set the source 
		/// folder from it.
		/// </summary>
		private void SetSourceFolder()
		{
			string folder = Dialog.SelectFolder("Select Input Folder", SourceFolder);
			if (folder != null)
			{
				SourceFolder = folder;
			}
		}

		/// <summary>
		/// Shows a folder selection dialog and allow the user to set the target 
		/// folder from it.
		/// </summary>
		private void SetTargetFolder()
		{
			string folder = Dialog.SelectFolder("Select Output Folder", TargetFolder);
			if (folder != null)
			{
				TargetFolder = folder;
			}
		}

		/// <summary>
		/// Resizes the images.
		/// </summary>
		/// <remarks>The operation is performed asynchronously on a thread
		/// from the thread pool.</remarks>
		private void ResizeImages()
		{
			if (IsFolderPresentOrCreatable(_targetFolder))
			{
				_resizer.SourceFiles.Clear();
				_resizer.NewImageSize = new System.Drawing.Size(_newWidth, _newHeight);
				_resizer.PreserveAspectRatio = _preserveAspectRatio;
				_resizer.Quality = _quality;
				_resizer.TargetFolder = _targetFolder;
				foreach (string file in Directory.GetFiles(_sourceFolder))
				{
					_resizer.SourceFiles.Add(file);
				}

				ControlsEnabled = false;
				Status = "Resizing images";

				Func<bool> asyncDelegate = new Func<bool>(_resizer.ResizeImages);
				asyncDelegate.BeginInvoke(ImageResizeCompleted, null);
			}
		}

		/// <summary>
		/// Determines whether a folder is present on the specified path. If it is
		/// not present, the user is prompted for it to be created.
		/// </summary>
		/// <param name="path">The path to the folder.</param>
		/// <returns>
		/// 	<c>true</c> if the folder is present or created; otherwise, <c>false</c>.
		/// </returns>
		private bool IsFolderPresentOrCreatable(string path)
		{
			if (!Directory.Exists(_targetFolder))
			{
				if (!Dialog.AskToCreateFolder(_targetFolder))
				{
					Dialog.ShowInformation(
						"Operation Cancelled", 
						"The operation could not be completed because the output folder does not exist");
					return false;
				}
				else
				{
					try
					{
						Directory.CreateDirectory(_targetFolder);
					}
					catch (UnauthorizedAccessException e)
					{
						Dialog.ShowError("Access Denied!", e.Message);
						return false;
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Cancels the resizing process.
		/// </summary>
		private void CancelResizing()
		{
			Status = "Cancelling";
			_currentlyCancelling = true;
			_resizer.CancelResizing();
		}

		/// <summary>
		/// Called when the asynchronous resize operation is completed.
		/// </summary>
		/// <param name="asyncResult">The result of the operation.</param>
		private void ImageResizeCompleted(IAsyncResult asyncResult)
		{
			AsyncResult result = (AsyncResult)asyncResult;
			Func<bool> asyncDelegate = (Func<bool>)result.AsyncDelegate;
			bool returnValue;
			try
			{
				returnValue = asyncDelegate.EndInvoke(asyncResult);
				if (!returnValue)
				{
					Dispatcher.CurrentDispatcher.Invoke(
						new Action<string, string>(Dialog.ShowError), 
						"Operation Aborted", 
						"The operation was aborted.");
				}
			}
			catch (Exception e)
			{
				Dispatcher.CurrentDispatcher.Invoke(
					new Action<string,string>(Dialog.ShowError), 
					"Error!", 
					e.Message);
			}
			finally
			{
				_currentlyCancelling = false;
				ControlsEnabled = true;
				Progress = 0;
				Status = "Ready";
			}
		}

		/// <summary>
		/// Event handler for the ProgressChanged event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing 
		/// the event data.</param>
		private void ProgressChanged(object sender, EventArgs e)
		{
			Progress = (int)Math.Ceiling(_resizer.Progress * 100);
		}

		/// <summary>
		/// Event handler for the FileNameCollision event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="Thumbler.Model.FileNameCollisionEventArgs"/> 
		/// instance containing the event data.</param>
		private void FileNameColission(object sender, FileNameCollisionEventArgs e)
		{
			// TODO: Ask user
			e.Action = FileNameCollisionEventArgs.CollisionAction.Overwrite;
		}
	}
}
