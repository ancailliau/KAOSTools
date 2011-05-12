using System;
using Gtk;

namespace Editor.Widgets
{
	public class TabLabel : HBox
	{
		public delegate void CloseClickedHandler (object sender, EventArgs args);
		public event CloseClickedHandler CloseClicked;
		
		public string Name {
			get;
			set;
		}
		
		private Label label;
		private Button button;
		
		public TabLabel (string name)
		{
			this.Name = name;
			label = new Label(name);
			
			// Ugly stuff to get rid of some noisy padding
			Gtk.Rc.ParseString ("style \"KaosEditor.ViewsNotebook.CloseButton\" {\n GtkButton::inner-border = {0,0,0,0}\n }\n");
      		Gtk.Rc.ParseString ("widget \"*.KaosEditor.ViewsNotebook.CloseButton\" style  \"KaosEditor.ViewsNotebook.CloseButton\"\n");
			
			button = new Button();
			button.Relief = ReliefStyle.None;
			button.Image = new Image(Gdk.Pixbuf.LoadFromResource("Editor.Images.Close.png"));
			button.Name = "KaosEditor.ViewsNotebook.CloseButton";
			button.Clicked += delegate(object sender, EventArgs e) {
				if (CloseClicked != null) {
					CloseClicked (this, EventArgs.Empty);
				}
			};
			
			this.PackStart(label, true, true, 0);
			this.PackEnd(button, false, false, 0);
			this.ShowAll();
		}
	}
}

