// 
// ViewsNotebook.cs
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
using System.Collections.Generic;
using Gtk;
using KaosEditor.Model;
using KaosEditor.UI.Shapes;
using KaosEditor.Controllers;
using KaosEditor.Views;

namespace KaosEditor.UI.Widgets
{
	
	/// <summary>
	/// Represents a notebook for views.
	/// </summary>
	public class ViewsNotebook : Notebook
	{
		
		/// <summary>
		/// The list of displayed views.
		/// </summary>
		private List<ModelView> displayedViews;
		
		/// <summary>
		/// Gets the current view.
		/// </summary>
		/// <value>
		/// The current view.
		/// </value>
		public ModelView CurrentView {
			get {
				if (this.CurrentPage >= 0 && this.CurrentPage < displayedViews.Count) {
					return displayedViews[this.CurrentPage];
				} else {
					return null;
				}
			}
		}
		
		private MainController controller;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="KaosEditor.UI.Widgets.ViewsNotebook"/> class.
		/// </summary>
		public ViewsNotebook ()
		{
			displayedViews = new List<ModelView>();
			this.controller = null;
			this.Scrollable = true;
		}
		
		/// <summary>
		/// Displaies the view.
		/// </summary>
		/// <param name='view'>
		/// View.
		/// </param>
		public void DisplayView (ModelView view)
		{
			// Move to page if page already exists
			foreach (var child in this.Children) {
				if (((TabLabel) this.GetTabLabel (child)).Text == view.Name) {
					this.CurrentPage = this.PageNum(child);
					return ;
				}
			}
			
			displayedViews.Add(view);
			
			// Add the page if the page does not exists
			var scroll = new ScrolledWindow ();
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
			scroll.AddWithViewport(diagram);
			
			foreach (var p in menuPopulater) {
				diagram.RegisterForMenu (p);
			}
			
			int page = this.AppendPage(scroll, tabLabel);
			this.ShowAll();
			this.CurrentPage = page;
			
		}
		
		public void CloseView (ModelView view)
		{
			foreach (var child in this.Children) {
				var scroll = (ScrolledWindow) child;
				var viewport = (Viewport) scroll.Child;
				var diagram = (DiagramArea) viewport.Child;
				if (diagram.View == view) {
					int pageNum = this.PageNum (child);
					Hide (pageNum);
				}
			}
		}
		
		/// <summary>
		/// Hide the specified page.
		/// </summary>
		/// <param name='pageNum'>
		/// Page number.
		/// </param>
		private void Hide (int pageNum)
		{
			this.displayedViews.RemoveAt(pageNum);
			this.RemovePage(pageNum);
		}
		
		/// <summary>
		/// Redraw current view.
		/// </summary>
		public void RedrawCurrentView ()
		{
			if (this.CurrentView != null) {
				this.CurrentView.Redraw();
			}
		}
		
		public void AddToCurrentView (KAOSElement element, double x, double y)
		{
			if (this.CurrentView != null) {
				var shape = ShapeFactory.Create(element, x, y);
				this.CurrentView.Add(shape);
			}
		}
		
		
		private List<IPopulateMenu> menuPopulater = new List<IPopulateMenu>();
		
		public void RegisterForDiagramMenu (IPopulateMenu populater)
		{
			this.menuPopulater.Add (populater);
		}
	}
}

