// 
// Parallelogram.cs
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

namespace Beaver.UI.Shapes
{
	public abstract class Parallelogram : PolygonalShape
	{
		protected int shear = 4;
		
		public Parallelogram (KAOSElement element)
			: this (element, new PointD(0,0))
		{}
		
		public Parallelogram (KAOSElement element, PointD position)
			: base (element, position)
		{
			xPadding = 10;
			yPadding = 4;
			
			points.Add (() => new PointD (Position.X - width / 2f + shear / 2f,	Position.Y - height / 2f	));
			points.Add (() => new PointD (Position.X + width / 2f + shear / 2f,	Position.Y - height / 2f	));
			points.Add (() => new PointD (Position.X + width / 2f - shear / 2f,	Position.Y + height / 2f	));
			points.Add (() => new PointD (Position.X - width / 2f - shear / 2f,	Position.Y + height / 2f	));
			
			anchors.Add (() => new PointD (Position.X - width / 2f, 				Position.Y					));
			anchors.Add (() => new PointD (Position.X - shear / 2f,					Position.Y + height / 2f	));
			anchors.Add (() => new PointD (Position.X + width / 2f, 				Position.Y					));
			anchors.Add (() => new PointD (Position.X + shear / 2f, 				Position.Y - height / 2f	));
		}
	}
}

