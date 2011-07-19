// 
// RoundedBoxDecoration.cs
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
	public class RoundedBoxDecoration : IDecoration
	{
		private Func<string> getValue;
	
		public string StrokeColor {
			get;
			set;
		}
		
		public string FillColor {
			get;
			set;
		}
		
		public string TextColor {
			get;
			set;
		}
		
		public double Position {
			get;
			set;
		}
		
		private int fontSize = 9;
		private int padding = 4;
		
		public RoundedBoxDecoration (Func<string> getValue)
		{
			this.getValue = getValue;
			
			StrokeColor = "#000";
			FillColor = "#fff";
			TextColor = "#000";
		}
		
		public void Render (Context context, Pango.Context pangoContext, double x, double y)
		{
			context.Save ();
			
			var pangoLayout = new Pango.Layout(pangoContext);
			pangoLayout.Alignment = Pango.Alignment.Center;
			pangoLayout.SetMarkup(getValue());
			
			var font = new Pango.FontDescription ();
			font.Size = (int) (fontSize * Pango.Scale.PangoScale);
			pangoLayout.FontDescription = font;
			
			int textWidth, textHeight;
			pangoLayout.GetPixelSize(out textWidth, out textHeight);
			
			context.MoveTo (
				x - textWidth / 2f - padding / 2f,
				y - textHeight / 2f  - padding / 2f);
			
			context.RoundedRectangle (
				x - textWidth / 2f - padding / 2f,
				y - textHeight / 2f - padding / 2f, 
				textWidth + padding, 
				textHeight + padding, 3);
			
			context.SetColor (StrokeColor);
			context.StrokePreserve ();
			context.SetColor (FillColor);
			context.Fill ();
			
			context.SetColor (TextColor);
			context.MoveTo(
				x - textWidth / 2f,
				y - textHeight / 2f);
			Pango.CairoHelper.ShowLayout(context, pangoLayout);
			
			context.Restore ();
		}
		
	}
}

