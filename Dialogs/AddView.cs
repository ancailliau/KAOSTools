using System;
using Model;
using Editor.Model;
using Editor.Windows;

namespace Editor.Dialogs
{
	public partial class AddView : Gtk.Dialog
	{
		private MainWindow window;
		
		public AddView (MainWindow window)
		{
			this.Build ();
			
			this.window = window;
		}
		
		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			string name = nameEntry.Text;
			if (name != null && name != "") {
				this.window.Views.Add (name);
				this.Destroy();
			}
		}
		
		protected virtual void OnButtonCancelClicked (object sender, System.EventArgs e)
		{
			this.Destroy ();
		}
		
	}
}

