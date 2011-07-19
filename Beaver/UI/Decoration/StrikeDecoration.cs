// 
// StrikeDecoration.cs
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

namespace Beaver.UI.Decoration
{
	public class StrikeDecoration : IDecoration
	{
		public double Position {
			get;
			set;
		}
		
		private double distance = 15;
		private double delta = 3;
		private double width = 5;
		private Func<double> getAngle;
		
		public StrikeDecoration (Func<double> getAngle)
		{
			this.getAngle = getAngle;
		}

		public void Render (Cairo.Context cairoContext, Pango.Context pangoContext, double x, double y)
		{
			cairoContext.Save ();
			
			double alpha = getAngle ();
			
			cairoContext.MoveTo (
				x + (distance - delta / 2f) * Math.Cos(alpha) + width * Math.Sin(alpha), 
				y + (distance - delta / 2f) * Math.Sin(alpha) - width * Math.Cos(alpha));
			
			cairoContext.LineTo (
				x + (distance + delta / 2f) * Math.Cos(alpha) - width * Math.Sin(alpha), 
				y + (distance + delta / 2f) * Math.Sin(alpha) + width * Math.Cos(alpha));
			
			
			cairoContext.Restore ();
		}
	}
}

