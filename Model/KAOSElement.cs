// 
// KAOSElement.cs
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
using KaosEditor.UI;
using Gtk;

namespace KaosEditor.Model
{
	public abstract class KAOSElement : IModelElement, IContextMenu
	{
	
		public string Id {
			get;
			set;
		}
		
		public string Name {
			get;
			set;
		}
		
		public virtual void PopulateContextMenu (Gtk.Menu menu, MenuContext context)
		{
			if (!(context.Initiator is DrawingArea)
				&& context.Controller.Window.HasCurrentView()) {
				
				var addToCurrentView = new MenuItem("Add to current view");
				addToCurrentView.Activated += delegate(object sender2, EventArgs e) {
					context.Controller.Window.AddToCurrentView (this);
				};
				menu.Add(addToCurrentView);
			}
		}
		
	}
}

