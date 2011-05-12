using System;
using System.Collections.Generic;
using Model;
using Editor.Windows;
using Editor.Model;

namespace Editor.Controllers
{
	public class MainController
	{
		
		public GoalModel Model {
			get;
			set;
		}
		
		public Views Views {
			get;
			set;
		}
		
		private MainWindow window;
		public MainWindow Window {
			get { return window ; }
			set { value.Controller = this; window = value ; }
		}
		
		public MainController ()
		{
		}
		
		public void Show ()
		{
			Window.Present();
		}
		
	}
}

