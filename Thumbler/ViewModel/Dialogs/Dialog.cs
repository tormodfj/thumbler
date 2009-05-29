using Thumbler.Shell;

namespace Thumbler.ViewModel.Dialogs
{
    /// <summary>
    /// Contains logic for showing dialogs to the user. If the user is
    /// running Windows Vista or newer, the new Task dialogs are used;
    /// otherwise, regular WinForms dialogs are used.
    /// </summary>
    internal static class Dialog
    {
        private static readonly IDialogProvider _dialogs;

        /// <summary>
        /// Initializes the <see cref="Dialog"/> class.
        /// </summary>
        static Dialog()
        {
            if (WindowsVersion.IsVistaOrNewer)
            {
                _dialogs = new VistaDialogProvider();
            }
            else
            {
                _dialogs = new LegacyDialogProvider();
            }
        }

        /// <summary>
        /// Prompts the user to select a folder
        /// </summary>
        /// <param name="description">The textual description of the
        /// operation to show to the user.</param>
        /// <param name="initialFolder">The initial folder.</param>
        /// <returns>The selected folder, or <c>null</c> if the user
        /// cancelled.</returns>
        public static string SelectFolder(string description, string initialFolder)
        {
            return _dialogs.SelectFolder(description, initialFolder);
        }

        /// <summary>
        /// Shows an information dialog with a single OK button.
        /// </summary>
        /// <param name="title">The title of the message.</param>
        /// <param name="description">The message.</param>
        public static void ShowInformation(string title, string description)
        {
            _dialogs.ShowInformation(title, description);
        }

        /// <summary>
        /// Shows and error dialog with a single OK button.
        /// </summary>
        /// <param name="title">The title of the message.</param>
        /// <param name="description">The message.</param>
        public static void ShowError(string title, string description)
        {
            _dialogs.ShowError(title, description);
        }

        /// <summary>
        /// Shows a question dialog with Yes/No buttons.
        /// </summary>
        /// <param name="title">The title of the question.</param>
        /// <param name="question">The question.</param>
        /// <returns>
        /// <c>true</c> is user answered yes; otherwise <c>false</c>.
        /// </returns>
        public static bool AskYesNoQuestion(string title, string question)
        {
            return _dialogs.AskYesNoQuestion(title, question);
        }

        /// <summary>
        /// Shows a dialog asking the user whether she want to create the
        /// specified folder.
        /// </summary>
        /// <param name="path">The folder path.</param>
        /// <returns>
        /// <c>true</c> is user wants to create folder; otherwise <c>false</c>.
        /// </returns>
        public static bool AskToCreateFolder(string path)
        {
            return _dialogs.AskToCreateFolder(path);
        }
    }
}
