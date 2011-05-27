// 
// EditorConfiguration.cs
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

using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Beaver.Logging;

namespace Beaver
{
	
	/// <summary>
	/// Represents the editor configuration.
	/// </summary>
	public class EditorConfiguration
	{
		
		/// <summary>
		/// Gets or sets a value indicating whether main window is maximized.
		/// </summary>
		/// <value>
		/// <c>true</c> if maximized; otherwise, <c>false</c>.
		/// </value>
		public bool Maximized {
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the last opened file
		/// </summary>
		/// <value>
		/// The last opened.
		/// </value>
		public string LastOpenedFilename {
			get;
			set;
		}
		
		/// <summary>
		/// Gets or sets the plugins.
		/// </summary>
		/// <value>
		/// The plugins.
		/// </value>
		public List<Plugin> Plugins {
			get;
			set;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Beaver.EditorConfiguration"/> class.
		/// </summary>
		public EditorConfiguration ()
		{
			Maximized = false;
			Plugins = new List<Plugin> ();
			LastOpenedFilename = "";
		}
		
		/// <summary>
		/// Adds a new plugin with the specified name, path and whether it is enabled.
		/// </summary>
		/// <param name='name'>
		/// Name.
		/// </param>
		/// <param name='path'>
		/// Path.
		/// </param>
		/// <param name='enabled'>
		/// true if enabled, false otherwise
		/// </param>
		public void AddPlugin (string name, string path, bool enabled)
		{
			Plugins.Add (new Plugin() { Name = name, Path = path, Enabled = enabled });
		}
		
		/// <summary>
		/// Save the configuration to the given filename.
		/// </summary>
		/// <param name='filename'>
		/// Filename.
		/// </param>
		public void SaveToFile (string filename)
		{
			var settings = new XmlWriterSettings();
			settings.Indent = true;
			
			using (var writer = XmlWriter.Create(filename, settings)) {
				XmlSerializer x = new XmlSerializer(this.GetType());
				x.Serialize (writer, this);
			}
			
			Logger.Info ("Configuration saved to '{0}'", filename);
		}
		
		/// <summary>
		/// Loads the configuration from the given filename
		/// </summary>
		/// <returns>
		/// The loaded configuration.
		/// </returns>
		/// <param name='filename'>
		/// Filename.
		/// </param>
		public static EditorConfiguration LoadFromFile (string filename)
		{
			XmlSerializer mySerializer = new XmlSerializer(typeof(EditorConfiguration));
			FileStream myFileStream = new FileStream(filename, FileMode.Open);
			var temp = (EditorConfiguration) mySerializer.Deserialize(myFileStream);
			myFileStream.Close();
			
			Logger.Info ("Configuration loaded from '{0}'", filename);	
			
			return temp;
		}
		
	}
}

