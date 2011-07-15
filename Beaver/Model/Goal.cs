// 
// Goal.cs
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
using Beaver.UI.Windows;
using Beaver.UI.Dialogs;
using Beaver.UI;

namespace Beaver.Model
{
	
	/// <summary>
	/// Represents a goal.
	/// </summary>
	public class Goal : IGoalRefinee
	{
		public string Name {
			get;
			set;
		}
		
		public string Definition {
			get;
			set;
		}
		
		public string Id {
			get;
			set;
		}
		
		public float Likelihood {
			get;
			set;
		}
		
		public float SoftThreshold {
			get;
			set;
		}
		
		public float HardThreshold {
			get;
			set;
		}
		
		public Goal (string name)
			: this (name, "")
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Beaver.Model.Goal"/> class.
		/// </summary>
		/// <param name='name'>
		/// Name.
		/// </param>
		public Goal (string name, string definition) 
		{
			Id = Guid.NewGuid().ToString();
			Definition = definition;
			Name = name;
			Likelihood = 1;
			SoftThreshold = 1;
			HardThreshold = 1;
		}
		
		public override bool Equals (object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals (this, obj))
				return true;
			if (obj.GetType () != typeof(Goal))
				return false;
			Model.Goal other = (Model.Goal)obj;
			return Id == other.Id;
		}

		public override int GetHashCode ()
		{
			unchecked {
				return (Id != null ? Id.GetHashCode () : 0);
			}
		}
	}
}
