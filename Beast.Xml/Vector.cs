using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beast.Xml
{
	public struct Vector
	{
		private int x;
		private int y;

		public int X { get { return x; } set { x = value; } }
		public int Y { get { return y; } set { y = value; } }

		public Vector(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public override string ToString()
		{
			return string.Format("({0},{1})", x, y);
		}

		public static bool operator <(Vector a, Vector b) 
		{
			return a.y * b.x < b.y * a.x;
		}

		public static bool operator >(Vector a, Vector b) 
		{
			return a.y * b.x > b.y * a.x;
		}

		public static Vector operator +(Vector a, Vector b)
		{
			return new Vector(a.x + b.x, a.y + b.y);
		}

		public static Vector operator *(Vector a, int b) 
		{
			return new Vector(a.x * b, a.y * b);
		}

		public static Vector operator *(int a, Vector b) 
		{
			return new Vector(a * b.x, a * b.y);
		}

		public void Reduction() 
		{
			int a = x, b = y;
			while (b
				> 0) {
				a = a % b;
				b ^= a;
				a ^= b;
				b ^= a;
			}
			x /= a;
			y /= a;
		}
	}
}
