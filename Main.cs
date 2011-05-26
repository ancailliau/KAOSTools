using Gtk;
using KaosEditor.Controllers;
using KaosEditor.Model;
using KaosEditor.UI.Windows;
using System;
using GLib;

namespace KaosEditor
{
	class MainClass
	{
		private static MainController controller;
		private static MainWindow window;
		
		public static void Main (string[] args)
		{
			Application.Init ();
			
			window = new MainWindow ();
			controller = new MainController(window);
			
			controller.Show ();
			
			ExceptionManager.UnhandledException += HandleExceptionManagerUnhandledException;
			
			Application.Run ();
			
		}

		static void HandleExceptionManagerUnhandledException (UnhandledExceptionArgs args)
		{
			
			var dialog = new MessageDialog (window,
			DialogFlags.DestroyWithParent, MessageType.Error,
			ButtonsType.Ok, false, args.ExceptionObject.ToString ());
		
			dialog.Response += delegate(object o, ResponseArgs args2) {
				dialog.Destroy ();
				// args.ExitApplication = true;
			};
			
			dialog.Present ();
		}
		
	}
}
