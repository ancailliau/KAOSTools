// 
// HistogramArea.cs
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
using Gtk;
using Cairo;

namespace Beaver.UI.Widgets
{
	public class HistogramArea : DrawingArea
	{
		private double[] data;
		
		private int padding = 10 * 2;
		private int xAxisPadding = 35;
		
		public HistogramArea (double[] data)
		{
			this.data = data;
		}
		
		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			using (Context context = Gdk.CairoHelper.Create(evnt.Window)) {
			
				context.SetColor("#fff");
				context.Paint();
				
				context.LineWidth = 1;
				context.Translate (-0.5,-0.5);
				
				int numBuckets = data.Length;
				int daWidth, daHeight;
				this.GetSizeRequest(out daWidth, out daHeight);
				
				daWidth -= padding;
				daHeight -= padding + xAxisPadding;
				
				double widthOfBucket = ((double) daWidth) / numBuckets;
				double heightScale = ((double) daHeight) / data.Max();
				
				double start = padding / 2f;
				if (numBuckets > 0) { 
					double y = data[0] * heightScale;					
					context.MoveTo (start, daHeight - y + start);
				}
				
				for (int i = 0; i < numBuckets; i++) {
					double x = i * widthOfBucket;
					double y = data[i] * heightScale;
					
					context.Rectangle (
						(int) (start + x), 
						(int) (daHeight + start),
						(int) widthOfBucket, (int) - y);
				}
				context.SetColor ("#729fcf");
				context.FillPreserve ();
				context.SetColor ("#000");
				context.Stroke();
				
				context.MoveTo (padding/2f, daHeight + xAxisPadding - 16);
				context.RelLineTo (daWidth, 0);
				context.Stroke ();
				
				for (int i = 0; i < numBuckets + 1; i++) {
					
					double x = i * widthOfBucket;
					
					var pangoLayout = new Pango.Layout(this.PangoContext);
			
					pangoLayout.Alignment = Pango.Alignment.Center;
					
					int k = MainClass.Controller.Configuration.NumBuckets;
					double @double = Math.Round (Math.Log10 (k),0);
					pangoLayout.SetText(Math.Round (((double) i)/numBuckets, (int) @double).ToString ());
			
					var font = new Pango.FontDescription ();
					font.Size = (int) (8 * Pango.Scale.PangoScale);
					pangoLayout.FontDescription = font;
					
					int textWidth, textHeight;
					pangoLayout.GetPixelSize(out textWidth, out textHeight);
					
					context.MoveTo (start + x + textHeight / 2f, daHeight + xAxisPadding - padding / 2f);
					context.Rotate (Math.PI/2);
					Pango.CairoHelper.ShowLayout(context, pangoLayout);
					context.Rotate (-Math.PI/2);
					
				}
			}
			
			return true;
		}
	}
}

