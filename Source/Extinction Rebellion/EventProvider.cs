using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Extinction_Rebellion
{
	public class EventProvider
	{

		const int SECOND = 1;
		const int MINUTE = 60 * SECOND;
		const int HOUR = 60 * MINUTE;
		const int DAY = 24 * HOUR;
		const int MONTH = 30 * DAY;


		public static string TimeAgo(DateTime dt)
		{


			var ts = new TimeSpan(DateTime.UtcNow.Ticks - dt.Ticks);
			double delta = Math.Abs(ts.TotalSeconds);

			if (delta < 1 * MINUTE)
				return ts.Seconds == 1 ? "a second ago" : ts.Seconds + " seconds ago";

			if (delta < 2 * MINUTE)
				return "a minute ago";

			if (delta < 45 * MINUTE)
				return ts.Minutes + " minutes ago";

			if (delta < 90 * MINUTE)
				return "an hour ago";

			if (delta < 24 * HOUR)
				return ts.Hours + " hours ago";

			if (delta < 48 * HOUR)
				return "yesterday";

			if (delta < 30 * DAY)
				return ts.Days + " days ago";

			if (delta < 12 * MONTH)
			{
				int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
				return months <= 1 ? "1 month ago" : months + " months ago";
			}
			else
			{
				int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
				return years <= 1 ? "1 year ago" : years + " years ago";
			}

		}

		public static string LastSynced { get; private set; }
		private static string CSV = "";
		internal static void Download()
		{

			try
			{
				using (WebClient client = new WebClient())
				{
					CSV = client.DownloadString("https://raw.githubusercontent.com/turen1234/XRCalendar/master/Calendar.csv");
					var updated = client.DownloadString("https://raw.githubusercontent.com/turen1234/XRCalendar/master/updated.txt");


					if (string.IsNullOrWhiteSpace(updated))
						return;

					var cols = updated.Split(',');

					if (cols.Count() != 2)
						throw new Exception("Unable to retrieve sync information");

					var date = TimeAgo(DateTime.Parse(cols[0]));
					var newitems = cols[1];

					LastSynced = $"Last Sync: {date},\t{newitems} new events found";
				}
			}
			catch (Exception e)
			{

				MessageBox.Show(e.Message);

				Environment.Exit(1);
			}


		}


		public static EventEntry[] All
		{
			get
			{
				var comparer = new EventComparer();

				return CSV
					.Split('\n')
					.Where(y => !string.IsNullOrWhiteSpace(y))
					.Select(x => new EventEntry(x))
					.Where(x => x.IsValid)
					.Distinct(comparer)
					.ToArray();
			}
		}

	}
}
