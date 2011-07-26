// 
// SimulationResult.cs
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

namespace Beaver
{
	public partial class SimulationResult : Gtk.Dialog
	{
		private double[] data;
		private int numSample;
		
		public SimulationResult (Gtk.Window window, double[] data, int numSample)
			: base ("Simulation results", window, Gtk.DialogFlags.DestroyWithParent)
		{
			this.Build ();
			
			this.data = data;
			this.numSample = numSample;
			
			this.summaryLabel.Markup = string.Format ("<b>Simulation results</b> ({0} {1})",
				numSample, numSample > 1 ? "samples" : "sample");
			
			this.drawingarea.ExposeEvent += HandleDrawingareahandleExposeEvent;			
			
			this.ShowAll ();
		}

		void HandleDrawingareahandleExposeEvent (object o, Gtk.ExposeEventArgs args)
		{
			var evnt = args.Event;
			using (Context context = Gdk.CairoHelper.Create(evnt.Window)) {
				context.LineWidth = 1;
				context.Translate (-0.5,-0.5);
				
				int numBuckets = data.Length;
				int daWidth, daHeight;
				this.drawingarea.GetSizeRequest(out daWidth, out daHeight);
				
				int padding = 10 * 2;
				int XaxisPadding = 35;
				daWidth -= padding;
				daHeight -= padding + XaxisPadding;
				
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
				
				context.MoveTo (padding/2f, daHeight + XaxisPadding - 16);
				context.RelLineTo (daWidth, 0);
				context.Stroke ();
				
				for (int i = 0; i < numBuckets + 1; i++) {
					
					double x = i * widthOfBucket;
					
					var pangoLayout = new Pango.Layout(this.drawingarea.PangoContext);
			
					pangoLayout.Alignment = Pango.Alignment.Center;
					
					int k = MainClass.Controller.Configuration.NumBuckets;
					double @double = Math.Round (Math.Log10 (k),0);
					pangoLayout.SetText(Math.Round (((double) i)/numBuckets, (int) @double).ToString ());
			
					var font = new Pango.FontDescription ();
					font.Size = (int) (8 * Pango.Scale.PangoScale);
					pangoLayout.FontDescription = font;
					
					int textWidth, textHeight;
					pangoLayout.GetPixelSize(out textWidth, out textHeight);
					
					context.MoveTo (start + x + textHeight / 2f, daHeight + XaxisPadding - padding / 2f);
					context.Rotate (Math.PI/2);
					Pango.CairoHelper.ShowLayout(context, pangoLayout);
					context.Rotate (-Math.PI/2);
					
				}
			}
		}

		protected void OnButtonCancelActivated (object sender, System.EventArgs e)
		{
			this.Destroy ();
		}

		protected void OnButtonCancelClicked (object sender, System.EventArgs e)
		{
			this.Destroy ();
		}
	}
}

