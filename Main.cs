using Gtk;
using KaosEditor.Controllers;
using KaosEditor.Model;
using KaosEditor.UI.Windows;

namespace KaosEditor
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			
			var model = new EditorModel ();
			var controller = new MainController(model);
			var window = new MainWindow (model, controller);
			controller.Window = window;
			
			controller.Show ();
			
			Application.Run ();
		}
		
	}
}
