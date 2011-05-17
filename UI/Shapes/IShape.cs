// 
// IShape.cs
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
using KaosEditor.Model;

namespace KaosEditor.UI.Shapes
{
	public interface IShape
	{
		
		/// <summary>
		/// Gets or sets the position of the center of the shape
		/// </summary>
		/// <value>
		/// The position.
		/// </value>
		PointD Position { 
			get ; 
			set ;
		}
		
		/// <summary>
		/// Gets or sets the element graphically represented by the shape
		/// </summary>
		/// <value>
		/// The represented element.
		/// </value>
		IModelElement RepresentedElement {
			get ;
			set ;
		}
		
		/// <summary>
		/// Gets or sets the depth of the shape
		/// </summary>
		/// <value>
		/// The depth.
		/// </value>
		int Depth {
			get ; 
			set ;
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="KaosEditor.UI.Shapes.IShape"/> is selected.
		/// </summary>
		/// <value>
		/// <c>true</c> if selected; otherwise, <c>false</c>.
		/// </value>
		bool Selected { 
			get ; 
			set ;
		}
		
		/// <summary>
		/// Display the shape on the specified context and view.
		/// </summary>
		/// <param name='context'>
		/// Context.
		/// </param>
		/// <param name='view'>
		/// View.
		/// </param>
		void Display (Context context, View view);
		
		/// <summary>
		/// Determines whether coordinates are in the form.
		/// </summary>
		/// <returns>
		/// The bounding box.
		/// </returns>
		/// <param name='x'>
		/// If set to <c>true</c> x.
		/// </param>
		/// <param name='y'>
		/// If set to <c>true</c> y.
		/// </param>
		/// <param name='delta'>
		/// If set to <c>true</c> delta.
		/// </param>
		bool InBoundingBox (double x, double y, out PointD delta);
		
		/// <summary>
		/// Gets the anchor corresponding for the given point.
		/// </summary>
		/// <returns>
		/// The anchor.
		/// </returns>
		/// <param name='point'>
		/// Point.
		/// </param>
		PointD GetAnchor(PointD point);
		
		/// <summary>
		/// Gets the bounds of the shape
		/// </summary>
		/// <returns>
		/// The bounds.
		/// </returns>
		Bounds GetBounds ();
	}
}

