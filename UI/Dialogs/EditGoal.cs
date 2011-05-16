using System;
using KaosEditor.Controllers;
using KaosEditor.Model;

namespace Editor.Dialogs
{
	public partial class EditGoal : Gtk.Dialog
	{
		private MainController controller;
		private Goal goal;
		
		public EditGoal (MainController controller, Goal goal)
		{
			this.Build ();
			this.Title = string.Format("Edit {0}", goal.Name);
			
			this.controller = controller;
			this.goal = goal;
			
			nameTextView.Buffer.Text = this.goal.Name;
		}
		
		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			if (nameTextView.Buffer.Text != "") {
				this.goal.Name = nameTextView.Buffer.Text;
				this.controller.Model.NotifyChange();
				this.controller.Model.Views.NotifyViewsChanged();
				this.Destroy();
			}
		}
		
		protected virtual void OnButtonCancelClicked (object sender, System.EventArgs e)
		{
			this.Destroy();
		}
		
		
	}
}

