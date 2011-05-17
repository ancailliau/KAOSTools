// 
// AddAgent.cs
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

namespace KaosEditor.UI.Dialogs
{
	/// <summary>
	/// Represents the dialog to add a new agent to the system
	/// </summary>
	public partial class AddAgent : Gtk.Dialog
	{
		
		/// <summary>
		/// The parent window.
		/// </summary>
		private MainWindow window;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="KaosEditor.UI.Dialogs.AddAgent"/> class.
		/// </summary>
		/// <param name='window'>
		/// Parent window.
		/// </param>
		public AddAgent  (MainWindow window)
		{
			this.Build ();
			this.DestroyWithParent = true;
			
			this.window = window;
		}
		
		/// <summary>
		/// Handles the button ok clicked event.
		/// </summary>
		/// <param name='sender'>
		/// Sender.
		/// </param>
		/// <param name='e'>
		/// E.
		/// </param>
		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			string name = nameEntry.Text;
			if (name != null && name != "") {
				this.window.Controller.Model.Add (new Agent(name));
				this.Destroy();
			}
		}
		
		/// <summary>
		/// Handles the button cancel clicked event.
		/// </summary>
		/// <param name='sender'>
		/// Sender.
		/// </param>
		/// <param name='e'>
		/// E.
		/// </param>
		protected virtual void OnButtonCancelClicked (object sender, System.EventArgs e)
		{
			this.Destroy ();
		}
	}
}

