// 
// FilledArrow.cs
//  
// Author:
//       Antoine Cailliau <antoine.cailliau@uclouvain.be>
// 
// Copyright (c) 2011 2011 Université Catholique de Louvain and Antoine Cailliau
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using Cairo;
using KaosEditor.Model;
using KaosEditor.Views;

namespace KaosEditor.UI.Arrows
{
	
	/// <summary>
	/// Represents an arrow with a filled triangle at the end of the edge.
	/// </summary>
	public class FilledArrow : Arrow
	{
		/// <summary>
		/// Gets or sets the color of the fill.
		/// </summary>
		/// <value>
		/// The color of the fill.
		/// </value>
		public string FillColor {
			get;
			set;
		}
		
		/// <summary>
		/// The half angle for the arrow.
		/// </summary>
		private double arrowHalfAngle;
		
		/// <summary>
		/// The width of the side of the arrow.
		/// </summary>
		private double arrowSideWitdh;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="KaosEditor.Arrows.FilledArrow"/> class.
		/// </summary>
		public FilledArrow ()
		{
			arrowHalfAngle = Math.Atan2(5, 10);
			arrowSideWitdh = Math.Sqrt(125);
		}
		
		/// <summary>
		/// Display the arrow on the specified context and view.
		/// </summary>
		/// <param name='context'>
		/// Context.
		/// </param>
		/// <param name='view'>
		/// View.
		/// </param>
		public void Display (Context context, ModelView view)
		{
			base.Display(context, view);
			
			var startPosition = Start.GetAnchor(End.Position);
			var endPosition = End.GetAnchor(Start.Position);
			
			var oldSource = context.Source;
			
			context.MoveTo(endPosition);
			
			double xx = startPosition.X - endPosition.X;
			double yy = startPosition.Y - endPosition.Y;
			double alpha = Math.Atan2(yy, xx);
			
			double gamma = alpha - arrowHalfAngle;
			context.LineTo(endPosition.X + arrowSideWitdh * Math.Cos(gamma), endPosition.Y + arrowSideWitdh * Math.Sin(gamma));
			
			double gamma2 = alpha + arrowHalfAngle;
			context.LineTo(endPosition.X + arrowSideWitdh * Math.Cos(gamma2), endPosition.Y + arrowSideWitdh * Math.Sin(gamma2));
			context.ClosePath();
			
			
			context.SetColor (FillColor);
			context.FillPreserve ();
			
			context.SetColor ("#000");
			context.Stroke();
			
			
			context.Source = oldSource;
		}
		
	}
}

