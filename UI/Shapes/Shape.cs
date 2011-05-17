using Cairo;
using KaosEditor.Model;

namespace KaosEditor.UI.Shapes
{
	
	/// <summary>
	/// Represents an abstract shape.
	/// </summary>
	public abstract class Shape : IShape
	{
		
		/// <summary>
		/// Gets or sets the position.
		/// </summary>
		/// <value>
		/// The position.
		/// </value>
		public PointD Position {
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the X padding.
		/// </summary>
		/// <value>
		/// The X padding.
		/// </value>
		protected double XPadding {
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the Y padding.
		/// </summary>
		/// <value>
		/// The Y padding.
		/// </value>
		protected double YPadding {
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the represented element.
		/// </summary>
		/// <value>
		/// The represented element.
		/// </value>
		public IModelElement RepresentedElement {
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the depth.
		/// </summary>
		/// <value>
		/// The depth.
		/// </value>
		public int Depth {
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="KaosEditor.UI.Shapes.Shape"/> is selected.
		/// </summary>
		/// <value>
		/// <c>true</c> if selected; otherwise, <c>false</c>.
		/// </value>
		public bool Selected {
			get ;
			set ;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="KaosEditor.UI.Shapes.Shape"/> class.
		/// </summary>
		public Shape ()
		{
			Position = new PointD(0,0);
		}		
		
		/// <summary>
		/// Display the shape on the specified context and view.
		/// </summary>
		/// <param name='context'>
		/// Context.
		/// </param>
		/// <param name='view'>
		/// View.
		/// </param>
		public abstract void Display (Context context, View view);
		
		/// <summary>
		/// Determines whether coordinates are in the form.
		/// </summary>
		/// <returns>
		/// The bounding box.
		/// </returns>
		/// <param name='x'>
		/// If set to <c>true</c> x.
		/// </param>
		/// <param name='y'>
		/// If set to <c>true</c> y.
		/// </param>
		/// <param name='delta'>
		/// If set to <c>true</c> delta.
		/// </param>
		public abstract bool InBoundingBox (double x, double y, out PointD delta);
		
		/// <summary>
		/// Gets the anchor corresponding for the given point.
		/// </summary>
		/// <returns>
		/// The anchor.
		/// </returns>
		/// <param name='point'>
		/// Point.
		/// </param>
		public abstract PointD GetAnchor (PointD point);
	}
}

