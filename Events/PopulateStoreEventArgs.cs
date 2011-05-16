// 
// PopulateStoreEventArgs.cs
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
using Editor.Widgets;
using Gtk;

namespace KaosEditor.Events
{
	
	/// <summary>
	/// Represents the argument for the event raised with store is populated.
	/// </summary>
	public class PopulateStoreEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the tree view bind to the store
		/// </summary>
		/// <value>
		/// The view.
		/// </value>
		public TreeView View {
			get;
			private set;
		}
		
		/// <summary>
		/// Gets or sets the store.
		/// </summary>
		/// <value>
		/// The store.
		/// </value>
		public TreeStore Store {
			get;
			private set;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Editor.Events.PopulateStoreEventArgs"/> class.
		/// </summary>
		/// <param name='view'>
		/// View.
		/// </param>
		/// <param name='store'>
		/// Store.
		/// </param>
		public PopulateStoreEventArgs (TreeView view, TreeStore store)
		{
			View = view;
			Store = store;
		}
	}
}

