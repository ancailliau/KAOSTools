using System;

namespace Editor
{
	public partial class TextEntryDialog : Gtk.Dialog
	{
		
		public delegate bool OnConfirm (string answers);
		
		private OnConfirm handler;		
		
		public TextEntryDialog (string question, OnConfirm handler)
		{
			this.Build ();
			this.handler = handler;
			questionLabel.Text = question;
		}
		
		protected virtual void OnButtonCancelClicked (object sender, System.EventArgs e)
		{
			this.Destroy();
		}
		
		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			if (this.handler(answerEntry.Text)) {
				this.Destroy();
			}
		}
		
		
	}
}

