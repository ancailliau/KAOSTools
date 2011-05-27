// 
// Agent.cs
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
using Beaver.UI;
using Gtk;

namespace Beaver.Model
{
	
	/// <summary>
	/// Represents an agent for the system.
	/// </summary>
	public class Agent : KAOSElement
	{
		public string Id {
			get;
			set;
		}
		
		public string Name {
			get;
			set;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Beaver.Model.Agent"/> class.
		/// </summary>
		public Agent ()
			: this ("")
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Beaver.Model.Agent"/> class.
		/// </summary>
		/// <param name='name'>
		/// Name.
		/// </param>
		public Agent (string name) 
			: this(name, Guid.NewGuid().ToString())
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Beaver.Model.Agent"/> class.
		/// </summary>
		/// <param name='name'>
		/// Name.
		/// </param>
		/// <param name='id'>
		/// Identifier.
		/// </param>
		public Agent (string name, string id)
		{
			Name = name;
			Id = id;
		}
		
		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Beaver.Model.Agent"/>.
		/// </summary>
		/// <param name='obj'>
		/// The <see cref="System.Object"/> to compare with the current <see cref="Beaver.Model.Agent"/>.
		/// </param>
		/// <returns>
		/// <c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="Beaver.Model.Agent"/>; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals (object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals (this, obj))
				return true;
			if (obj.GetType () != typeof(Agent))
				return false;
			Model.Agent other = (Model.Agent)obj;
			return Id == other.Id;
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="Beaver.Model.Agent"/> object.
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

