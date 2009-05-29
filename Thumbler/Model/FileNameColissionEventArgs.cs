using System;

namespace Thumbler.Model
{
	/// <summary>
	/// Event data for a file name collision.
	/// </summary>
    public class FileNameCollisionEventArgs : EventArgs
    {
		/// <summary>
		/// Possible actions when a file name collision occurs.
		/// </summary>
        public enum CollisionAction 
		{
			/// <summary>
			/// Overwrite the old file with the new file.
			/// </summary>
			Overwrite,
			/// <summary>
			/// Keep both files, generating a unique new name for the new file.
			/// </summary>
			KeepBoth,
			/// <summary>
			/// Keep the old file, skipping the new file.
			/// </summary>
			Skip,
			/// <summary>
			/// Abort the entire file generation process.
			/// </summary>
			AbortOperation 
		};
		/// <summary>
		/// Gets or sets the action to perform as a response to the collision.
		/// </summary>
		/// <value>The action to perform as a response to the collision.</value>
        public CollisionAction Action { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="FileNameCollisionEventArgs"/> class.
		/// </summary>
        public FileNameCollisionEventArgs()
            : this(CollisionAction.Skip)
        { }

		/// <summary>
		/// Initializes a new instance of the <see cref="FileNameCollisionEventArgs"/> class.
		/// </summary>
		/// <param name="defaultAction">The default action.</param>
        public FileNameCollisionEventArgs(CollisionAction defaultAction)
        {
            Action = defaultAction;
        }
    }
}
