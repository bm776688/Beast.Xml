using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Beast.Xml;

namespace TestXmlCoder
{
	class Program
	{
		static XmlDataContext context;

		static void TestParseXml()
		{
			string connectionString = @"data source=6CR251WSQC\BEAST;initial catalog=Clover;integrated security=True;multipleactiveresultsets=True;";
			string path = Environment.CurrentDirectory + @"\Xml\bib_0.xml";
			XmlParser parser = new XmlParser(path, connectionString);
			parser.Parse("TestTable");
			Console.ReadLine();	
		}

		#region Test Query Xml Data Context

		private static void TestFindByName() 
		{
			XmlNodeList list = context.FindElementsByName("title");
			foreach (XmlNode node in list) Console.WriteLine(node);
			Console.ReadLine();		
		}

		private static void TestFindByContent() 
		{
			XmlNodeList list = context.FindElementsByContent("amazon");
			foreach (XmlNode node in list) Console.WriteLine(node);
			Console.ReadLine();
		}

		private static void TestGetChild()
		{
			XmlNodeList list = context.FindElementsByName("book");
			Console.WriteLine(context.GetFirstChild(list[0]));
			Console.WriteLine(context.GetLastChild(list[0]));
			Console.WriteLine(context.HasChildNodes(list[0]));
			list = context.GetAllChildNodes(list[0]);
			foreach (XmlNode node in list) Console.WriteLine(node);
			Console.ReadLine();
		}

		private static void TestGetPosterity()
		{
			XmlNodeList list = context.FindElementsByName("book");
			list = context.GetPosterity(list[0]);
			foreach (XmlNode node in list) Console.WriteLine(node);
			Console.ReadLine();
		}

		private static void TestGetParent() {
			XmlNodeList list = context.FindElementsByName("book");
			Console.WriteLine(context.GetParentNode(list[0]));
			Console.ReadLine();		
		}

		private static void TestGetAncestor()
		{
			XmlNodeList list = context.FindElementsByName("book");
			list = context.GetAncestor(list[0]);
			foreach (XmlNode node in list) Console.WriteLine(node);
			Console.ReadLine();
		}

		private static void TestGetLCA() 
		{
			XmlNodeList list = context.FindElementsByName("book");
			XmlNode lca = context.GetLCA(list[0], list[1]);
			Console.WriteLine(lca);

			lca = context.GetLCA(list[0], context.GetFirstChild(list[0]));
			Console.WriteLine(lca);
			Console.ReadLine();		
		}

		private static void TestGetSibling() 
		{
			XmlNodeList list = context.FindElementsByName("book");
			XmlNode pre = context.GetPreviousSibling(list[0]);
			Console.WriteLine(pre);

			XmlNode next = context.GetNextSibling(list[0]);
			Console.WriteLine(next);
			Console.ReadLine();				
		}


		static void TestOperatorXml() 
		{
			string connectionString = @"data source=6CR251WSQC\BEAST;initial catalog=Clover;integrated security=True;multipleactiveresultsets=True;";
			context = new XmlDataContext(connectionString);
			context.TableName = "TestTable";

			// TestFindByName();
			// TestFindByContent();

			// TestGetChild();
			// TestGetPosterity();

			// TestGetParent();
			// TestGetAncestor();
			// TestGetLCA();

			TestGetSibling();
		}

		#endregion

		#region Test Modify Xml Data Context

		private static void TestAddNode()
		{
			XmlNode newNode = new XmlNode();
			newNode.Content = "Beast Book";
			newNode.Name = "title";
			XmlNode node = context.FindElementById(6);
			context.AppendChild(node, newNode);
		}

		#endregion

		static void Main(string[] args)
		{
			// TestParseXml();
			TestOperatorXml();
			TestAddNode();
		}
	}
}
