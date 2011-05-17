// 
// TabLabel.cs
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
using Gtk;
using KaosEditor.Logging;

namespace KaosEditor.UI.Widgets
{
	
	/// <summary>
	/// Represents the label of a tab page.
	/// </summary>
	public class TabLabel : HBox
	{
		/// <summary>
		/// The close image pixbuf.
		/// </summary>
		private static Gdk.Pixbuf closePixbuf;
		
		/// <summary>
		/// Initializes the <see cref="KaosEditor.UI.Widgets.TabLabel"/> class.
		/// </summary>
		static TabLabel () {
			try {
				closePixbuf = Gdk.Pixbuf.LoadFromResource("KaosEditor.Images.Close.png");
			} catch (Exception e) {
				Logger.Warning ("Cannot load 'KaosEditor.Images.Close.png' from ressources", e);
			}
		}
		
		/// <summary>
		/// Handler for the clicked event.
		/// </summary>
		public delegate void CloseClickedHandler (object sender, EventArgs args);
		
		/// <summary>
		/// Occurs when close clicked.
		/// </summary>
		public event CloseClickedHandler CloseClicked;
		
		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>
		/// The text.
		/// </value>
		public string Text {
			get;
			private set;
		}
		
		/// <summary>
		/// The label.
		/// </summary>
		private Label label;
		
		/// <summary>
		/// The button.
		/// </summary>
		private Button button;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="KaosEditor.UI.Widgets.TabLabel"/> class.
		/// </summary>
		/// <param name='name'>
		/// Name.
		/// </param>
		public TabLabel (string name)
		{
			this.Text = name;
			label = new Label(name);
			
			// Ugly stuff to get rid of some noisy padding
			Gtk.Rc.ParseString ("style \"KaosEditor.ViewsNotebook.CloseButton\" {\n GtkButton::inner-border = {0,0,0,0}\n }\n");
      		Gtk.Rc.ParseString ("widget \"*.KaosEditor.ViewsNotebook.CloseButton\" style  \"KaosEditor.ViewsNotebook.CloseButton\"\n");
			
			button = new Button();
			button.Relief = ReliefStyle.None;
			button.Image = new Image(closePixbuf);
			button.Name = "KaosEditor.ViewsNotebook.CloseButton";
			button.Clicked += delegate(object sender, EventArgs e) {
				if (CloseClicked != null) {
					CloseClicked (this, EventArgs.Empty);
				}
			};
			
			this.PackStart(label, true, true, 0);
			this.PackEnd(button, false, false, 0);
			this.ShowAll();
		}
	}
}

