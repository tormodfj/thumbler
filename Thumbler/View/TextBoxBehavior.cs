using System.Windows;
using System.Windows.Controls;

namespace Thumbler.View
{
    /// <summary>
    /// Contains attached behaviors for text boxes.
    /// </summary>
    /// <remarks>
    /// The concept of attached behaviors is explained on:
    /// http://www.codeproject.com/KB/WPF/AttachedBehaviors.aspx
    /// </remarks>
    public static class TextBoxBehavior
    {
        /// <summary>
        /// Gets the value of the <c>SelectAllOnFocus</c> attached property.
        /// </summary>
        /// <param name="obj">The object that the property is attached to.</param>
        /// <returns>The value of the <c>SelectAllOnFocus</c> attached property.</returns>
        public static bool GetSelectAllOnFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectAllOnFocusProperty);
        }

        /// <summary>
        /// Sets the value of the <c>SelectAllOnFocus</c> attached property.
        /// </summary>
        /// <param name="obj">The object that the property is attached to.</param>
        /// <param name="value">if set to <c>true</c> on a <see cref="TextBox"/>, all
        /// text is selected when it gets focus.</param>
        public static void SetSelectAllOnFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectAllOnFocusProperty, value);
        }

        /// <summary>
        /// The backing store for the <c>SelectAllOnFocus</c> attached property.
        /// </summary>
        /// <remarks>This is a dependency property.</remarks>
        public static readonly DependencyProperty SelectAllOnFocusProperty =
            DependencyProperty.RegisterAttached(
                "SelectAllOnFocus",
                typeof(bool),
                typeof(TextBoxBehavior),
                new UIPropertyMetadata(false, OnSelectAllOnFocusChanged)
            );

        /// <summary>
        /// Called when the value of a <c>SelectAllOnFocus</c> attached property
        /// changes.
        /// </summary>
        /// <param name="depObj">The object that the property is attached to.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> 
        /// instance containing the event data.</param>
        private static void OnSelectAllOnFocusChanged(
            DependencyObject depObj,
            DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = depObj as TextBox;

            if (textBox != null && e.NewValue is bool)
            {
                if ((bool)e.NewValue)
                    textBox.GotFocus += TextBox_SelectAllOnFocus;
                else
                    textBox.GotFocus -= TextBox_SelectAllOnFocus;
            }
        }

        /// <summary>
        /// Handles the SelectAllOnFocus event of a TextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance 
        /// containing the event data.</param>
        private static void TextBox_SelectAllOnFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }
    }
}
