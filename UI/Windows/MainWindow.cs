using System;
using Gtk;
using KaosEditor.Controllers;
using KaosEditor.Events;
using KaosEditor.Model;
using KaosEditor.UI.Widgets;
using KaosEditor.UI.Shapes;

namespace KaosEditor.UI.Windows {

	public partial class MainWindow: Gtk.Window
	{	
		public delegate void OnPopulateConceptListHandler (object sender, PopulateStoreEventArgs args);
		public event OnPopulateConceptListHandler PopulateConceptList;
		
		public EditorModel Model {
			get;
			set;
		}
		
		public MainController Controller {
			get;
			set;
		}
		
		private ViewsNotebook viewsNotebook;
		private ConceptsTreeView conceptTreeView;
		
		public MainWindow (EditorModel model): base (Gtk.WindowType.Toplevel)
		{
			this.Model = model;
			Build ();
			
			viewsNotebook = new ViewsNotebook();
			conceptTreeView = new ConceptsTreeView (this);
			
			hpaned1.Add1 (conceptTreeView);
			hpaned1.Add2 (viewsNotebook);
			hpaned1.ShowAll();
					
			Model.Changed += UpdateWidgets;
			Model.Views.ViewsChanged += UpdateWidgets;
			Model.Views.AddedView += UpdateWidgets;
				
			conceptTreeView.PopulateList += delegate(object sender, PopulateStoreEventArgs args) {
					if (PopulateConceptList != null) {
						PopulateConceptList(sender, args);
					}
				};
		}
		
		private void UpdateWidgets (object sender, EventArgs args)
		{
			viewsNotebook.Update();
			conceptTreeView.Update();
		}
		
		public void DisplayView (string name)
		{
			viewsNotebook.DisplayView (Model.Views.Get(name));
		}
			
		public void AddToCurrentView (IModelElement g)
		{
			if (g != null) {
				viewsNotebook.CurrentView.Add ( ShapeFactory.Create(g) );
			} else {
				Console.WriteLine ("Ignoring element '{0}'", g.Id);
			}
		}
		
		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			this.Controller.Quit ();
			a.RetVal = true;
		}
		
		protected virtual void OnQuitActionActivated (object sender, System.EventArgs e)
		{
			this.Controller.Quit ();
		}
			
		protected virtual void OnSaveActionActivated (object sender, System.EventArgs e)
		{
			this.Controller.SaveProject ();
		}
		
		protected virtual void OnOpenActionActivated (object sender, System.EventArgs e)
		{
			this.Controller.LoadProject ();
		}
		
		protected virtual void OnSaveAsActionActivated (object sender, System.EventArgs e)
		{
			this.Controller.SaveProjectAs ();
		}
			
	}
}
