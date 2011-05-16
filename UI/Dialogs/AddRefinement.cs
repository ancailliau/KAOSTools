using System;
using System.Collections.Generic;
using Gtk;
using KaosEditor.Controllers;
using KaosEditor.Model;

namespace Editor.Dialogs
{
	public partial class AddRefinement : Gtk.Dialog
	{
		
		private MainController controller;
		private ListStore childrenComboStore;
		private ListStore childrenNodeStore;
		
		private List<Goal> refinees;
		private Goal parentGoal;
		
		public AddRefinement (MainController controller, Goal parent)
		{
			this.Build ();
			
			this.parentGoal = parent;
			this.refinees = new List<Goal> ();
			
			this.Title = string.Format("Refine goal {0}", parent.Name);
			
			this.controller = controller;
			
			childrenComboStore = new ListStore(typeof(string), typeof(object));
			childrenComboBox.Model = childrenComboStore;
			
			childrenNodeStore = new ListStore (typeof(string), typeof(object));
			childrenNodeView.Model = childrenNodeStore;
			
			CellRendererText cell = new CellRendererText ();
			var col = new TreeViewColumn();
			col.Title = "Children";
			col.PackStart(cell, true);
			col.AddAttribute(cell, "text", 0);
			
			childrenNodeView.AppendColumn(col);
			childrenNodeView.HeadersVisible = false;
			
			cell = new CellRendererText();
			childrenComboBox.PackStart(cell, false);
			childrenComboBox.AddAttribute(cell, "text", 0);
			
			foreach (var g in controller.Model.Elements.FindAll(e => e is Goal)) {
				if (g != parent) {
					childrenComboStore.AppendValues(g.Name, g as Goal);
				}
			}
		}
		
		protected virtual void OnAddButtonActivated (object sender, System.EventArgs e)
		{
			TreeIter iter = new TreeIter();
			
			if (childrenComboBox.GetActiveIter(out iter)) {
				Console.WriteLine ("Add element");
				
				// Get the element
				var element = (Goal) childrenComboStore.GetValue(iter, 1);
				
				this.refinees.Add(element);
				childrenNodeStore.AppendValues(element.Name, element);
			}
		}
		
		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			string name = nameTextView.Buffer.Text.Trim();
			if (this.refinees.Count > 0 && name != "") {
				var refinement = new Refinement(name);
				refinement.Refined = this.parentGoal;
				this.parentGoal.Refinements.Add(refinement);
				foreach (var refinee in this.refinees) {
					refinement.Refinees.Add(refinee);
				}
				this.controller.Model.Add(refinement);
				
				this.Destroy();
			}
		}
		
		protected virtual void OnButtonCancelClicked (object sender, System.EventArgs e)
		{
			this.Destroy();
		}
		
	}
}

