using System;
using Gtk;
using Model;

namespace Editor
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			
			//var g1 = new Goal ("Achieve\n[IncidentResolved]") { Id = "goal1" };
			//var g2 = new Goal ("Achieve\n[IncidentReported]") { Id = "goal2" };
			//var g3 = new Goal ("Achieve\n[ReportedIncidentResolved]") { Id = "goal3" };
			//g1.Children.Add(g2);
			//g1.Children.Add(g3);
			
			var model = new GoalModel();
			//model.AddGoals(new [] { g1, g2, g3 });
			
			MainWindow win = new MainWindow (model);
			win.Show ();
			Application.Run ();
		}
	}
}
