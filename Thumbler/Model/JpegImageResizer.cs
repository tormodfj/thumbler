using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Thumbler.Model
{
	/// <summary>
	/// Class for resizing a collection of images to a specified size,
	/// using the JPEG codec.
	/// </summary>
    class JpegImageResizer : IImageResizer
    {
        private readonly ImageCodecInfo _codec;
		private bool _cancel;

		/// <summary>
		/// Occurs when the <see cref="Progress"/> property changes.
		/// </summary>
        public event EventHandler ProgressChanged;
		/// <summary>
		/// Raises the <see cref="E:ProgressChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing 
		/// the event data.</param>
        protected virtual void OnProgressChanged(EventArgs e)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, e);
        }

		/// <summary>
		/// Occurs when the target folder already contains a file with the
		/// name of an input image.
		/// </summary>
        public event EventHandler<FileNameCollisionEventArgs> FileNameColission;
		/// <summary>
		/// Raises the <see cref="E:FileNameColission"/> event.
		/// </summary>
		/// <param name="e">The <see cref="Thumbler.Model.FileNameColissionEventArgs"/>
		/// instance containing the event data.</param>
        protected virtual void OnFileNameColission(FileNameCollisionEventArgs e)
        {
            if (FileNameColission != null)
                FileNameColission(this, e);
        }

		/// <summary>
		/// Gets the collection of strings with the paths to the images
		/// to be resized.
		/// </summary>
        public IList<string> SourceFiles { get; private set; }

		/// <summary>
		/// Gets or sets the size of the resized images.
		/// </summary>
        public Size NewImageSize { get; set; }

		/// <summary>
		/// Gets or sets the path to the folder where the resized images
		/// are saved.
		/// </summary>
        public string TargetFolder { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether resizing preserves the
		/// aspect ratio of the images.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if resizing preserves the aspect ratio of the
		/// images; otherwise, <c>false</c>.
		/// </value>
        public bool PreserveAspectRatio { get; set; }

		/// <summary>
		/// Gets a value in the interval [0, 1] indicating the progress of
		/// an active resize process.
		/// </summary>
        public float Progress { get; private set; }

		/// <summary>
		/// Gets or sets the quality of the resized images.
		/// </summary>
        public int Quality { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="JpegImageResizer"/> class.
		/// </summary>
        public JpegImageResizer()
        {
            _codec = getJpegCodec();
			_cancel = false;

            SourceFiles = new List<string>();
            NewImageSize = new Size(200, 200);
			TargetFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Output");
            PreserveAspectRatio = true;
            Progress = 0f;
            Quality = 65;
        }

		/// <summary>
		/// Resizes the images in the <see cref="SourceFiles"/> collection.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if resizing completed successfully; otherwise
		/// <c>false</c>.
		/// </returns>
        public bool ResizeImages()
        {
            if (NewImageSize.Height <= 0 || NewImageSize.Width <= 0)
                throw new InvalidOperationException("MaximumImageSize must have positive width and height.");
            if (String.IsNullOrEmpty(TargetFolder))
                throw new InvalidOperationException("Target folder is not set.");
            if (!Directory.Exists(TargetFolder))
                throw new InvalidOperationException("Target folder does not exist.");
            if (Quality < 20 || Quality > 100)
                throw new InvalidOperationException("Quality must be in range [20, 100].");

            EncoderParameters parameters = new EncoderParameters();
            parameters.Param[0] = new EncoderParameter(Encoder.Quality, Quality);

            string[] sourceFiles = SourceFiles.ToArray();
            SourceFiles.Clear();

            for (int i = 0; i < sourceFiles.Length; i++)
            {
				if (_cancel)
				{
					break;
				}
                else if (resizeImage(sourceFiles[i], parameters))
                {
                    Progress = (float)i / (float)sourceFiles.Length;
                    OnProgressChanged(EventArgs.Empty);
                }
                else
                {
                    return false;
                }
            }
			_cancel = false;
			Progress = 0f;
            OnProgressChanged(EventArgs.Empty);
            return true;
        }

		/// <summary>
		/// Cancels an active resize process.
		/// </summary>
		public void CancelResizing()
		{
			_cancel = true;
		}

		/// <summary>
		/// Resizes the specified image using the specified parameters.
		/// </summary>
		/// <param name="sourceFile">The source file.</param>
		/// <param name="param">The parameters.</param>
		/// <returns></returns>
        private bool resizeImage(string sourceFile, EncoderParameters param)
        {
            string targetFile = getTargetFileName(sourceFile);
            if (File.Exists(targetFile))
            {
                FileNameCollisionEventArgs e = new FileNameCollisionEventArgs();
                OnFileNameColission(e);
                switch (e.Action)
                {
                    case FileNameCollisionEventArgs.CollisionAction.Skip:
                        return true;

                    case FileNameCollisionEventArgs.CollisionAction.AbortOperation:
                        return false;

                    case FileNameCollisionEventArgs.CollisionAction.KeepBoth:
                        targetFile = getSafeTargetFileName(sourceFile);
                        break;
                }
            }

            try
            {
                using (Image sourceImage = Image.FromFile(sourceFile))
                {
                    Size newSize = calculateNewSize(sourceImage.Size);
                    using (Bitmap newImage = new Bitmap(sourceImage, newSize))
                    {
                        newImage.Save(targetFile, _codec, param);
                    }
                    return true;
                }
            }
            catch (OutOfMemoryException)
            {
                // The file does not have a valid image format.-or- GDI+ does not support the
                // pixel format of the file. Ignore and try next.
                return true;
            }
        }

		/// <summary>
		/// Gets the JPEG codec.
		/// </summary>
		/// <returns>The JPEG codec.</returns>
		/// <exception cref="NotSupportedException">If no JPEG coded was
		/// found on the system.</exception>
        private ImageCodecInfo getJpegCodec()
        {
            foreach (var encoder in ImageCodecInfo.GetImageEncoders())
            {
                if (encoder.MimeType == "image/jpeg")
                {
                    return encoder;
                }
            }
            throw new NotSupportedException("JPEG codec not found on system");
        }

		/// <summary>
		/// Gets the name of the target file based on the source file.
		/// </summary>
		/// <param name="sourceFile">The source file.</param>
		/// <returns>The name of the target file based on the source file.</returns>
        private string getTargetFileName(string sourceFile)
        {
            return Path.Combine(TargetFolder, Path.GetFileNameWithoutExtension(sourceFile)) + ".jpeg";
        }

		/// <summary>
		/// Gets the name of the target file based on the source file, 
		/// avoiding existing file names.
		/// </summary>
		/// <param name="sourceFile">The source file.</param>
		/// <returns>The name of the target file based on the source file.</returns>
		private string getSafeTargetFileName(string sourceFile)
        {
            string file = Path.Combine(TargetFolder, Path.GetFileNameWithoutExtension(sourceFile));
            string ext = "({0}).jpeg";
            int counter = 1;

            while (File.Exists(file + string.Format(ext, counter))) counter++;

            return file + string.Format(ext, counter);
        }

		/// <summary>
		/// Calculates the new size of the image.
		/// </summary>
		/// <param name="oldSize">The old size.</param>
		/// <returns>The new size of the image.</returns>
        private Size calculateNewSize(Size oldSize)
        {
            if (PreserveAspectRatio)
            {
                Size newSize = new Size();
                if (oldSize.Width > oldSize.Height)
                {
                    newSize.Width = NewImageSize.Width;
                    newSize.Height = (int)((float)oldSize.Height / oldSize.Width * newSize.Width);
                }
                else
                {
                    newSize.Height = NewImageSize.Height;
                    newSize.Width = (int)((float)oldSize.Width / oldSize.Height * newSize.Height);
                }
                return newSize;
            }
            else
            {
                return NewImageSize;
            }
        }
	}
}
