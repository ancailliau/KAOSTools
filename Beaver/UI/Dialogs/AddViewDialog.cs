// 
// AddView.cs
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
using Beaver.Controllers;
using Beaver.UI.Windows;
using Gtk;
using Beaver.Model;
using Beaver.Views;

namespace Beaver.UI.Dialogs
{

	/// <summary>
	/// Represents the dialog to add a view
	/// </summary>
	public partial class AddViewDialog : Gtk.Dialog
	{
		
		/// <summary>
		/// The parent window.
		/// </summary>
		private MainWindow window;
		
		public string ViewName {
			get {
				return nameEntry.Text.Trim ();
			}
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Beaver.UI.Dialogs.AddViewDialog"/> class.
		/// </summary>
		/// <param name='window'>
		/// Parent window.
		/// </param>
		public AddViewDialog (MainWindow window)
			: this (window, "")
		{
		}
		
		public AddViewDialog (MainWindow window, string viewName)
			: base ("Add view...", 
				window, DialogFlags.DestroyWithParent)
		{
			this.Build ();
			this.window = window;
			this.nameEntry.Text = viewName;
		}
		
		public AddViewDialog (MainWindow window, ModelView view)
			: base ("Edit view...", 
				window, DialogFlags.DestroyWithParent)
		{
			this.Build ();
			this.window = window;
			this.nameEntry.Text = view.Name;
		}
		
	}
}
