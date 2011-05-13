using System;
using Gtk;
using Editor;
using Editor.Widgets;
using Editor.Model;
using System.Collections.Generic;

namespace Editor.Widgets
{
	public class ViewsNotebook : Notebook
	{
		
		private List<View> displayedViews;
		
		public View CurrentView {
			get {
				if (this.CurrentPage >= 0 && this.CurrentPage < displayedViews.Count) {
					return displayedViews[this.CurrentPage];
				} else {
					return null;
				}
			}
		}
		
		public ViewsNotebook ()
		{
			displayedViews = new List<View>();
		}
		
		protected override void OnSwitchPage (NotebookPage page, uint page_num)
		{
			base.OnSwitchPage (page, page_num);
		}
		
		public void DisplayView (View view)
		{
			// Move to page if page already exists
			foreach (var child in this.Children) {
				if (((TabLabel) this.GetTabLabel (child)).Name == view.Name) {
					this.CurrentPage = this.PageNum(child);
					return ;
				}
			}
			
			displayedViews.Add(view);
			
			// Add the page if the page does not exists
			var diagram = new DiagramArea(view);
			view.DrawingArea = diagram;
			var tabLabel = new TabLabel (view.Name);
			tabLabel.CloseClicked += delegate(object sender, EventArgs args) {
				TabLabel label = (TabLabel) sender;
				foreach (var child in this.Children) {
					if (this.GetTabLabel (child) == label) {
						int pageNum = this.PageNum (child);
						Hide (pageNum);
					}
				}
			};
			this.AppendPage(diagram, tabLabel);
			
			this.ShowAll();
		}
		
		private void Hide (int pageNum)
		{
			this.displayedViews.RemoveAt(pageNum);
			this.RemovePage(pageNum);
		}
		
		public void Update ()
		{
			if (this.CurrentView != null) {
				this.CurrentView.Redraw();
			}
		}
	}
}

