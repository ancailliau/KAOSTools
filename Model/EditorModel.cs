// 
// EditorModel.cs
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
using KaosEditor.Controllers;

namespace KaosEditor.Model
{
	
	/// <summary>
	/// Represents the model behind the editor. The model is broadly divided in
	/// two parts: 
	/// - instances of object from the meta-model of KAOS,
	/// - and the views (graphical representation of a subset of these elements)
	/// </summary>
	public class EditorModel
	{
		
		/// <summary>
		/// Handler activated when the model changed
		/// </summary>
		public delegate void ChangedModelHandler (object sender, EventArgs e);
		
		public delegate void ElementAddedHandler (KAOSElement element);
		public delegate void ElementRemovedHandler (KAOSElement element);
		public event ElementAddedHandler ElementAdded;
		public event ElementRemovedHandler ElementRemoved;
		
		/// <summary>
		/// Occurs when model changed.
		/// </summary>
		public event ChangedModelHandler Changed;
		
		/// <summary>
		/// The list of element representing the model.
		/// </summary>
		public List<KAOSElement> Elements {
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the views.
		/// </summary>
		/// <value>
		/// The views.
		/// </value>
		public Views Views {
			get;
			set;
		}
		
		
		private MainController controller;
		
		/// <summary>
		/// Gets or sets the controller.
		/// </summary>
		/// <value>
		/// The controller.
		/// </value>
		public MainController Controller {
			get { return controller; }
			set { controller = value; Views.Controller = value; }
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="KaosEditor.Model.EditorModel"/> class.
		/// </summary>
		public EditorModel ()
		{
			Elements = new List<KAOSElement>();
			Views = new Views(null);
		}
		
		/// <summary>
		/// Add the specified element.
		/// </summary>
		/// <param name='element'>
		/// Element.
		/// </param>
		public void Add (KAOSElement element) 
		{
			if (Elements.Contains (element)) {
				throw new ArgumentException (string.Format("Duplicated element '{0}'", element));
			}
			Elements.Add(element);
			if (ElementAdded != null) {
				ElementAdded (element);
			}
		}
		
		public void Remove (KAOSElement element)
		{
			if (Elements.Contains (element)) {
				Elements.Remove (element);
				if (ElementRemoved != null) {
					ElementRemoved (element);
				}
			} else {
				throw new ArgumentException (
					string.Format ("Removed element '{0}' is not in the model", 
						element));
			}
		}
		
		public void Update (KAOSElement element)
		{
			if (Elements.Contains (element)) {
				if (ElementRemoved != null) {
					ElementRemoved (element);
				}
			} else {
				throw new ArgumentException (
					string.Format ("Modified element '{0}' is not in the model", 
						element));
			}
		}
		
		/// <summary>
		/// Get the specified id.
		/// </summary>
		/// <param name='id'>
		/// Identifier.
		/// </param>
		public KAOSElement Get (string id)
		{
			return Elements.Find(t => t.Id == id);
		}

		[Obsolete("Use method `Add (KAOSElement)', `Remove (KAOSElement)' or `Update (KAOSElement)' instead")]
		public void NotifyChange ()
		{
			if (Changed != null) {
				Changed(this, EventArgs.Empty);
			}
		}
		
		/// <summary>
		/// Set the specified model as the current model.
		/// </summary>
		/// <param name='model'>
		/// Model.
		/// </param>
		public void Set (Model.EditorModel model)
		{
			this.Elements = model.Elements;
			this.Views.Set(model.Views);
			
			NotifyChange ();
		}
		
	}
}

