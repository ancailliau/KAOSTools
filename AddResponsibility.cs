// 
// AddResponsibility.cs
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
using KaosEditor.Model;
using Gtk;
using KaosEditor.UI.Windows;
using KaosEditor.UI;

namespace KaosEditor
{
	
	/// <summary>
	/// Represents the dialog to add a responsibility link.
	/// </summary>
	public partial class AddResponsibility : Gtk.Dialog
	{
		/// <summary>
		/// The store.
		/// </summary>
		private ListStore store;
		
		public Agent ResponsibleAgent {
			get {
				var iter = new TreeIter();
				if (agentComboBox.GetActiveIter(out iter)) {
					return (Agent) store.GetValue(iter, 1);
				} else {
					return null;
				}
			}
		}
		
		public string ResponsibilityName {
			get {
				return nameEntry.Text;
			}
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="KaosEditor.AddResponsibility"/> class.
		/// </summary>
		/// <param name='goal'>
		/// Goal.
		/// </param>
		public AddResponsibility (MainWindow window, Goal goal)
			: base ("Assign responsibility", window, DialogFlags.DestroyWithParent)
		{
			this.Build ();
			nameEntry.Text = "Responsibility assignement";
			
			store = new ListStore(typeof (string), typeof (Agent));
			agentComboBox.Model = store;
			
			var cell = new CellRendererText();		
			// agentComboBox.PackStart(cell, false);
			agentComboBox.AddAttribute(cell, "text", 0);
			
			foreach (var element in window.Controller.Model.Elements.FindAll(e => e is Agent)) {
				var agent = (Agent) element;
				store.AppendValues (agent.Name, agent);
			}
		}
	}
}

