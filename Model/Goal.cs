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

namespace KaosEditor.Model
{
	
	/// <summary>
	/// Represents a goal.
	/// </summary>
	public class Goal : IModelElement
	{
		
		/// <summary>
		/// Gets or sets the identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		public string Id {
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name {
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the refinements.
		/// </summary>
		/// <value>
		/// The refinements.
		/// </value>
		public List<Refinement> Refinements {
			get;
			set;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="KaosEditor.Model.Goal"/> class.
		/// </summary>
		/// <param name='name'>
		/// Name.
		/// </param>
		public Goal (string name) 
		{
			Id = Guid.NewGuid().ToString();
			Refinements = new List<Refinement>();
			
			Name = name;
		}
		
		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="KaosEditor.Model.Goal"/>.
		/// </summary>
		/// <param name='obj'>
		/// The <see cref="System.Object"/> to compare with the current <see cref="KaosEditor.Model.Goal"/>.
		/// </param>
		/// <returns>
		/// <c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="KaosEditor.Model.Goal"/>; otherwise, <c>false</c>.
		/// </returns>
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

		/// <summary>
		/// Serves as a hash function for a <see cref="KaosEditor.Model.Goal"/> object.
		/// </summary>
		/// <returns>
		/// A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.
		/// </returns>
		public override int GetHashCode ()
		{
			unchecked {
				return (Id != null ? Id.GetHashCode () : 0);
			}
		}
	}
}
