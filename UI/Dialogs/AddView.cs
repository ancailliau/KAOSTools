using System;
using KaosEditor.Controllers;

namespace Editor.Dialogs
{
	public partial class AddView : Gtk.Dialog
	{
		private MainController controller;
		
		public AddView (MainController controller)
		{
			this.Build ();
			
			this.controller = controller;
		}
		
		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			string name = nameEntry.Text;
			if (name != null && name != "") {
				this.controller.Model.Views.Add (name);
				this.Destroy();
			}
		}
		
		protected virtual void OnButtonCancelClicked (object sender, System.EventArgs e)
		{
			this.Destroy ();
		}
		
	}
}

