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

namespace Beaver.UI.ColorSchemes
{
	public class TangoColorScheme : IColorScheme
	{
		
		public string AgentFillColor { get { return  "#fce94f"; } }
		public string AgentStrokeColor { get { return  "#c4a000"; } }
		public string AgentTextColor { get { return  "#000"; } }
		
		public string DomainPropertyFillColor { get { return  "#ad7fa8"; } }
		public string DomainPropertyStrokeColor { get { return  "#5c3566"; } }
		public string DomainPropertyTextColor { get { return  "#000"; } }
		
		public string ExceptionFillColor { get { return  "#fff"; } }
		public string ExceptionStrokeColor { get { return  "#000"; } }
		public string ExceptionTextColor { get { return  "#000"; } }
		
		public string GoalFillColor { get { return  "#729fcf"; } }
		public string GoalStrokeColor { get { return  "#204a87"; } }
		public string GoalTextColor { get { return  "#000"; } }
		
		public string ObstacleFillColor { get { return  "#ef2929"; } }
		public string ObstacleStrokeColor { get { return  "#a40000"; } }
		public string ObstacleTextColor { get { return  "#000"; } }
		
		public string ObstacleRefinementFillColor { get { return  "#cc0000"; } }
		public string ObstacleRefinementStrokeColor { get { return  "#000"; } }
		public string ObstacleRefinementTextColor { get { return  "#000"; } }
		
		public string ObstructionFillColor { get { return  "#f54326"; } }
		public string ObstructionStrokeColor { get { return  "#000"; } }
		public string ObstructionTextColor { get { return  "#000"; } }
		
		public string RefinementFillColor { get { return  "#cc0000"; } }
		public string RefinementStrokeColor { get { return  "#000"; } }
		public string RefinementTextColor { get { return  "#000"; } }
		
		public string ResolutionFillColor { get { return  "#5b953f"; } }
		public string ResolutionStrokeColor { get { return  "#000"; } }
		public string ResolutionTextColor { get { return  "#000"; } }
		
		public string ResponsibilityFillColor { get { return  "#cc0000"; } }
		public string ResponsibilityStrokeColor { get { return  "#000"; } }
		public string ResponsibilityTextColor { get { return  "#000"; } }
		
	}
}

