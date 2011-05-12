using System;
using Model;
using Editor.Model;

namespace Editor.Dialogs
{
	public partial class AddView : Gtk.Dialog
	{
		private GoalModel model;
		private Views views;
		
		public AddView (GoalModel model, Views views)
		{
			this.Build ();
			
			this.model = model;
			this.views = views;
		}
		
		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			string name = nameEntry.Text;
			if (name != null && name != "") {
				views.Add (name);
				this.Destroy();
			}
		}
		
		protected virtual void OnButtonCancelClicked (object sender, System.EventArgs e)
		{
			this.Destroy();
		}
		
	}
}

