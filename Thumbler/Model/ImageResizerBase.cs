using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Thumbler.Model
{
	internal abstract class ImageResizerBase : IImageResizer
	{
		private bool _cancel;
		private float _progress;

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
		public float Progress
		{
			get { return _progress; }
			private set
			{
				_progress = value;
				OnProgressChanged(EventArgs.Empty);
			}
		}

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
		/// Initializes a new instance of the <see cref="ImageResizerBase"/> class.
		/// </summary>
		public ImageResizerBase()
		{
			SourceFiles = new List<string>();
			NewImageSize = new Size(400, 400);
			TargetFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Output");
			PreserveAspectRatio = true;

			_progress = 0f;
			_cancel = false;
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
			ValidatePropertiesBeforeResize();

			string[] sourceFiles = SourceFiles.ToArray();
			SourceFiles.Clear();

			try
			{
				for (int i = 0; i < sourceFiles.Length; i++)
				{
					if (_cancel) break;

					if (tryResizeImage(sourceFiles[i]))
					{
						Progress = (float)i / (float)sourceFiles.Length;
					}
					else
					{
						return false;
					}
				}
			}
			finally
			{
				_cancel = false;
				Progress = 0f;
			}

			return true;
		}

		/// <summary>
		/// Validates the properties.
		/// </summary>
		/// <exception cref="InvalidOperationException">If properties are not
		/// valid for the resize operation.</exception>
		protected virtual void ValidatePropertiesBeforeResize()
		{
			if (NewImageSize.Height <= 0 || NewImageSize.Width <= 0)
				throw new InvalidOperationException("NewImageSize must have positive width and height.");
			if (String.IsNullOrEmpty(TargetFolder))
				throw new InvalidOperationException("Target folder is not set.");
			if (!Directory.Exists(TargetFolder))
				throw new InvalidOperationException("Target folder does not exist.");
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
		private bool tryResizeImage(string sourceFile)
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
				return ResizeImage(sourceFile, targetFile);
			}
			catch (OutOfMemoryException)
			{
				// Ignore and try next.
				return true;
			}
		}

		/// <summary>
		/// Gets the name of the target file based on the source file.
		/// </summary>
		/// <param name="sourceFile">The source file.</param>
		/// <returns>The name of the target file based on the source file.</returns>
		private string getTargetFileName(string sourceFile)
		{
			return Path.Combine(TargetFolder, Path.GetFileNameWithoutExtension(sourceFile)) + "." + FileExtension;
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
			string ext = "({0})." + FileExtension;
			int counter = 1;

			while (File.Exists(file + string.Format(ext, counter))) counter++;

			return file + string.Format(ext, counter);
		}

		/// <summary>
		/// Calculates the new size of the image.
		/// </summary>
		/// <param name="oldSize">The old size.</param>
		/// <returns>The new size of the image.</returns>
		protected virtual Size CalculateNewSize(Size oldSize)
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

		/// <summary>
		/// Gets or sets the quality of the resized images.
		/// </summary>
		/// <value></value>
		/// <remarks>This property is only applicable for codecs which
		/// takes a quality parameter, e.g. JPEG.</remarks>
		public abstract int Quality { get; set; }

		/// <summary>
		/// Gets the image format.
		/// </summary>
		/// <value>The image format.</value>
		public abstract string ImageFormat { get; }

		/// <summary>
		/// Gets the file extension.
		/// </summary>
		/// <value>The file extension.</value>
		protected abstract string FileExtension { get; }

		/// <summary>
		/// Resizes the image.
		/// </summary>
		/// <param name="sourceFile">The source file.</param>
		/// <param name="targetFile">The target file.</param>
		/// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
		/// <exception cref="OutOfMemoryException">If  The file does not have a valid image 
		/// format.-or- GDI+ does not support the pixel format of the file.</exception>
		protected abstract bool ResizeImage(string sourceFile, string targetFile);
	}
}
