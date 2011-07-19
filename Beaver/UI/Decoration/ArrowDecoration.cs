// 
// ArrowDecoration.cs
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

namespace Beaver.UI.Decoration
{
	public class ArrowDecoration : IDecoration
	{
		public double Position {
			get;
			set;
		}
		
		protected Func<double> getAngle;
		
		public string FillColor {
			get;
			set;
		}
		
		public string StrokeColor {
			get;
			set;
		}
		
		public ArrowDecoration (Func<double> getAngle)
		{
			this.getAngle = getAngle;
			FillColor = "#fff";
			StrokeColor = "#000";
		}
		
		private double haldAngle = Math.Atan2 (5, 10);
		private double sideWidth = Math.Sqrt (125);
		
		public void Render (Context cairoContext, Pango.Context pangoContext, double x, double y)
		{
			cairoContext.Save ();
			cairoContext.MoveTo (x, y);
			
			double alpha = getAngle ();
			
			double gamma = alpha - haldAngle;
			cairoContext.LineTo(x + sideWidth * Math.Cos(gamma), y + sideWidth * Math.Sin(gamma));
			
			double gamma2 = alpha + haldAngle;
			cairoContext.LineTo(x + sideWidth * Math.Cos(gamma2), y + sideWidth * Math.Sin(gamma2));
			cairoContext.ClosePath();
			
			cairoContext.SetColor (this.FillColor);
			cairoContext.FillPreserve ();
			
			cairoContext.SetColor (this.StrokeColor);
			cairoContext.Stroke();
		}
		
	}
}

