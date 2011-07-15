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
using Beaver.Controllers;
using Beaver.Model;
using Beaver.UI.Windows;
using Gtk;

namespace Beaver.UI.Dialogs
{
	
	public partial class AddGoalDialog : Gtk.Dialog
	{
				
		#region Fields value
		
		public string GoalName {
			get {
				return nameEntry.Text.Trim();
			}
		}
		
		public string GoalDefinition {
			get {
				return definitionTextView.Buffer.Text.Trim();
			}
		}
		
		public float SoftThreshold {
			get { try {
					return Math.Max (0, Math.Min (1, float.Parse(softThresholdEntry.Text)));
				} catch (Exception e) {
					Logging.Logger.Error ("Soft threshold '{0}' cannot be parsed to float value ({1})", softThresholdEntry.Text, e.Message);
				}
				return 1;
			}
			set {
				softThresholdEntry.Text = value.ToString ();
			}
		}
		
		public float HardThreshold {
			get { try {
					return Math.Max (0, Math.Min (1, float.Parse(hardThresholdEntry.Text)));
				} catch (Exception e) {
					Logging.Logger.Error ("Hard threshold '{0}' cannot be parsed to float value ({1})", hardThresholdEntry.Text, e.Message);
				}
				return 1;
			}
			set {
				hardThresholdEntry.Text = value.ToString ();
			}
		}
		
		public bool AddToCurrentView {
			get { 
				return this.addToCurrentViewCheck.Active;
			}
			set {
				this.addToCurrentViewCheck.Active = value;
			}
		}
		
		#endregion
			
		public bool IsValid {
			get {
				return !string.IsNullOrEmpty (GoalName);
			}
		}
		
		public AddGoalDialog (MainWindow window, string futureGoalName)
			: this (window, new Goal (futureGoalName), false)
		{
		}
		
		public AddGoalDialog (MainWindow window, Goal goal, bool edit)
			: base (edit ? string.Format ("Edit goal '{0}'", goal) : "Add new goal", 
				window, DialogFlags.DestroyWithParent)
		{
			if (goal == null) 
				throw new ArgumentNullException ("goal");
			
			this.Build ();
			
			nameEntry.Text = goal.Name;
			definitionTextView.Buffer.Text = goal.Definition;
			likelihoodEntry.Text = goal.Likelihood.ToString ();
			SoftThreshold = goal.SoftThreshold;
			HardThreshold = goal.HardThreshold;
		}
		
	}
}

