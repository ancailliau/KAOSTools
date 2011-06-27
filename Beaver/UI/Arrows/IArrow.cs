// 
// IArrow.cs
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

using Cairo;
using Beaver.Model;
using Beaver.UI.Shapes;
using Beaver.Views;

namespace Beaver.UI.Arrows
{
	
	/// <summary>
	/// Represents an abstract arrow
	/// </summary>
	public interface IArrow
	{
		
		/// <summary>
		/// Gets or sets the shape at the start of the arrow.
		/// </summary>
		/// <value>
		/// The start.
		/// </value>
		IShape Start {
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the shape at the end of the arrow.
		/// </summary>
		/// <value>
		/// The end.
		/// </value>
		IShape End {
			get;
			set;
		}
		
		/// <summary>
		/// Display the arrow on the specified context and for the specified view.
		/// </summary>
		/// <param name='context'>
		/// Context.
		/// </param>
		/// <param name='view'>
		/// View.
		/// </param>
		void Display (Context context, ModelView view);
	}
}
