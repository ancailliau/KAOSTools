using System;
using System.Collections.Generic;

namespace Editor.Model
{
	public class Views : IEnumerable<View>
	{
		
		public delegate void AddedViewHandler (object sender, EventArgs e);
		public event AddedViewHandler AddedView;
		
		private List<View> views;
		
		public int Count {
			get { return views.Count ; }
		}
		
		public Views ()
		{
			views = new List<View>();
		}
		
		public void Add (View view)
		{
			views.Add(view);
			if (AddedView != null) {
				AddedView(this, EventArgs.Empty);
			}
		}
		
		public View Add (string name)
		{
			var view = new View(name);
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
	}
}

