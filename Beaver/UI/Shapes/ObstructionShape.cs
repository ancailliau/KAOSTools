// 
// RefinementShape.cs
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
using System.Linq;
using Cairo;
using Beaver;
using Beaver.Model;
using Beaver.UI.Arrows;
using Beaver.Views;
using Beaver.UI.Decoration;

namespace Beaver.UI.Shapes
{
	
	public class ObstructionShape : LineShape
	{
		public ObstructionShape (Obstruction obstruction, PointD position) 
			: base (obstruction, position)
		{
			getStart = () => obstruction.Goal;
			getEnd = () => obstruction.Obstacle;
						
			decorations.Add (new RoundedBoxDecoration (() => {
				return string.Format ("{0:0.00}", obstruction.Likelihood);
			}) { Position = 0.65f });
			
			decorations.Add (new ArrowDecoration (() => {
				return Math.Atan2 (anchor1.Y - anchor2.Y, anchor1.X - anchor2.X) - Math.PI ;
			}) { Position = 0, FillColor = this.FillColor, StrokeColor = this.StrokeColor });
			
			decorations.Add (new StrikeDecoration (() => {
				return Math.Atan2 (anchor1.Y - anchor2.Y, anchor1.X - anchor2.X) - Math.PI ;
			}) { Position = 0 });
		}
	}
}

