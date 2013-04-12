using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data.SqlClient;

namespace Beast.Xml
{
	public class XmlParser
	{
		private XmlDocument doc = null;
		private string connectionString = null;
		private string tableName = null;

		private int id;
		private int codeId;
		private int layer;
		private SqlConnection con = null;

		public bool IsLoaded { get { return doc == null; } }

		public XmlParser() { }

		public XmlParser(string path, string connectionStr) 
		{
			Load(path);
			connectionString = connectionStr;
		}

		public XmlParser(XmlReader reader) 
		{
			Load(reader);
		}

		public void Load(string path) 
		{
			doc = new XmlDocument();
			try
			{
				doc.Load(path);
			}
			catch(Exception e) {
				doc = null;
				throw e;
			}
		}

		public void Load(XmlReader reader)
		{
			doc = new XmlDocument();
			try
			{
				doc.Load(reader);
			}
			catch (Exception e)
			{
				doc = null;
				throw e;
			}
		}

		public void Parse(string tableName) 
		{
			if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("doesn't config the connecting string");
			if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException(tableName);

			this.tableName = tableName;
			using (con = new SqlConnection(connectionString)) 
			{
				// check if the table exist
				string sql = string.Format(@"SELECT * FROM sysobjects WHERE objectproperty(object_id('{0}'),'istable') = 1", tableName);
				con.Open();
				SqlCommand command = new SqlCommand(sql, con);
				SqlDataReader reader = command.ExecuteReader();
				bool exist = reader.HasRows;
				reader.Close();
				con.Close();

				// create table
				if (exist == false)
				{
					sql = string.Format(@"CREATE TABLE {0} (Id int, Layer int, StartCodeX int, StartCodeY int, EndCodeX int, EndCodeY int, NodeName nvarchar(100), NodeType int, Content ntext)", tableName);
					con.Open();
					command = new SqlCommand(sql, con);
					command.ExecuteNonQuery();
					con.Close();
				}
				else{
					sql = string.Format(@"DROP TABLE {0}", tableName);
					con.Open();
					command = new SqlCommand(sql, con);
					command.ExecuteNonQuery();
					sql = string.Format(@"CREATE TABLE {0} (Id int, Layer int, StartCodeX int, StartCodeY int, EndCodeX int, EndCodeY int, NodeName nvarchar(100), NodeType int, Content ntext)", tableName);
					command = new SqlCommand(sql, con);
					command.ExecuteNonQuery();
					con.Close();
				}
				// else throw new ArgumentException(string.Format("the table named {0} is already exsist, please rename it"));
			}

			id = 0;
			layer = 0;
			codeId = 0;
			using (con = new SqlConnection(connectionString))
			{
				con.Open();
				Traverse(doc.DocumentElement);
				con.Close();
			}
		}

		private void Traverse(System.Xml.XmlNode node) 
		{
			id++;
			layer++;
			int curId = id;
			codeId++;
			InsertNode(node, curId, new Vector(0, codeId));
			foreach (System.Xml.XmlNode child in node.ChildNodes) 
			{
				if (child is XmlElement)
				{
					Console.WriteLine((child as XmlElement).Name);
					Traverse(child);
				}
			}
			layer--;
			codeId++;
			ModifyNode(node, curId, new Vector(0, codeId));
		}

		private void InsertNode(System.Xml.XmlNode node, int id, Vector v) 
		{
			string sql = null;
			if(node is XmlElement)
			{
				XmlElement ele = node as XmlElement;
				string content = "";
				if (ele.HasChildNodes && ele.FirstChild is XmlText) {
					content = ele.InnerText;
				}
				sql = string.Format("INSERT INTO {0} VALUES(@id, @layer, @startX, @startY, @endX, @endY, @name, @type, @content)", tableName);
				SqlCommand cmd = new SqlCommand(sql, con);
				SqlParameter[] para = new SqlParameter[]{
					new SqlParameter("@id", id),
					new SqlParameter("@layer", layer),
					new SqlParameter("@startX", 1),
					new SqlParameter("@startY", codeId),
					new SqlParameter("@endX", 1),
					new SqlParameter("@endY", 1),
					new SqlParameter("@name", ele.Name),
					new SqlParameter("@type", 1),
					new SqlParameter("@content", content),					
				};
				cmd.Parameters.AddRange(para);
				cmd.ExecuteNonQuery();
			}
		}

		private void ModifyNode(System.Xml.XmlNode node, int id, Vector v)
		{
			string sql = null;
			if (node is XmlElement)
			{
				XmlElement ele = node as XmlElement;
				sql = string.Format("UPDATE {0} SET EndCodeX='{1}', EndCodeY='{2}' WHERE id = {3}", tableName, 1, codeId, id);
				SqlCommand cmd = new SqlCommand(sql, con);
				cmd.ExecuteNonQuery();
			}
		}
	}

}
