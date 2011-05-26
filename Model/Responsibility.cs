// 
// Responsibility.cs
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
using KaosEditor.UI;
using Gtk;
using KaosEditor.UI.Windows;
using KaosEditor.UI.Shapes;
using Cairo;

namespace KaosEditor.Model
{
	
	/// <summary>
	/// Represents a responsibility link.
	/// </summary>
	public class Responsibility : KAOSElement
	{
		
		public string Id {
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the goal.
		/// </summary>
		/// <value>
		/// The goal.
		/// </value>
		public Goal Goal {
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the agent.
		/// </summary>
		/// <value>
		/// The agent.
		/// </value>
		public Agent Agent {
			get;
			set;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="KaosEditor.Model.Responsibility"/> class.
		/// </summary>
		/// <param name='name'>
		/// Name.
		/// </param>
		/// <param name='goal'>
		/// Goal.
		/// </param>
		/// <param name='agent'>
		/// Agent.
		/// </param>
		public Responsibility (Goal goal, Agent agent)
			: this (Guid.NewGuid().ToString(), goal, agent)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="KaosEditor.Model.Responsibility"/> class.
		/// </summary>
		/// <param name='id'>
		/// Identifier.
		/// </param>
		/// <param name='name'>
		/// Name.
		/// </param>
		/// <param name='goal'>
		/// Goal.
		/// </param>
		/// <param name='agent'>
		/// Agent.
		/// </param>
		public Responsibility (string id, Goal goal, Agent agent)
		{
			Id = id;
			Goal = goal;
			Agent = agent;
		}
		
	}
}

