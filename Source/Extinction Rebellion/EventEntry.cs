using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using HtmlAgilityPack;

namespace Extinction_Rebellion
{



	[Serializable]
	public class EventEntry : IEquatable<EventEntry>
	{

		public string ID { get; set; }
		public DateTime? Date { get; set; }
		public string Description { get; set; }
		public string Link { get; set; }
		public string Time { get; set; }
		public string Group { get; set; }

		public EventEntry(string currentGroup, HtmlNodeCollection date, HtmlNodeCollection desc, HtmlNodeCollection link, HtmlNodeCollection location)
		{

			Group = currentGroup;
			var dates = date.Select(x => x?.InnerText)
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.ToArray();


			if(desc != null)
			{
				Description = desc.Select(x => x.InnerText).FirstOrDefault();

				var m = Regex.Match(Description, @"\d\d\:\d\d");

				if(m.Success)
				{
					Time = m.Value;
				}

			}

			if (dates.Count() >= 2)
			{
				var ds = $"{dates[0]} {dates[1]}";

				DateTime found;

				bool parsed = false;

				if (DateTime.TryParseExact(ds, "MMM dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out found))
				{
					parsed = true;
				}
				else if (DateTime.TryParseExact(ds, "MMM d", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out found))
				{
					parsed = true;
				}

				if (parsed)
				{

					if (Time != null)
					{
						var ts = TimeSpan.ParseExact(Time, "h\\:mm", CultureInfo.InvariantCulture);

						Date = new DateTime(found.Year, found.Month, found.Day, ts.Hours, ts.Minutes, 0);

					}
					else
					{
						Date = found;
					}
				}
				else
				{

				}
			}

			if(link != null)
			{
				Link = link.First().Attributes["href"].Value;


				IDFromLink();
			}

		}

		private void IDFromLink()
		{
			if (String.IsNullOrWhiteSpace(Link))
				return;


			var match = Regex.Match(Link, @"\/events\/\d+\/");

			if (match.Success)
				ID = Regex.Match(match.Value, @"\d+").Value;
		}

		public EventEntry(string x)
		{

			var cols = x.Split(',');

			var time = cols.ElementAtOrDefault(0);
			var date = cols.ElementAtOrDefault(1);
			var group = cols.ElementAtOrDefault(2);
			var desc = cols.ElementAtOrDefault(3);
			var link = cols.ElementAtOrDefault(4);


			Time = time;
			Date = DateTime.Parse(date).ToLocalTime();
			Group = Base64Decode(group);
			Description = Base64Decode(desc);
			Link = Base64Decode(link);

			IDFromLink();

		}

		public string ToLine()
		{
			var desc = Base64Encode(Description);
			var date = Date.Value.ToUniversalTime();
			var group = Base64Encode(Group);
			var link = Base64Encode(Link);
			return $"{Time},{date},{group},{desc},{link}\n";
		}


		

		internal bool IsOn(DateTime date)
		{
			return Date.Value.Date == date.Date;
		}

		public static string Base64Encode(string plainText)
		{
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}

		public static string Base64Decode(string base64EncodedData)
		{
			var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
			return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
		}


	
		public bool IsValid {
			get {
				return Date.HasValue && !string.IsNullOrWhiteSpace("Description");
			}
		}

		public override string ToString()
		{
			return Description;
		}

		public bool Equals(EventEntry other)
		{

			if (!string.IsNullOrWhiteSpace(ID) && !string.IsNullOrWhiteSpace(other?.ID))
				return ID == other?.ID;

			var time = Time == other?.Time;
			var dates = Date.Value.Date == other?.Date.Value.Date;

			var desc = TextCompare(Description, other?.Description);


			return time && dates && desc;
		}

		private bool TextCompare(string description1, string description2)
		{
			var a = Regex.Replace(description1, @"\d+", "", RegexOptions.Multiline);

			var b = Regex.Replace(description2, @"\d+", "", RegexOptions.Multiline);


			return a == b;

		}
	}
}
