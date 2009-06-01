namespace Thumbler.Model
{
	/// <summary>
	/// Represents an object which can resize a collection of images based
	/// on an additional quality parameter.
	/// </summary>
	public interface IQualityAdjustableImageResizer : IImageResizer
	{
		/// <summary>
		/// Gets or sets the quality of the resized images.
		/// </summary>
		int Quality { get; set; }
	}
}
