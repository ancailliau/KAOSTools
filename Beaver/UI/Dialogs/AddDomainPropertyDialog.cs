// 
// AddDomainPropertyDialog.cs
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
using Beaver.UI.Windows;
using Gtk;
using Beaver.Model;

namespace Beaver.UI.Dialogs
{
	public partial class AddDomainPropertyDialog : Gtk.Dialog
	{
		
		#region Value of fields
		
		public string PropertyName {
			get {
				return nameEntry.Text.Trim ();
			}
			set {
				nameEntry.Text = value;
			}
		}
		
		public string Definition {
			get {
				return definitionTextview.Buffer.Text.Trim ();
			}
			set {
				definitionTextview.Buffer.Text = value;
			}
		}
		
		#endregion
	
		public AddDomainPropertyDialog (MainWindow window, string domPropName)
			: this (window, new DomainProperty (domPropName, ""), false)
		{
		}
		
		public AddDomainPropertyDialog (MainWindow window, DomainProperty domProp, bool edit)
			: base (edit ? string.Format ("Edit domain property '{0}'", domProp.Name) : "Add domain property", 
				window, DialogFlags.DestroyWithParent)
		{
			if (domProp == null)
				throw new ArgumentNullException ("domProp");
			
			this.Build ();
			PropertyName = domProp.Name;
			Definition = domProp.Definition;
		}
	}
}

