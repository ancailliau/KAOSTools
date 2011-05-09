using System;
using System.Xml;

namespace Editor
{
	public static class Helpers
	{
		
		public static bool IsEndElement(this XmlReader reader, string name) {
			return reader.NodeType == XmlNodeType.EndElement && reader.Name == name;
		}
		
	}
}

