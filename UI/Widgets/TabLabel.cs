using System;
using Gtk;
using KaosEditor.Logging;

namespace KaosEditor.UI.Widgets
{
	public class TabLabel : HBox
	{
		private static Gdk.Pixbuf closePixbuf;
		
		static TabLabel () {
			try {
				closePixbuf = Gdk.Pixbuf.LoadFromResource("KaosEditor.Images.Close.png");
			} catch (Exception e) {
				Logger.Warning ("Cannot load 'KaosEditor.Images.Close.png' from ressources", e);
			}
		}
		
		public delegate void CloseClickedHandler (object sender, EventArgs args);
		public event CloseClickedHandler CloseClicked;
		
		public string Text {
			get;
			private set;
		}
		
		private Label label;
		private Button button;
		
		public TabLabel (string name)
		{
			this.Text = name;
			label = new Label(name);
			
			// Ugly stuff to get rid of some noisy padding
			Gtk.Rc.ParseString ("style \"KaosEditor.ViewsNotebook.CloseButton\" {\n GtkButton::inner-border = {0,0,0,0}\n }\n");
      		Gtk.Rc.ParseString ("widget \"*.KaosEditor.ViewsNotebook.CloseButton\" style  \"KaosEditor.ViewsNotebook.CloseButton\"\n");
			
			button = new Button();
			button.Relief = ReliefStyle.None;
			button.Image = new Image(closePixbuf);
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

