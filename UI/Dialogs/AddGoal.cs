// 
// AddGoal.cs
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
using KaosEditor.Controllers;
using KaosEditor.Model;
using KaosEditor.UI.Windows;
using Gtk;

namespace KaosEditor.UI.Dialogs
{
	
	/// <summary>
	/// Represents the dialog to add a new goal
	/// </summary>
	public partial class AddGoal : Gtk.Dialog
	{
		
		/// <summary>
		/// The parent window.
		/// </summary>
		private MainWindow window;		
		
		private MenuContext context;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="KaosEditor.UI.Dialogs.AddGoal"/> class.
		/// </summary>
		/// <param name='window'>
		/// Parent window.
		/// </param>
		public AddGoal (MainWindow window, MenuContext context)
			: base ("Add new goal", window, DialogFlags.DestroyWithParent)
		{
			this.Build ();
			this.window = window;
			this.context = context;
		}
		
		/// <summary>
		/// Handles the button ok activated event.
		/// </summary>
		/// <param name='sender'>
		/// Sender.
		/// </param>
		/// <param name='e'>
		/// E.
		/// </param>
		protected virtual void OnButtonOkActivated (object sender, System.EventArgs e)
		{
			string name = nameTextView.Buffer.Text;
			if (name != null && name != "") {
				Goal goal = new Goal (name);
				this.window.Controller.Model.Add(goal);
				if (context.Initiator is DrawingArea) {
					this.window.AddToCurrentView (goal, context.ClickedPoint.X, context.ClickedPoint.Y);
				}
				this.Destroy();
			}
		}
		
		/// <summary>
		/// Handles the button cancel activated event.
		/// </summary>
		/// <param name='sender'>
		/// Sender.
		/// </param>
		/// <param name='e'>
		/// E.
		/// </param>
		protected virtual void OnButtonCancelActivated (object sender, System.EventArgs e)
		{
			this.Destroy();
		}
		
	}
}

