using System;
using System.Xml;
using Cairo;

namespace Beaver
{
	public static class Helpers
	{
		
		public static bool IsEndElement(this XmlReader reader, string name) {
			return reader.NodeType == XmlNodeType.EndElement && reader.Name == name;
		}
		
		public static void SetColor (this Context context, string hexa) 
		{
			Gdk.Color col = new Gdk.Color(0,0,0);			
			Gdk.Color.Parse(hexa, ref col);
			context.SetSourceRGB (col.Red/65535.0, col.Green/65535.0, col.Blue/65535.0);
		}
		
		public static void SetColor (this Context context, string hexa, float opacity) 
		{
			Gdk.Color col = new Gdk.Color(0,0,0);			
			Gdk.Color.Parse(hexa, ref col);
			context.SetSourceRGBA (col.Red/65535.0, col.Green/65535.0, col.Blue/65535.0, opacity);
		}
	}
}

