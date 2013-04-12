using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;

namespace Beast.Xml
{
	public class XmlDataContext
	{
		private string connectionString = null;

		/// <summary> the table name refer to the xml data source </summary>
		public string TableName { get; set; }

		public XmlDataContext(string connectionString) 
		{
			this.connectionString = connectionString;
		}

		/// <summary> find node by the id </summary>
		/// <param name="id">id</param>
		/// <returns>matched node</returns>
		public XmlNode FindElementById(int id) 
		{
			XmlNode result = null;
			using (SqlConnection con = new SqlConnection(connectionString))
			{
				string sql = string.Format(@"SELECT * FROM {0} WHERE Id = @id", TableName);
				SqlParameter para = new SqlParameter("@id", id);
				con.Open();
				SqlCommand cmd = new SqlCommand(sql, con);
				cmd.Parameters.Add(para);
				SqlDataReader reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					result = CreateReadedData(reader);
				}
				reader.Close();
				con.Close();
			}
			return result;		
		}

		/// <summary> find xml nodes whitch have same name </summary>
		/// <param name="name">node name</param>
		/// <returns>matched xml nodes</returns>
		public XmlNodeList FindElementsByName(string name) 
		{
			XmlNodeList result = null;
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
			using (SqlConnection con = new SqlConnection(connectionString)) 
			{
				string sql = string.Format(@"SELECT * FROM {0} WHERE NodeName = @name", TableName);
				SqlParameter para = new SqlParameter("@name", name);
				con.Open();
				SqlCommand cmd = new SqlCommand(sql, con);
				cmd.Parameters.Add(para);
				SqlDataReader reader = cmd.ExecuteReader();
				if(reader.HasRows)result = new XmlNodeList();
				while (reader.Read()) 
				{
					result.Add(CreateReadedData(reader));
				}
				reader.Close();
				con.Close();
			}
			return result;
		}

		/// <summary> find xml nodes which have similar content </summary>
		/// <param name="content">node content</param>
		/// <returns>matched xml nodes</returns>
		public XmlNodeList FindElementsByContent(string content) 
		{
			XmlNodeList result = null;
			if (string.IsNullOrEmpty(content)) throw new ArgumentNullException("content");
			using (SqlConnection con = new SqlConnection(connectionString))
			{
				string sql = string.Format(@"SELECT * FROM {0} WHERE Content LIKE @content", TableName);
				SqlParameter para = new SqlParameter("@content", content);
				con.Open();
				SqlCommand cmd = new SqlCommand(sql, con);
				cmd.Parameters.Add(para);
				SqlDataReader reader = cmd.ExecuteReader();
				if (reader.HasRows) result = new XmlNodeList();
				while (reader.Read())
				{
					result.Add(CreateReadedData(reader));
				}
				reader.Close();
				con.Close();
			}
			return result;
		}

		/// <summary> get all child xml nodes of a given node</summary>
		/// <param name="node">given node</param>
		/// <returns>child xml nodes list</returns>
		public XmlNodeList GetAllChildNodes(XmlNode node) 
		{
			XmlNodeList result = null;
			if (node == null) throw new ArgumentNullException("node");
			using (SqlConnection con = new SqlConnection(connectionString))
			{
				string sql = string.Format(@"SELECT * FROM {0} WHERE Layer = @layer AND StartCodeY * @startX > @startY * StartCodeX  AND EndCodeY * @endX < @endY * EndCodeX ORDER BY (StartCodeY / StartCodeX) ASC", TableName);
				SqlParameter[] paras = new SqlParameter[]{
					new SqlParameter("@layer", node.Layer + 1),
					new SqlParameter("@startX", node.Start.X),
					new SqlParameter("@startY", node.Start.Y),
					new SqlParameter("@endX", node.End.X),
					new SqlParameter("@endY", node.End.Y)
				};
				con.Open();
				SqlCommand cmd = new SqlCommand(sql, con);
				cmd.Parameters.AddRange(paras);
				SqlDataReader reader = cmd.ExecuteReader();
				if (reader.HasRows) result = new XmlNodeList();
				while (reader.Read())
				{
					result.Add(CreateReadedData(reader));
				}
				reader.Close();
				con.Close();
			}
			return result;
		}

		/// <summary> get the first child node of a given node</summary>
		/// <param name="node">given node</param>
		/// <returns>first child node</returns>
		public XmlNode GetFirstChild(XmlNode node)
		{
			XmlNodeList nodeList = GetAllChildNodes(node);
			if (nodeList == null) return null;
			else return nodeList.Count > 0 ? nodeList.First() : null;
		}

