namespace Thumbler.ViewModel.Dialogs
{
	/// <summary>
	/// Represents a provider of user interface dialogs to be shown
	/// to the end user.
	/// </summary>
	interface IDialogProvider
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
		string SelectFolder(string description, string initialFolder);

		/// <summary>
		/// Shows an information dialog with a single OK button.
		/// </summary>
		/// <param name="title">The title of the message.</param>
		/// <param name="description">The message.</param>
		void ShowInformation(string title, string description);

		/// <summary>
		/// Shows and error dialog with a single OK button.
		/// </summary>
		/// <param name="title">The title of the message.</param>
		/// <param name="description">The message.</param>
		void ShowError(string title, string description);

		/// <summary>
		/// Shows a question dialog with Yes/No buttons.
		/// </summary>
		/// <param name="title">The title of the question.</param>
		/// <param name="question">The question.</param>
		/// <returns>
		/// <c>true</c> is user answered yes; otherwise <c>false</c>.
		/// </returns>
		bool AskYesNoQuestion(string title, string question);

		/// <summary>
		/// Shows a dialog asking the user whether she want to create the
		/// specified folder.
		/// </summary>
		/// <param name="path">The folder path.</param>
		/// <returns>
		/// <c>true</c> is user wants to create folder; otherwise <c>false</c>.
		/// </returns>
		bool AskToCreateFolder(string path);
	}
}
