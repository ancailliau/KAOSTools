using System;
using Gtk;
using Editor;
using Editor.Widgets;
using Editor.Model;

namespace Editor.Widgets
{
	public class ViewsNotebook : Notebook
	{
		public ViewsNotebook ()
		{
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
			
			// Add the page if the page does not exists
			var diagram = new DiagramArea(view);
			var tabLabel = new TabLabel (view.Name);
			tabLabel.CloseClicked += delegate(object sender, EventArgs args) {
				TabLabel label = (TabLabel) sender;
				foreach (var child in this.Children) {
					if (this.GetTabLabel (child) == label) {
						int pageNum = this.PageNum (child);
						this.RemovePage(pageNum);
					}
				}
			};
			this.AppendPage(diagram, tabLabel);
			
			this.ShowAll();
		}
		
	}
}

