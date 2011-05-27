// 
// Logger.cs
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

namespace Beaver.Logging
{
	
	/// <summary>
	/// Provides logging facilities
	/// </summary>
	public static class Logger
	{
		
		/// <summary>
		/// Display the specified debug message.
		/// </summary>
		/// <param name='message'>
		/// Message.
		/// </param>
		public static void Debug (string message)
		{
			Console.WriteLine ("[DEBUG {0}]", message);
		}
		
		/// <summary>
		/// Display the specified debug message.
		/// </summary>
		/// <param name='message'>
		/// Message.
		/// </param>
		public static void Debug (string message, params object[] param)
		{
			Debug (string.Format(message, param));
		}
		
		/// <summary>
		/// Display the specified info message.
		/// </summary>
		/// <param name='message'>
		/// Message.
		/// </param>
		public static void Info (string message)
		{
			Console.WriteLine ("[INFO  {0}]", message);
		}
		
		/// <summary>
		/// Display the specified info message.
		/// </summary>
		/// <param name='message'>
		/// Message.
		/// </param>
		public static void Info (string message, params object[] param)
		{
			Info (string.Format(message, param));
		}
		
		/// <summary>
		/// Display the specified warning message.
		/// </summary>
		/// <param name='message'>
		/// Message.
		/// </param>
		public static void Warning (string message)
		{
			Console.WriteLine ("[WARN  {0}]", message);
		}
		
		/// <summary>
		/// Display the specified warning message.
		/// </summary>
		/// <param name='message'>
		/// Message.
		/// </param>
		public static void Warning (string message, params object[] param)
		{
			Warning (string.Format(message, param));
		}
		
		/// <summary>
		/// Display the specified error message.
		/// </summary>
		/// <param name='message'>
		/// Message.
		/// </param>
		public static void Error (string message)
		{
			Console.WriteLine ("[ERROR {0}]", message);
		}
		
		/// <summary>
		/// Display the specified error message.
		/// </summary>
		/// <param name='message'>
		/// Message.
		/// </param>
		public static void Error (string message, params object[] param)
		{
			Error (string.Format(message, param));
		}
		
	}
}

