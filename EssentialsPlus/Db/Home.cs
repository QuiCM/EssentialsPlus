using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EssentialsPlus.Db
{
	public class Home
	{
		public string Name { get; set; }
		public int UserID { get; set; }
		public float X { get; set; }
		public float Y { get; set; }

		public Home(int userId, string name, float x, float y)
		{
			Name = name;
			UserID = userId;
			X = x;
			Y = y;
		}
	}
}
