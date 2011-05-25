using System;
using System.Collections.Generic;
using KaosEditor.Controllers;

namespace KaosEditor.Views
{
	public class ModelViews : IEnumerable<ModelView>
	{
		
		public delegate void AddedViewHandler (object sender, EventArgs e);
		public event AddedViewHandler AddedView;
		
		public delegate void ViewsChangedHandler (object sender, EventArgs e);
		public event ViewsChangedHandler ViewsChanged;
		
		private List<ModelView> views;
		
		private MainController controller;
		public MainController Controller {
			get {
				return controller;
			}
			set { 
				foreach (var v in views) {
					v.Controller = value;
				}
				this.controller = value;
			}
		}
		
		public int Count {
			get { return views.Count ; }
		}
		
		public ModelViews (MainController controller)
		{
			views = new List<ModelView>();
			this.controller = controller;
		}
		
		public void Add (ModelView view)
		{
			views.Add(view);
			view.ViewChanged += delegate(object sender, EventArgs e) {
				NotifyViewsChanged();
			};
			if (AddedView != null) {
				AddedView(this, EventArgs.Empty);
			}
		}
		
		public ModelView Add (string name)
		{
			var view = new ModelView(name, controller);
			Add (view);
			return view;
		}
		
		public ModelView Get (string name)
		{
			return views.Find(v => v.Name == name);
		}
		
		IEnumerator<ModelView> IEnumerable<ModelView>.GetEnumerator ()
		{
			return views.GetEnumerator ();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return views.GetEnumerator ();
		}
		
		public void NotifyViewsChanged ()
		{
			if (ViewsChanged != null) {
				ViewsChanged(this, EventArgs.Empty);
			}
		}
		
		public void Set (ModelViews newView)
		{
			views = newView.views;
			foreach (var v in views) {
				v.ViewChanged += delegate(object sender, EventArgs e) {
					NotifyViewsChanged();
				};
			}
			NotifyViewsChanged();
		}
		
	}
}

