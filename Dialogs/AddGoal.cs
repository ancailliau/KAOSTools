using System;
using Model;

namespace Editor.Dialogs
{
	public partial class AddGoal : Gtk.Dialog
	{
		private GoalModel model;		
		
		public AddGoal (GoalModel model)
		{
			this.Build ();
			this.model = model;
		}
		
		protected virtual void OnButtonOkActivated (object sender, System.EventArgs e)
		{
			string name = nameEntry.Text;
			if (name != null && name != "") {
				this.model.Add(new Goal(name));
				this.Destroy();
			}
		}
		
		protected virtual void OnButtonCancelActivated (object sender, System.EventArgs e)
		{
			this.Destroy();
		}
		
	}
}