		/// <summary> get the last child node of a given node</summary>
		/// <param name="node">given node</param>
		/// <returns>last child node</returns>
		public XmlNode GetLastChild(XmlNode node) 
		{
			XmlNodeList nodeList = GetAllChildNodes(node);
			if (nodeList == null) return null;
			else return nodeList.Count > 0 ? nodeList.Last() : null;
		}

		/// <summary> if the given node has child node </summary>
		/// <param name="node">given node</param>
		public bool HasChildNodes(XmlNode node) 
		{
			XmlNodeList nodeList = GetAllChildNodes(node);
			if (nodeList == null) return false;
			else return nodeList.Count > 0;
		}

		/// <summary> get all postertities of the given node </summary>
		/// <param name="node">given node</param>
		/// <returns>posterity nodes list</returns>
		public XmlNodeList GetPosterity(XmlNode node) 
		{
			XmlNodeList result = null;
			if (node == null) throw new ArgumentNullException("node");
			using (SqlConnection con = new SqlConnection(connectionString))
			{
				string sql = string.Format(@"SELECT * FROM {0} WHERE StartCodeY * @startX > @startY * StartCodeX  AND EndCodeY * @endX < @endY * EndCodeX ORDER BY (StartCodeY / StartCodeX) ASC", TableName);
				SqlParameter[] paras = new SqlParameter[]{
					new SqlParameter("@startX", node.Start.X),
					new SqlParameter("@startY", node.Start.Y),
					new SqlParameter("@endX", node.End.X),
					new SqlParameter("@endY", node.End.Y)
				};
				con.Open();
				SqlCommand cmd = new SqlCommand(sql, con);
				cmd.Parameters.AddRange(paras);
				SqlDataReader reader = cmd.ExecuteReader();
				if (reader.HasRows) result = new XmlNodeList();
				while (reader.Read())
				{
					result.Add(CreateReadedData(reader));
				}
				reader.Close();
				con.Close();
			}
			return result;
		}
		
		/// <summary> get all ancestors of the given node </summary>
		/// <param name="node">given node</param>
		/// <returns>ancestor nodes list</returns>
		public XmlNodeList GetAncestor(XmlNode node)
		{
			XmlNodeList result = null;
			if (node == null) throw new ArgumentNullException("node");
			using (SqlConnection con = new SqlConnection(connectionString))
			{
				string sql = string.Format(@"SELECT * FROM {0} WHERE StartCodeY * @startX < @startY * StartCodeX  AND EndCodeY * @endX > @endY * EndCodeX", TableName);
				SqlParameter[] paras = new SqlParameter[]{
					new SqlParameter("@startX", node.Start.X),
					new SqlParameter("@startY", node.Start.Y),
					new SqlParameter("@endX", node.End.X),
					new SqlParameter("@endY", node.End.Y)
				};
				con.Open();
				SqlCommand cmd = new SqlCommand(sql, con);
				cmd.Parameters.AddRange(paras);
				SqlDataReader reader = cmd.ExecuteReader();
				if (reader.HasRows) result = new XmlNodeList();
				while (reader.Read())
				{
					result.Add(CreateReadedData(reader));
				}
				reader.Close();
				con.Close();
			}
			return result;
		}

		/// <summary> get the direct parent node of the given node </summary>
		/// <param name="node">given node</param>
		/// <returns>the direct parent node</returns>
		public XmlNode GetParentNode(XmlNode node)
		{
			XmlNodeList ancestors = GetAncestor(node);
			if (ancestors == null) return null;
			foreach (XmlNode n in ancestors) 
			{
				if (n.Layer == node.Layer - 1) return n;
			}
			return null;
		}

		/// <summary> Get the least common ancestor of two given nodes </summary>
		/// <param name="a">node a</param>
		/// <param name="b">node b</param>
		/// <returns>the least common ancestor node</returns>
		public XmlNode GetLCA(XmlNode a, XmlNode b)
		{
			XmlNodeList result = null;
			if (a == null) throw new ArgumentNullException("a");
			if (b == null) throw new ArgumentNullException("b");
			if (a.Start > b.Start) 
			{
				XmlNode tmp = a; a = b; b = tmp;
			}
			using (SqlConnection con = new SqlConnection(connectionString))
			{
				string sql = string.Format(@"SELECT * FROM {0} WHERE StartCodeY * @startX <= @startY * StartCodeX  AND EndCodeY * @endX >= @endY * EndCodeX ORDER BY (StartCodeY / StartCodeX) DESC", TableName);
				SqlParameter[] paras = new SqlParameter[]{
					new SqlParameter("@startX", a.Start.X),
					new SqlParameter("@startY", a.Start.Y),
					new SqlParameter("@endX", b.End.X),
					new SqlParameter("@endY", b.End.Y)
				};
				con.Open();
				SqlCommand cmd = new SqlCommand(sql, con);
				cmd.Parameters.AddRange(paras);
				SqlDataReader reader = cmd.ExecuteReader();
				if (reader.HasRows) result = new XmlNodeList();
				while (reader.Read())
				{
					result.Add(CreateReadedData(reader));
				}
				reader.Close();
				con.Close();
			}
			if (result != null && result.Count > 0) return result[0];
			else return null;
		}

