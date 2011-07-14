// 
// AddObstacleDialog.cs
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
using Beaver.UI.Windows;
using Beaver.Model;

namespace Beaver.UI.Dialogs
{
	public partial class AddObstacleDialog : Gtk.Dialog
	{
		
		public string ObstacleName {
			get {
				return nameEntry.Text.Trim ();
			}
		}
		
		public string ObstacleDefinition {
			get {
				return definitionTextview.Buffer.Text.Trim ();
			}
		}
		
		public float Likelihood {
			get { return (float) Math.Min( Math.Max (likelihoodSpin.Value, 0), 1); }
			set { likelihoodSpin.Value = Math.Min( Math.Max (value, 0), 1); }
		}
		
		public AddObstacleDialog (MainWindow window)
			: this (window, "")
		{
		}
		
		public AddObstacleDialog (MainWindow window, string obstacleName)
			: base ("Add new obstacle", window, DialogFlags.DestroyWithParent)
		{
			this.Build ();
			
			nameEntry.Text = obstacleName;
		}
		
		public AddObstacleDialog (MainWindow window, Obstacle obstacle)
			: base (obstacle == null ? "Add new obstacle" : "Edit obstacle", window, DialogFlags.DestroyWithParent)
		{
			this.Build ();
			
			this.likelihoodText.Text = "Not computed";
			
			if (obstacle != null) {
				nameEntry.Text = obstacle.Name;
				definitionTextview.Buffer.Text = obstacle.Definition;
				Likelihood = obstacle.Likelihood;
				this.likelihoodText.Text = obstacle.ComputedLikelihood.ToString ();
			}
		}
	}
}

