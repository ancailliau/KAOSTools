using System;
using Gtk;
using Model;
using System.Collections.Generic;
using Editor.Windows;
using Editor.Model;
using Editor.Controllers;

namespace Editor
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			
			var model = new GoalModel ();
			var views = new Views ();
			
			var window = new MainWindow (model, views);
			
			var controller = new MainController() {
				Model = model, Views = views, Window = window
			};
			
			controller.Show ();
			
			Application.Run ();
		}
	}
}
