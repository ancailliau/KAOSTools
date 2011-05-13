using System;
using Model;
using Editor.Windows;

namespace Editor.Dialogs
{
	public partial class AddGoal : Gtk.Dialog
	{
		private MainWindow window;		
		
		public AddGoal (MainWindow window)
		{
			this.Build ();
			this.window = window;
		}
		
		protected virtual void OnButtonOkActivated (object sender, System.EventArgs e)
		{
			string name = nameEntry.Text;
			if (name != null && name != "") {
				this.window.Model.Add(new Goal(name));
				this.Destroy();
			}
		}
		
		protected virtual void OnButtonCancelActivated (object sender, System.EventArgs e)
		{
			this.Destroy();
		}
		
	}
}

