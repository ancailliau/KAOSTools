// 
// TangoColorScheme.cs
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

namespace Beaver.UI.ColorSchemes
{
	public class TangoColorScheme : IColorScheme
	{
		
		private static Dictionary<string, string> c = new Dictionary<string, string>() {
			{ "AgentFillColor", "#fce94f" },
			{ "AgentStrokeColor", "#c4a000" },
			{ "AgentTextColor", "#000" },
			
			{ "DomainPropertyFillColor", "#ad7fa8" },
			{ "DomainPropertyStrokeColor", "#5c3566" },
			{ "DomainPropertyTextColor", "#000" },
			
			{ "ExceptionFillColor", "#fff" },
			{ "ExceptionStrokeColor", "#000" },
			{ "ExceptionTextColor", "#000" },
			
			{ "GoalFillColor", "#729fcf" },
			{ "GoalStrokeColor", "#204a87" },
			{ "GoalTextColor", "#000" },
			
			{ "ObstacleFillColor", "#ef2929" },
			{ "ObstacleStrokeColor", "#a40000" },
			{ "ObstacleTextColor", "#000" },
			
			{ "ObstacleRefinementFillColor", "#cc0000" },
			{ "ObstacleRefinementStrokeColor", "#000" },
			{ "ObstacleRefinementTextColor", "#000" },
			
			{ "ObstructionFillColor", "#f54326" },
			{ "ObstructionStrokeColor", "#000" },
			{ "ObstructionTextColor", "#000" },
			
			{ "RefinementFillColor", "#cc0000" },
			{ "RefinementStrokeColor", "#000" },
			{ "RefinementTextColor", "#000" },
			
			{ "ResolutionFillColor", "#5b953f" },
			{ "ResolutionStrokeColor", "#000" },
			{ "ResolutionTextColor", "#000" },
			
			{ "ResponsibilityFillColor", "#cc0000" },
			{ "ResponsibilityStrokeColor", "#000" },
			{ "ResponsibilityTextColor", "#000" }
			
		};
		
		protected override Dictionary<string, string> colors {
			get {
				 return c;
			}
		}
		
	}
}

