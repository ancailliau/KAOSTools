using System;
using Cairo;
using Gtk;

namespace Shapes
{
	public interface IShape
	{
		/// <summary>
		/// Gets or sets the label of the shape.
		/// </summary>
		/// <value>
		/// The label.
		/// </value>
		string Label { get ; set ; }
		
		/// <summary>
		/// Gets or sets the top left point of the shape.
		/// </summary>
		/// <value>
		/// The top left.
		/// </value>
		PointD TopLeft { get ; set ; }
		
		/// <summary>
		/// Gets or sets the color of the border.
		/// </summary>
		/// <value>
		/// The color of the border.
		/// </value>
		Color BorderColor { get ; set ; }
		
		/// <summary>
		/// Gets or sets the color of the background.
		/// </summary>
		/// <value>
		/// The color of the background.
		/// </value>
		Color BackgroundColor { get ; set ; }
		
		/// <summary>
		/// Gets or sets the padding for the x-axis.
		/// </summary>
		/// <value>
		/// The X padding.
		/// </value>
		double XPadding { get ; set ; }
		
		/// <summary>
		/// Gets or sets the padding for the y-axis.
		/// </summary>
		/// <value>
		/// The Y padding.
		/// </value>
		double YPadding { get ; set ; }
		
		/// <summary>
		/// Display the current shape on the specified context and drawing area.
		/// </summary>
		/// <param name='context'>
		/// Context.
		/// </param>
		/// <param name='drawingArea'>
		/// Drawing area.
		/// </param>
		void Display (Context context, DrawingArea drawingArea);
		
		/// <summary>
		/// Returns whether the point determined by the coordinates (x,y)
		/// are in the current shape.
		/// </summary>
		/// <returns>
		/// true if the point is in the shape, false otherwise.
		/// </returns>
		/// <param name='x'>
		/// The x-coordinate of the point
		/// </param>
		/// <param name='y'>
		/// The y-coordinate of the point
		/// </param>
		/// <param name='delta'>
		/// The distance between the point and top-left anchor.
		/// </param>
		bool InBoundingBox (double x, double y, out PointD delta);

	}
}

