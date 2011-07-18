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
using Beaver.Logging;
using Beaver.Model;
using Beaver.UI.Windows;
using Beaver.UI.Widgets;
using Beaver.UI.Shapes;
using System.Collections.Generic;
using Beaver.Views;
using Beaver.UI.ColorSchemes;

namespace Beaver.Controllers
{
	/// <summary>
	/// Main controller.
	/// </summary>
	public class MainController
	{
		
		public GoalController GoalController {
			get;
			private set;
		}
		
		public AgentController AgentController  {
			get;
			private set;
		}
		
		public RefinementController RefinementController  {
			get;
			private set;
		}
		
		public ResponsibilityController ResponsibilityController  {
			get;
			private set;
		}
		
		public ObstacleController ObstacleController  {
			get;
			private set;
		}
		
		public ObstructionController ObstructionController  {
			get;
			private set;
		}
		
		public ResolutionController ResolutionController  {
			get;
			private set;
		}
		
		public ObstacleRefinementController ObstacleRefinementController  {
			get;
			private set;
		}
		
		public ExceptionController ExceptionController  {
			get;
			private set;
		}
		
		public ViewController ViewController  {
			get;
			private set;
		}
		
		public DomainPropertyController DomainPropertyController  {
			get;
			private set;
		}
		
		private List<IController> controllers = new List<IController> ();
		
		public IColorScheme CurrentColorScheme {
			get;
			set;
		}
		
		/// <summary>
		/// The current filename (empty if not yet saved)
		/// </summary>
		public string currentFilename = "";
		
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
		/// Gets or sets the window.
		/// </summary>
		/// <value>
		/// The window.
		/// </value>
		public MainWindow Window {
			get ; 
			set ;
		}
		
		public delegate void HandleProjectLoaded ();
		public event HandleProjectLoaded ProjetLoaded;
		
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Editor.Controllers.MainController"/> class.
		/// </summary>
		/// <param name='model'>
		/// Model.
		/// </param>
		/// <param name='window'>
		/// Window.
		/// </param>
		public MainController (MainWindow window)
		{	
			CurrentColorScheme = new TangoColorScheme ();
			
			this.Window = window;
			this.Window.Controller = this;
			
			InitControllers ();
			
			// Finish loading application
			this.LoadConfiguration();
			this.LoadPlugins();
		}

		private void InitControllers ()
		{
			ViewController = new ViewController (this);
			GoalController = new GoalController (this);
			AgentController = new AgentController (this);
			RefinementController = new RefinementController (this);
			ResponsibilityController = new ResponsibilityController (this);
			ObstacleController = new ObstacleController (this);
			ObstructionController = new ObstructionController (this);
			ResolutionController = new ResolutionController (this);
			ObstacleRefinementController = new ObstacleRefinementController (this);
			ExceptionController = new ExceptionController (this);
			DomainPropertyController = new DomainPropertyController (this);
			
			controllers = new List<IController> ();
			controllers.AddRange (new IController[] {
				ViewController,	GoalController, AgentController, RefinementController, 
				ResponsibilityController, ObstacleController,
				ObstructionController, ResolutionController,
				ObstacleRefinementController, ExceptionController,
				DomainPropertyController
			});
		}
		
		/// <summary>
		/// Show the main window refered by this instance.
		/// </summary>
		public void Show ()
		{
			if (Configuration.LastOpenedFilename != ""
				&& File.Exists(Configuration.LastOpenedFilename)) {
				this.currentFilename = Configuration.LastOpenedFilename;
				ReloadCurrentProject ();
			}
			
			if (this.Configuration.Maximized) {
				Window.Maximize();
			}
			
			string[] views = this.Configuration.OpenedViews.ToArray ();
			this.Configuration.OpenedViews.Clear ();
			foreach (var v in views) {
				var mv = this.ViewController.Get (v);
				this.ViewController.DisplayView (mv);
			}
			
			Window.Update ();
			Window.Present ();
		}
		
		/// <summary>
		/// Quit the application.
		/// </summary>
		public void Quit ()
		{
			this.SaveConfiguration();
			Application.Quit();
		}
		
		#region New/Load/Save project
		
		public void NewProject ()
		{
			Window.Reset ();
			InitControllers ();
			Window.Update ();
		}
		
		/// <summary>
		/// Saves the project.
		/// </summary>
		public void SaveProject ()
		{
			if (this.currentFilename == null | this.currentFilename == "") {
				this.SaveProjectAs ();
				ConnectWatcher ();
				
			} else {
				new XmlExporter(this.currentFilename, this).Export();
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
			
			Window.Title = string.Format("KAOS Editor - " + this.currentFilename);
			
			if (ProjetLoaded != null)
				ProjetLoaded ();
			
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
				"Beaver");
			
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
				"Beaver");
			
			this.Configuration = new EditorConfiguration();
			
			if (Directory.Exists(path)) {
				var configPath = Path.Combine(path, "config.xml");
				if (File.Exists(configPath)) {
					this.Configuration = EditorConfiguration.LoadFromFile(configPath);
				}
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
		
		public KAOSElement Get (string id)
		{
			if (GoalController.Get (id) != null)
				return GoalController.Get (id) ;
				
			if (AgentController.Get (id) != null)
				return AgentController.Get (id) ;
			
			if (ObstacleController.Get (id) != null)
				return ObstacleController.Get (id) ;
			
			if (ResolutionController.Get (id) != null)
				return ResolutionController.Get (id) ;
			
			if (DomainPropertyController.Get (id) != null)
				return DomainPropertyController.Get (id) ;
			
			if (ExceptionController.Get (id) != null)
				return ExceptionController.Get (id) ;
		
			if (ObstacleRefinementController.Get (id) != null)
				return ObstacleRefinementController.Get (id) ;
			
			if (ObstructionController.Get (id) != null)
				return ObstructionController.Get (id) ;
			
			if (ResponsibilityController.Get (id) != null)
				return ResponsibilityController.Get (id) ;
			
			if (RefinementController.Get (id) != null)
				return RefinementController.Get (id) ;
			
			Logger.Warning ("Ignoring element '{0}'", id);
			
			return null;
		}
		
		public void CheckModel () 
		{
			this.Window.PushStatus ("Checking model...");
			this.Window.ErrorList.Clear ();
			this.GoalController.CheckModel ();
			this.Window.PushStatus ("Model checked");
		}
		
		
	}
	
}

