using System;

namespace Editor
{
	public partial class Temporary : Gtk.Window
	{
		public Temporary () : 
				base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
		}
	}
}

