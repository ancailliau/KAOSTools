using Editor.Windows;
using Gtk;
using KaosEditor.Controllers;
using KaosEditor.Model;

namespace Editor
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
