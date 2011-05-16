using System;
using System.Collections.Generic;
using KaosEditor.Controllers;

namespace KaosEditor.Model
{
	public class Views : IEnumerable<View>
	{
		
		public delegate void AddedViewHandler (object sender, EventArgs e);
		public event AddedViewHandler AddedView;
		
		public delegate void ViewsChangedHandler (object sender, EventArgs e);
		public event ViewsChangedHandler ViewsChanged;
		
		private List<View> views;
		
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
		
		public Views (MainController controller)
		{
			views = new List<View>();
			this.controller = controller;
		}
		
		public void Add (View view)
		{
			views.Add(view);
			view.ViewChanged += delegate(object sender, EventArgs e) {
				NotifyViewsChanged();
			};
			if (AddedView != null) {
				AddedView(this, EventArgs.Empty);
			}
		}
		
		public View Add (string name)
		{
			var view = new View(name, controller);
			Add (view);
			return view;
		}
		
		public View Get (string name)
		{
			return views.Find(v => v.Name == name);
		}
		
		IEnumerator<View> IEnumerable<View>.GetEnumerator ()
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
		
		public void Set (Views newView)
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

