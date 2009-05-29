using System.IO;
using System.Windows.Forms;

namespace Thumbler.ViewModel.Dialogs
{
    /// <summary>
    /// Provider of user interface dialogs on older Windows versions.
    /// </summary>
    class LegacyDialogProvider : IDialogProvider
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
            FolderBrowserDialog dialog = new FolderBrowserDialog
            {
                Description = description
            };
            if (Directory.Exists(initialFolder))
            {
                dialog.SelectedPath = initialFolder;
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.SelectedPath;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Shows an information dialog with a single OK button.
        /// </summary>
        /// <param name="title">The title of the message.</param>
        /// <param name="description">The message.</param>
        public void ShowInformation(string title, string description)
        {
            MessageBox.Show(description, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Shows and error dialog with a single OK button.
        /// </summary>
        /// <param name="title">The title of the message.</param>
        /// <param name="description">The message.</param>
        public void ShowError(string title, string description)
        {
            MessageBox.Show(description, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            return DialogResult.Yes ==
                MessageBox.Show(question, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
            return AskYesNoQuestion("Create Folder?",
                "The folder \"" + path + "\" does not exist. Do you want Thumbler to create it for you?");
        }
    }
}
