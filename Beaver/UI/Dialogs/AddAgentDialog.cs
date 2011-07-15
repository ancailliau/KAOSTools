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
using Beaver.Controllers;
using Beaver.Model;
using Beaver.UI.Windows;
using Gtk;

namespace Beaver.UI.Dialogs
{
	/// <summary>
	/// Represents the dialog to add a new agent to the system
	/// </summary>
	public partial class AddAgentDialog : Gtk.Dialog
	{
		
		#region Values of fields
		
		public string AgentName {
			get { return nameEntry.Text.Trim(); }
		}
		
		#endregion
		
		public AddAgentDialog  (MainWindow window, string agentName)
			: this (window, new Agent (agentName), false)
		{
			this.Build ();
			nameEntry.Text = agentName;
		}
		
		public AddAgentDialog  (MainWindow window, Agent agent, bool edit)
			: base (edit ? string.Format ("Edit agent '{0}'", agent.Name) : "Add new agent", 
				window, DialogFlags.DestroyWithParent)
		{
			if (agent == null) 
				throw new ArgumentNullException ("agent");
			
			this.Build ();
			
			nameEntry.Text = agent.Name;
		}
	}
}