		/// <summary> Get the next sibling node of the given nodes </summary>
		/// <param name="node">given node</param>
		/// <returns>next sibling node</returns>
		public XmlNode GetNextSibling(XmlNode node)
		{
			XmlNode parentNode = GetParentNode(node);
			if(parentNode == null)return null;
			XmlNodeList list = GetAllChildNodes(parentNode);
			if(list == null)return null;

			foreach (XmlNode sib in list) 
			{
				if (sib.Start > node.Start) return sib;
			}
			return null;
		}

		/// <summary> Get the previous node of the given nodes </summary>
		/// <param name="node">given node</param>
		/// <returns>the previous node</returns>
		public XmlNode GetPreviousSibling(XmlNode node)
		{
			XmlNode parentNode = GetParentNode(node);
			if (parentNode == null) return null;
			XmlNodeList list = GetAllChildNodes(parentNode);
			if (list == null) return null;

			for(int i = list.Count - 1; i >= 0; i--)
			{
				if (list[i].Start < node.Start) return list[i];
			}
			return null;
		}

		/// <summary> append child node for the fisrt child of given parent node </summary>
		/// <param name="node"></param>
		/// <param name="newChild"></param>
		public void AppendChild(XmlNode node, XmlNode newChild) 
		{
			if (node == null) throw new ArgumentNullException("node");
			Vector start = node.Start;
			Vector end = default(Vector);
			XmlNode firstChild = this.GetFirstChild(node);
			if(firstChild == null)end = node.End;
			else end = firstChild.Start;
			newChild.Start = end + 2 * start;
			newChild.End = start + 2 * end;
			newChild.Start.Reduction();
			newChild.End.Reduction();

			using (SqlConnection con = new SqlConnection(connectionString))
			{
				string sql = string.Format("INSERT INTO {0} VALUES(null, @layer, @startX, @startY, @endX, @endY, @name, @type, @content)", TableName);
				SqlCommand cmd = new SqlCommand(sql, con);
				SqlParameter[] para = new SqlParameter[]{
					new SqlParameter("@layer", node.Layer+1),
					new SqlParameter("@startX", node.Start.X),
					new SqlParameter("@startY", node.Start.Y),
					new SqlParameter("@endX", node.End.X),
					new SqlParameter("@endY", node.End.Y),
					new SqlParameter("@name", newChild.Name),
					new SqlParameter("@type", 1),
					new SqlParameter("@content", newChild.Content)
				};
				cmd.Parameters.AddRange(para);
				con.Open();
				cmd.ExecuteNonQuery();
				con.Close();
			}
		}

		public void InsertAfter(XmlNode node, XmlNode newChild, XmlNode refChild)
		{ 
			 
		}

		public void InsertBefore(XmlNode node, XmlNode newChild, XmlNode refChild)
		{ 
		
		}

		public void ReOrder() 
		{ 
		
		}

		public void RemoveAll() 
		{ 
		
		}

		public void RemoveNode(XmlNode node) 
		{ 
		
		}

		public void UpdateNode(XmlNode oldNode, XmlNode newNode) 
		{ 
		
		}


		public void ExportToFile(string path)
		{

		}

		public void ExportToStream(Stream stream)
		{

		}

		public XmlNode[] SelectNodes(string xpath) 
		{
			return null;
		}

		public XmlNode SelectSingleNode(string xpath)
		{
			return null;
		}

		private XmlNode CreateReadedData(SqlDataReader reader) 
		{ 
			XmlNode result = new XmlNode();
			result.Id = reader.GetInt32(0);
			result.Layer = reader.GetInt32(1);
			result.Start = new Vector(reader.GetInt32(2), reader.GetInt32(3));
			result.End = new Vector(reader.GetInt32(4), reader.GetInt32(5));
			result.Name = reader.GetString(6);
			result.Type = reader.GetInt32(7);
			result.Content = reader.GetString(8);
			return result;
		}
	}
}
