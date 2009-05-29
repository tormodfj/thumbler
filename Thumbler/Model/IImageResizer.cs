using System;
using System.Collections.Generic;
using System.Drawing;

namespace Thumbler.Model
{
    /// <summary>
    /// Represents an object which can resize a collection of images.
    /// </summary>
    public interface IImageResizer
    {
        /// <summary>
        /// Gets the collection of strings with the paths to the images
        /// to be resized.
        /// </summary>
        IList<string> SourceFiles { get; }

        /// <summary>
        /// Gets or sets the size of the resized images.
        /// </summary>
        Size NewImageSize { get; set; }

        /// <summary>
        /// Gets or sets the path to the folder where the resized images
        /// are saved.
        /// </summary>
        string TargetFolder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether resizing preserves the
        /// aspect ratio of the images.
        /// </summary>
        /// <value><c>true</c> if resizing preserves the aspect ratio of the 
        /// images; otherwise, <c>false</c>.</value>
        bool PreserveAspectRatio { get; set; }

        /// <summary>
        /// Gets or sets the quality of the resized images.
        /// </summary>
        /// <remarks>This property is only applicable for codecs which
        /// takes a quality parameter, e.g. JPEG.</remarks>
        int Quality { get; set; }

        /// <summary>
        /// Resizes the images in the <see cref="SourceFiles"/> collection.
        /// </summary>
        /// <returns><c>true</c> if resizing completed successfully; otherwise
        /// <c>false</c>.</returns>
        bool ResizeImages();

        /// <summary>
        /// Cancels an active resize process.
        /// </summary>
        void CancelResizing();

        /// <summary>
        /// Gets a value in the interval [0, 1] indicating the progress of
        /// an active resize process. 
        /// </summary>
        float Progress { get; }

        /// <summary>
        /// Occurs when the <see cref="Progress"/> property changes.
        /// </summary>
        event EventHandler ProgressChanged;

        /// <summary>
        /// Occurs when the target folder already contains a file with the
        /// name of an input image.
        /// </summary>
        event EventHandler<FileNameCollisionEventArgs> FileNameColission;
    }
}
