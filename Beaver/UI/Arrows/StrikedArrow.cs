// 
// StrikedArrow.cs
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
using Beaver.Model;
using Beaver.Views;

namespace Beaver.UI.Arrows
{
	public class StrikedArrow : FilledArrow
	{
		public StrikedArrow ()
			: base ()
		{
		}
		
		public void Display (Context context, ModelView view)
		{
			/*
			base.Display (context, view);
			
			var startPosition = Start.GetAnchor(End.Position);
			var endPosition = End.GetAnchor(Start.Position);
			
			var distanceFromPoint = 15;
			var width = 5;
			
			double xx = startPosition.X - endPosition.X;
			double yy = startPosition.Y - endPosition.Y;
			double alpha = Math.Atan2(yy, xx);
			
			context.MoveTo (endPosition.X + distanceFromPoint * Math.Cos(alpha) + width * Math.Sin(alpha), 
				endPosition.Y + distanceFromPoint * Math.Sin(alpha) - width * Math.Cos(alpha));
			
			context.LineTo (endPosition.X + distanceFromPoint * Math.Cos(alpha) - width * Math.Sin(alpha), 
				endPosition.Y + distanceFromPoint * Math.Sin(alpha) + width * Math.Cos(alpha));
			context.Stroke ();
			*/
		}
	}
}

