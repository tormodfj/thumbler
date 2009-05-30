using System;
using System.IO;
using Microsoft.WindowsAPICodePack;
using Microsoft.WindowsAPICodePack.Shell;

namespace Thumbler.ViewModel.Dialogs
{
	/// <summary>
	/// Provider of user interface dialogs on Windows Vista or newer.
	/// </summary>
	class VistaDialogProvider : IDialogProvider
	{
		/// <summary>
		/// Shows a dialog that lets the user select a folder
		/// </summary>
		/// <param name="description">The textual description of the operation
		/// to show to the user.</param>
		/// <param name="initialFolder">The initial folder.</param>
		/// <returns>
		/// The selected folder, or <c>null</c> if the user cancelled.
		/// </returns>
		public string SelectFolder(string description, string initialFolder)
		{
			CommonOpenFileDialog dialog = new CommonOpenFileDialog
			{
				Title = description,
				FoldersOnly = true,
				CheckFileExists = true,
				UsageIdentifier = new Guid("{E69E4F08-E54D-4523-891F-1B245D644DA7}")
			};
			if (initialFolder != null && Directory.Exists(initialFolder))
			{
				dialog.InitialDirectory = initialFolder;
			}

			CommonFileDialogResult result = dialog.ShowDialog();
			if (result.Canceled)
			{
				return null;
			}
			else
			{
				return dialog.FileName;
			}
		}

		/// <summary>
		/// Shows an information dialog with a single OK button.
		/// </summary>
		/// <param name="title">The title of the message.</param>
		/// <param name="description">The message.</param>
		public void ShowInformation(string title, string description)
		{
			TaskDialog dialog = new TaskDialog
			{
				Caption = title,
				Instruction = title,
				Content = description,
				MainIcon = TaskDialogStandardIcon.Information,
				StandardButtons = TaskDialogStandardButtons.Close
			};

			dialog.Show();
		}

		/// <summary>
		/// Shows and error dialog with a single OK button.
		/// </summary>
		/// <param name="title">The title of the message.</param>
		/// <param name="description">The message.</param>
		public void ShowError(string title, string description)
		{
			TaskDialog dialog = new TaskDialog
			{
				Caption = title,
				Instruction = title,
				Content = description,
				MainIcon = TaskDialogStandardIcon.Error,
				StandardButtons = TaskDialogStandardButtons.Close
			};

			dialog.Show();
		}

		/// <summary>
		/// Shows a question dialog with Yes/No buttons.
		/// </summary>
		/// <param name="title">The title of the question.</param>
		/// <param name="question">The question.</param>
		/// <returns>
		/// 	<c>true</c> is user answered yes; otherwise <c>false</c>.
		/// </returns>
		public bool AskYesNoQuestion(string title, string question)
		{
			TaskDialog dialog = new TaskDialog
			{
				Caption = title,
				Instruction = title,
				Content = question,
				MainIcon = TaskDialogStandardIcon.Warning,
				StandardButtons = TaskDialogStandardButtons.YesNo
			};

			TaskDialogResult result = dialog.Show();
			return result.StandardButtonClicked == TaskDialogStandardButton.Yes;
		}

		/// <summary>
		/// Shows a dialog asking the user whether she want to create the
		/// specified folder.
		/// </summary>
		/// <param name="path">The folder path.</param>
		/// <returns>
		/// 	<c>true</c> is user wants to create folder; otherwise <c>false</c>.
		/// </returns>
		public bool AskToCreateFolder(string path)
		{
			const string createButtonName = "create";
			const string cancelButtonName = "cancel";

			TaskDialog dialog = new TaskDialog
			{
				Caption = "Create folder?",
				Instruction = "Create folder?",
				Content = "The folder \"" + path + "\" does not exist.",
				MainIcon = TaskDialogStandardIcon.Warning,
				Controls = 
				{
					new TaskDialogCommandLink
					{
						Text = "Create Folder",
						Instruction = "This will create the folder for you and start the image resizing.",
						Name = createButtonName
					}, 
					new TaskDialogCommandLink
					{
						Text = "Cancel",
						Instruction = "This will cancel the operation and let you choose another output folder.",
						Name = cancelButtonName
					}
				}
			};

			TaskDialogResult result = dialog.Show();
			return result.CustomButtonClicked == createButtonName;
		}
	}
}
