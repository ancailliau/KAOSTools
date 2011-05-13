using System;
using System.Collections.Generic;
using Model;
using Editor.Windows;
using Editor.Model;
using Gtk;

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
			Window.Present();
		}
		
		public void Quit ()
		{
			Application.Quit();
		}
		
		public void Save ()
		{
			if (this.filename == "") {
				this.SaveAs ();
				
			} else {
				new XmlExporter(this.filename, Model, Views).Export();
			}
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
				var importer = new XmlImporter(dialog.Filename);
				importer.Import();
				
				this.Model = importer.Model;
				this.Views = importer.Views;
				
				if (this.Views.Count > 0) {
					foreach (var v in importer.Views) {
						Views.Add(v);
					}
					
				} else {
					var errorDialog = new MessageDialog(this.window, DialogFlags.DestroyWithParent, MessageType.Error,
						ButtonsType.Ok, false, "File is malformed.");
					
					if (errorDialog.Run() > 0) {
						errorDialog.Destroy();
					}
				}
			}
			
			dialog.Destroy();
		}
		
	}
}

