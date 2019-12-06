using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extinction_Rebellion
{
	public class EventComparer : IEqualityComparer<EventEntry>
	{
		public bool Equals(EventEntry x, EventEntry y)
		{
			if (x == null && y == null)
				return true;

			if (x == null || y == null)
				return false;


			return (x.Equals(y));
		}

		public int GetHashCode(EventEntry obj)
		{
			return obj.ToLine().GetHashCode();
		}
	}
}
