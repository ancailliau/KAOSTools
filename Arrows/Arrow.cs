using System;
using Shapes;
using Cairo;
using Editor;
using Editor.Model;

namespace Arrows
{
	public class Arrow : IArrow
	{
		public Arrow ()
		{
		}
		
		public IShape Start {
			get ;
			set ;
		}

		public IShape End {
			get ;
			set ;
		}
		
		
		public Color StrokeColor {
			get;
			set;
		}
		
		public void Display (Context context, View view)
		{
			var drawingArea = view.DrawingArea;
			var startPosition = Start.GetAnchor(End.Position);
			var endPosition = End.GetAnchor(Start.Position);
			
			var oldSource = context.Source;
			
			context.SetSourceRGB(StrokeColor.R,
				StrokeColor.G,
				StrokeColor.B);
			
			context.MoveTo(startPosition);
			context.LineTo(endPosition);
			
			context.Stroke();
			
			context.Source = oldSource;
		}
	}
}

