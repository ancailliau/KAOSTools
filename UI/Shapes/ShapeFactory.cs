// 
// ShapeFactory.cs
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

using KaosEditor.Model;
using KaosEditor.Logging;

namespace KaosEditor.UI.Shapes
{
	
	/// <summary>
	/// Shape factory.
	/// </summary>
	public static class ShapeFactory
	{
		
		/// <summary>
		/// Create the shape for the specified element.
		/// </summary>
		/// <param name='element'>
		/// Element.
		/// </param>
		public static IShape Create(IModelElement element)
		{
			if (element is Goal) {
				return new GoalShape(element as Goal);
			
			} else if (element is Refinement) {
				return new RefinementShape(element as Refinement);
			
			} else if (element is Agent) {
				return new AgentShape(element as Agent);
			
			} else if (element is Responsibility) {
				return new ResponsibilityShape(element as Responsibility);
				
			} else {
				if (element != null)
					Logger.Warning ("Shape does not exists for element '{0}'", element.GetType());
				
				return null;
			}
		}
	}
}
