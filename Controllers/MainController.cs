using System;
using System.Collections.Generic;
using Model;
using Editor.Windows;
using Editor.Model;
using Gtk;
using System.IO;

namespace Editor.Controllers
{
	public class MainController
	{
		private string filename = "";
		
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
			Window.Maximize();
			Window.Present();
		}
		
		public void Quit ()
		{
			Application.Quit();
		}
		
		public void Save ()
		{
			if (this.filename == null | this.filename == "") {
				this.SaveAs ();
				ConnectWatcher ();
				
			} else {
				new XmlExporter(this.filename, Model, Views).Export();
			}
		}
		
		private void ConnectWatcher ()
		{
			Console.WriteLine ("File watching enabled.");
			var directory = Path.GetDirectoryName(this.filename);
			var filename = Path.GetFileName(this.filename);
			var watcher = new FileSystemWatcher (directory, filename);
			watcher.NotifyFilter =  NotifyFilters.LastWrite ;
			watcher.Changed += delegate(object sender, FileSystemEventArgs e) {
				if (e.ChangeType == WatcherChangeTypes.Changed) {
					Console.WriteLine ("File '{0}' changed, reloading.", e.FullPath);
					this.Reload ();
				}
			};
			watcher.Renamed += delegate(object sender, RenamedEventArgs e) {
				Console.WriteLine ("File '{0}' moved to '{1}'.", e.OldFullPath, e.FullPath);
				this.filename = e.FullPath;
			};
			watcher.EnableRaisingEvents = true;
		}
		
		public void SaveAs ()
		{
			var dialog = new FileChooserDialog("Save file", this.window,
			FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Accept);
			
			if (dialog.Run() == (int) ResponseType.Accept) {
				this.filename = dialog.Filename;
				Save();
			}
			
			dialog.Destroy();		
		}
		
		public void Load ()
		{
			var dialog = new FileChooserDialog ("Choose file to open...",
			this.window, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
			
			if (dialog.Run() == (int) ResponseType.Accept) {
				this.filename = dialog.Filename;
				Reload ();
				ConnectWatcher ();
			}
			
			dialog.Destroy();
		}
		
		public void Reload ()
		{
			if (this.filename == null | this.filename == "") {
				Load ();
			}
			
			var importer = new XmlImporter(this.filename, this);
			importer.Import();
			
			this.Model.Set(importer.Model);
			this.Views.Set(importer.Views);
		}
		
	}
}

