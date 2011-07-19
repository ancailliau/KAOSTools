// 
// Shape.cs
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
using Beaver.Model;
using Cairo;
using Beaver.UI.Decoration;
using System.Collections.Generic;
using Beaver.Views;

namespace Beaver.UI.Shapes
{
	public abstract class Shape
	{
		
		protected IList<IDecoration> decorations = new List<IDecoration> ();
		
		protected Func<string> getContent;
		
		protected double height;
		protected double width;
		protected int maxWidth = 150;
		
		protected PointD position;
		public PointD Position {
			get { return position; }
			set { position = value; }
		}
		
		protected KAOSElement element;
		public KAOSElement RepresentedElement {
			get { return element; }
		}
		
		public bool Selected {
			get ;
			set ;
		}
		
		protected string FillColor {
			get {
				string key = this.RepresentedElement == null ? this.GetType().Name + "FillColor" : this.RepresentedElement.GetType().Name + "FillColor";
				return MainClass.Controller.CurrentColorScheme.Get (key);
			}
		}
		
		protected string StrokeColor {
			get {
				string key = this.RepresentedElement == null ? this.GetType().Name + "StrokeColor" : this.RepresentedElement.GetType().Name + "StrokeColor";
				return MainClass.Controller.CurrentColorScheme.Get (key);
			}
		}
		
		protected string TextColor {
			get {
				string key = this.RepresentedElement == null ? this.GetType().Name + "TextColor" : this.RepresentedElement.GetType().Name + "TextColor";
				return MainClass.Controller.CurrentColorScheme.Get (key);
			}
		}
		
		public Shape (KAOSElement element, PointD position)
		{
			this.element = element;
			this.position = position;
			this.getContent = () => "";
		}
		
		
		// TODO rename
		public abstract IQueryable<Func<PointD>> getAnchors (ModelView view);
		
		public void GetAnchors (Shape s, out PointD anchor1, out PointD anchor2, ModelView view) {
			var anchors = (from p1 in this.getAnchors(view).Select ((arg) => arg())
				from p2 in s.getAnchors(view).Select ((arg) => arg())
				orderby (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y)
				select new {
					anchor1 = p1,
					anchor2 = p2
				}).First ();
			anchor1 = anchors.anchor1;
			anchor2 = anchors.anchor2;
		}
				
		public abstract Bounds GetBounds (ModelView view);
		
		public abstract bool InBoundingBox (double x, double y, out PointD delta, ModelView view);
		
		public abstract void AbstractDisplay (Context cairoContext, Pango.Context pangoContext, ModelView view);
	}
}

