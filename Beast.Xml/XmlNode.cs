using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beast.Xml
{
	public class XmlNode
	{
		public Vector Start { get; set; }

		public Vector End { get; set; }

		public string Name { get; set; }

		public string Content { get; set; }

		public int Type { get; set; }

		public int Id { get; set; }

		public int Layer { get; set; }

		public override string ToString()
		{
			return string.Format("id={0} code=<{1},{2}> layer={3} name={4} Content={5}", Id, Start, End, Layer, Name, Content);
		}

	}

	public class XmlNodeList : List<XmlNode> 
	{ 
	
	}

	public class XmlAttribute 
	{
		public string Property { get; set; }

		public string Value { get; set; }
	}

}
