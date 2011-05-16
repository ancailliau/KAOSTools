using System;
using Model;
using Editor.Windows;
using Editor.Controllers;

namespace Editor.Dialogs
{
	public partial class AddGoal : Gtk.Dialog
	{
		private MainController controller;		
		
		public AddGoal (MainController controller)
		{
			this.Build ();
			this.controller = controller;
		}
		
		protected virtual void OnButtonOkActivated (object sender, System.EventArgs e)
		{
			string name = nameTextView.Buffer.Text;
			if (name != null && name != "") {
				this.controller.Model.Add(new Goal(name));
				this.Destroy();
			}
		}
		
		protected virtual void OnButtonCancelActivated (object sender, System.EventArgs e)
		{
			this.Destroy();
		}
		
	}
}

