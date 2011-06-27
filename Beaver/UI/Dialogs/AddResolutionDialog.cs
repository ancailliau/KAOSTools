// 
// AddResolutionDialog.cs
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
using Beaver.Model;
using Gtk;
using Beaver.UI.Windows;
using Beaver.Controllers;

namespace Beaver.UI.Dialogs
{
	public partial class AddResolutionDialog : Gtk.Dialog
	{
		private ListStore store;
		private MainController controller;
		
		public string ResolvingGoalName {
			get {
				if (ResolvingGoal == null) {
					return goalCombo.ActiveText;
				} else {
					return ResolvingGoal.Name;
				}
			}
		}
		
		public Goal ResolvingGoal {
			get {
				var iter = new TreeIter();
				if (goalCombo.GetActiveIter(out iter)) {
					return (Goal) store.GetValue(iter, 1);
				} else {
					return null;
				}
			}
			set {
				UpdateList (value);
			}
		}
		
		public AddResolutionDialog (MainController controller, Obstacle obstacle)
			: this (controller, obstacle, null)
		{
		}
		
		public AddResolutionDialog (MainController controller, Obstacle obstacle, Goal goal)
			: base ("Assign responsibility", controller.Window, DialogFlags.DestroyWithParent)
		{
			this.Build ();
			this.controller = controller;
			
			store = new ListStore(typeof (string), typeof (Goal));
			goalCombo.Model = store;
			
			var cell = new CellRendererText();		
			// agentComboBox.PackStart(cell, false);
			goalCombo.AddAttribute(cell, "text", 0);
			
			UpdateList (goal);
		}

		void UpdateList (Goal goal)
		{
			store.Clear ();
			TreeIter iter;
			foreach (var element in this.controller.GoalController.GetAll ()) {
				var possibleGoal = (Goal) element;
				var possibleIter = store.AppendValues (possibleGoal.Name, possibleGoal);
				if (possibleGoal == goal) {
					iter = possibleIter;
				}
			}
			goalCombo.SetActiveIter (iter);
		}
	}
}
