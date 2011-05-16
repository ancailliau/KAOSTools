using System;
using KaosEditor.Controllers;
using KaosEditor.Model;

namespace Editor.Dialogs
{
	public partial class AddAgent : Gtk.Dialog
	{
		private MainController controller;
		
		public AddAgent  (MainController controller)
		{
			this.Build ();
			
			this.controller = controller;
		}
		
		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			string name = nameEntry.Text;
			if (name != null && name != "") {
				this.controller.Model.Add (new Agent(name));
				this.Destroy();
			}
		}
		
		protected virtual void OnButtonCancelClicked (object sender, System.EventArgs e)
		{
			this.Destroy ();
		}
	}
}

