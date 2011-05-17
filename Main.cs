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
			var views = new Views (null);
			var window = new MainWindow (model);
			var controller = new MainController(model, window);
			controller.Show ();
			
			Application.Run ();
		}
		
	}
}
