
// This file has been generated by the GUI designer. Do not modify.
namespace KaosEditor.UI.Dialogs
{
	public partial class TextEntryDialog
	{
		private global::Gtk.VBox vbox3;
		private global::Gtk.Label questionLabel;
		private global::Gtk.Entry answerEntry;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;
        
		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget KaosEditor.UI.Dialogs.TextEntryDialog
			this.Name = "KaosEditor.UI.Dialogs.TextEntryDialog";
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			this.DestroyWithParent = true;
			// Internal child KaosEditor.UI.Dialogs.TextEntryDialog.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			this.vbox3.BorderWidth = ((uint)(6));
			// Container child vbox3.Gtk.Box+BoxChild
			this.questionLabel = new global::Gtk.Label ();
			this.questionLabel.Name = "questionLabel";
			this.questionLabel.Xalign = 0F;
			this.questionLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("label6");
			this.vbox3.Add (this.questionLabel);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.questionLabel]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.answerEntry = new global::Gtk.Entry ();
			this.answerEntry.CanFocus = true;
			this.answerEntry.Name = "answerEntry";
			this.answerEntry.IsEditable = true;
			this.answerEntry.InvisibleChar = '●';
			this.vbox3.Add (this.answerEntry);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.answerEntry]));
			w3.Position = 1;
			w3.Expand = false;
			w3.Fill = false;
			w1.Add (this.vbox3);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(w1 [this.vbox3]));
			w4.Position = 0;
			w4.Expand = false;
			w4.Fill = false;
			// Internal child KaosEditor.UI.Dialogs.TextEntryDialog.ActionArea
			global::Gtk.HButtonBox w5 = this.ActionArea;
			w5.Name = "dialog1_ActionArea";
			w5.Spacing = 10;
			w5.BorderWidth = ((uint)(5));
			w5.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w6 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w5 [this.buttonCancel]));
			w6.Expand = false;
			w6.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseStock = true;
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "gtk-ok";
			this.AddActionWidget (this.buttonOk, -5);
			global::Gtk.ButtonBox.ButtonBoxChild w7 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w5 [this.buttonOk]));
			w7.Position = 1;
			w7.Expand = false;
			w7.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 274;
			this.DefaultHeight = 99;
			this.Show ();
			this.buttonCancel.Clicked += new global::System.EventHandler (this.OnButtonCancelClicked);
			this.buttonOk.Clicked += new global::System.EventHandler (this.OnButtonOkClicked);
		}
	}
}
