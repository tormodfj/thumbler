using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Thumbler.Model
{
	/// <summary>
	/// Class for resizing a collection of images to a specified size,
	/// using the JPEG codec.
	/// </summary>
	class JpegImageResizer : ImageResizerBase
	{
		private static readonly ImageCodecInfo codec;

		/// <summary>
		/// Initializes the <see cref="JpegImageResizer"/> class.
		/// </summary>
		static JpegImageResizer()
		{
			codec = getJpegCodec();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JpegImageResizer"/> class.
		/// </summary>
		public JpegImageResizer()
		{
			Quality = 70;
		}

		/// <summary>
		/// Gets the JPEG codec.
		/// </summary>
		/// <returns>The JPEG codec.</returns>
		/// <exception cref="NotSupportedException">If no JPEG coded was
		/// found on the system.</exception>
		private static ImageCodecInfo getJpegCodec()
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
		/// Gets or sets the quality of the resized images.
		/// </summary>
		/// <remarks>This property is only applicable for codecs which
		/// takes a quality parameter, e.g. JPEG.</remarks>
		public override int Quality { get; set; }

		/// <summary>
		/// Gets the image format.
		/// </summary>
		/// <value>The image format.</value>
		public override string ImageFormat
		{
			get { return "JPEG"; }
		}

		/// <summary>
		/// Gets the file extension.
		/// </summary>
		/// <value>The file extension.</value>
		protected override string FileExtension
		{
			get { return "jpg"; }
		}

		/// <summary>
		/// Validates the properties.
		/// </summary>
		/// <exception cref="InvalidOperationException">If properties are not
		/// valid for the resize operation.</exception>
		protected override void ValidatePropertiesBeforeResize()
		{
			base.ValidatePropertiesBeforeResize();

			if (Quality < 20 || Quality > 100)
				throw new InvalidOperationException("Quality must be in range [20, 100].");
		}

		/// <summary>
		/// Resizes the image.
		/// </summary>
		/// <param name="sourceFile">The source file name.</param>
		/// <param name="targetFile">The target file name.</param>
		/// <returns>
		/// 	<c>true</c> if successful; otherwise, <c>false</c>.
		/// </returns>
		protected override bool ResizeImage(string sourceFile, string targetFile)
		{
			using (Image sourceImage = Image.FromFile(sourceFile))
			{
				Size newSize = CalculateNewSize(sourceImage.Size);
				EncoderParameters parameters = new EncoderParameters
				{
					Param = new EncoderParameter[] { new EncoderParameter(Encoder.Quality, Quality) }
				};
				using (Bitmap newImage = new Bitmap(sourceImage, newSize))
				{
					newImage.Save(targetFile, codec, parameters);
				}
				return true;
			}
		}
	}
}
