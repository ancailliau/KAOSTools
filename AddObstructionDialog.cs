// 
// AddObstructionDialog.cs
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
using KaosEditor.UI.Windows;
using Gtk;
using KaosEditor.Model;

namespace KaosEditor
{
	public partial class AddObstructionDialog : Gtk.Dialog
	{
		private ListStore store;
		private MainWindow window;
		
		public string ObstacleName {
			get {
				if (Obstacle != null) {
					return Obstacle.Name;
				} else {
					return obstacleCombo.ActiveText;
				}
			}
		}
		
		public Obstacle Obstacle {
			get {
				var iter = new TreeIter();
				if (obstacleCombo.GetActiveIter(out iter)) {
					return (Obstacle) store.GetValue(iter, 1);
				} else {
					return null;
				}
			}
			set {
				UpdateList (value);
			}
		}
		
		public AddObstructionDialog (MainWindow window, Goal goal)
			: this (window, goal, null)
		{
		}
		
		public AddObstructionDialog (MainWindow window, Goal goal, Obstacle obstacle)
			: base (string.Format ("Add obstruction to '{0}'", goal.Name), 
				window, DialogFlags.DestroyWithParent)
		{
			this.Build ();
			this.window = window;
			
			store = new ListStore(typeof (string), typeof (Obstacle));
			obstacleCombo.Model = store;
			
			var cell = new CellRendererText();		
			// agentComboBox.PackStart(cell, false);
			obstacleCombo.AddAttribute(cell, "text", 0);
			
			UpdateList (obstacle);
		}

		void UpdateList (Obstacle obstacle)
		{
			TreeIter iter;
			foreach (var element in window.Controller.Model.Elements.FindAll(e => e is Obstacle)) {
				var possibleObstacle = (Obstacle) element;
				var possibleIter = store.AppendValues (possibleObstacle.Name, possibleObstacle);
				if (possibleObstacle == obstacle) {
					iter = possibleIter;
				}
			}
			obstacleCombo.SetActiveIter (iter);
		}
	}
}

