// 
// MainController.cs
//  
// Author:
//       Antoine Cailliau <antoine.cailliau@uclouvain.be>
// 
// Copyright (c) 2011 2011 Université Catholique de Louvain and Antoine Cailliau
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.IO;
using System.Reflection;
using Gtk;
using KaosEditor.Logging;
using KaosEditor.Model;
using KaosEditor.UI.Windows;

namespace KaosEditor.Controllers
{
	/// <summary>
	/// Main controller.
	/// </summary>
	public class MainController
	{
		
		/// <summary>
		/// The current filename (empty if not yet saved)
		/// </summary>
		private string currentFilename = "";
		
		/// <summary>
		/// Gets or sets the configuration.
		/// </summary>
		/// <value>
		/// The configuration.
		/// </value>
		public EditorConfiguration Configuration {
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the model.
		/// </summary>
		/// <value>
		/// The model.
		/// </value>
		public EditorModel Model {
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the window.
		/// </summary>
		/// <value>
		/// The window.
		/// </value>
		public MainWindow Window {
			get ; 
			set ;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Editor.Controllers.MainController"/> class.
		/// </summary>
		/// <param name='model'>
		/// Model.
		/// </param>
		/// <param name='window'>
		/// Window.
		/// </param>
		public MainController (EditorModel model, MainWindow window)
		{
			// Save the model and window
			this.Model = model;
			this.Window = window;
			
			// Bind to the current controller
			this.Model.Controller = this;
			this.Window.Controller = this;
			
			// Finish loading application
			this.LoadConfiguration();
			this.LoadPlugins();
		}
		
		/// <summary>
		/// Show the main window refered by this instance.
		/// </summary>
		public void Show ()
		{
			if (this.Configuration.Maximized) {
				Window.Maximize();
			}
			Window.Present();
		}
		
		/// <summary>
		/// Quit the application.
		/// </summary>
		public void Quit ()
		{
			this.SaveConfiguration();
			Application.Quit();
		}
		
		#region Load/Save project
		
		/// <summary>
		/// Saves the project.
		/// </summary>
		public void SaveProject ()
		{
			if (this.currentFilename == null | this.currentFilename == "") {
				this.SaveProjectAs ();
				ConnectWatcher ();
				
			} else {
				new XmlExporter(this.currentFilename, Model).Export();
			}
		}
		
		/// <summary>
		/// Connects the file watcher to update file if modified outside the application
		/// </summary>
		private void ConnectWatcher ()
		{
			var directory = Path.GetDirectoryName(this.currentFilename);
			var filename = Path.GetFileName(this.currentFilename);
			var watcher = new FileSystemWatcher (directory, filename);
			watcher.NotifyFilter =  NotifyFilters.LastWrite ;
			watcher.Changed += delegate(object sender, FileSystemEventArgs e) {
				if (e.ChangeType == WatcherChangeTypes.Changed) {
					Logger.Info ("File '{0}' changed, reloading", e.FullPath);
					this.ReloadCurrentProject ();
				}
			};
			watcher.Renamed += delegate(object sender, RenamedEventArgs e) {
				Logger.Info ("File '{0}' moved to '{1}'.", e.OldFullPath, e.FullPath);
				this.currentFilename = e.FullPath;
			};
			watcher.EnableRaisingEvents = true;
			
			Logger.Info ("File watching enabled for file '{0}'", this.currentFilename);
		}
		
		/// <summary>
		/// Saves the project as new name
		/// </summary>
		public void SaveProjectAs ()
		{
			var dialog = new FileChooserDialog("Save file", this.Window,
			FileChooserAction.Save, "Cancel", ResponseType.Cancel, "Save", ResponseType.Accept);
			
			if (dialog.Run() == (int) ResponseType.Accept) {
				this.currentFilename = dialog.Filename;
				this.Configuration.LastOpenedFilename = dialog.Filename;
				SaveProject();
			}
			
			dialog.Destroy();		
		}
		
		/// <summary>
		/// Loads the project from external file
		/// </summary>
		public void LoadProject ()
		{
			var dialog = new FileChooserDialog ("Choose file to open...",
			this.Window, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);
			
			if (dialog.Run() == (int) ResponseType.Accept) {
				this.currentFilename = dialog.Filename;
				this.Configuration.LastOpenedFilename = dialog.Filename;
				ReloadCurrentProject ();
				ConnectWatcher ();
			}
			
			dialog.Destroy();
		}
		
		/// <summary>
		/// Reloads the current project.
		/// </summary>
		public void ReloadCurrentProject ()
		{
			if (this.currentFilename == null | this.currentFilename == "") {
				LoadProject ();
			}
			
			var importer = new XmlImporter(this.currentFilename, this);
			importer.Import();
			
			this.Model.Set(importer.Model);
		}
		
		#endregion
		
		#region Configuration
	
		/// <summary>
		/// Saves the configuration.
		/// </summary>
		private void SaveConfiguration ()
		{
			string path = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				"KaosEditor");
			
			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
			
			var configPath = Path.Combine(path, "config.xml");
			this.Configuration.SaveToFile(configPath);
		}
		
		/// <summary>
		/// Loads the configuration.
		/// </summary>
		private void LoadConfiguration ()
		{
			string path = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				"KaosEditor");
			
			this.Configuration = new EditorConfiguration();
			
			if (Directory.Exists(path)) {
				var configPath = Path.Combine(path, "config.xml");
				if (File.Exists(configPath)) {
					this.Configuration = EditorConfiguration.LoadFromFile(configPath);
				}
			}
			
			if (Configuration.LastOpenedFilename != ""
				&& File.Exists(Configuration.LastOpenedFilename)) {
				this.currentFilename = Configuration.LastOpenedFilename;
				ReloadCurrentProject ();
			}
		}
		
		#endregion
		
		#region Plugins
		
		/// <summary>
		/// Loads the plugins.
		/// </summary>
		private void LoadPlugins ()
		{
			foreach (var plugin in this.Configuration.Plugins) {
				LoadPlugin (plugin.Name, plugin.Path);
			}
		}
		
		/// <summary>
		/// Loads the plugin.
		/// </summary>
		/// <param name='name'>
		/// Name.
		/// </param>
		/// <param name='path'>
		/// Path.
		/// </param>
		private void LoadPlugin (string name, string path)
		{
			Logger.Info ("Load plugin '{0}'", name);
			
			Assembly assembly = Assembly.LoadFile(path);
			Type type = assembly.GetType(name);
			if (type != null) {
				ConstructorInfo constructor = type.GetConstructor(new Type[] {typeof(MainController)});
				if (constructor != null) {
					Activator.CreateInstance(type, this);
					Logger.Info ("Plugin '{0}' loaded", name);
					
				} else {
					Logger.Warning ("Cannot find constructor {0}(MainController) to initialize plugin", name);
				}
				
			} else {
				Logger.Warning ("Cannot load plugin '{0}'", name);
			}
		}
		
		#endregion
		
	}
}

