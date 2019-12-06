using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Extinction_Rebellion
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{


		List<DateTile> Tiles = new List<DateTile>();

		DateTime Selected = DateTime.Now.Date;

		public MainWindow()
		{
			InitializeComponent();


			ShowDonationMessage();

			CheckForUpdates(false);

			RefreshCalendar();

		}

		private async Task RefreshCalendar()
		{
			EventProvider.Download();
			LastUpdated.Content = EventProvider.LastSynced;

			Dispatcher.Invoke(() =>
			{
				ShowMonth(Selected);
			});
		}

		private void ShowMonth(DateTime selected)
		{

			foreach(var t in Tiles)
			{
				myGrid.Children.Remove(t);
			}

			var all = EventProvider.All;

			MonthLabel.Text = selected.ToString("MMMM");

			int dayCounter = 0;

			foreach (var date in AllDatesInMonth(selected.Year, selected.Month))
			{
				var week = (int)Math.Floor((float)dayCounter / 7);
				var calendarDay = new DateTile(date);
				Tiles.Add(calendarDay);
				myGrid.Children.Add(calendarDay);
				Grid.SetColumn(calendarDay, dayCounter % 7);
				Grid.SetRow(calendarDay, week);

				var events = all.Where(x => x.IsOn(date));

				calendarDay.Add(events);


				dayCounter++;
			}
		}

		private const string RemoteVersion = "https://raw.githubusercontent.com/turen1234/XRCalendar/master/Updates/version.txt";
		private const string RemoteInstall = "https://raw.githubusercontent.com/turen1234/XRCalendar/master/XR-Calendar.zip";

		private async Task CheckForUpdates(bool showMessage)
		{
			try
			{
				using (WebClient client = new WebClient())
				{
					var version = client.DownloadString(RemoteVersion)
						.Trim()
						.Replace("*", "");

					var latest = Version.Parse(version);

					var current = Assembly.GetExecutingAssembly().GetName().Version;


					if (latest > current)
					{
						var result = MessageBox.Show($"A new version is avaiable ({version.ToString()})\n Would you like to update now?", "Update Available", MessageBoxButton.OKCancel);

						if (result == MessageBoxResult.OK)
						{

							Process.Start(new ProcessStartInfo(RemoteInstall));
							//UpdateVersion(client);
						}
					}
					else if(showMessage)
					{
						MessageBox.Show($"You are update to date with the latest version ({current.ToString()})");
					}

				}
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message,"Error");
			}


		}

		string ReceiptUrl
		{
			get
			{
				try
				{
					return Properties.Settings.Default["receipt_url"] as string;
				}
				catch(Exception e)
				{
					return null;
				}
			}

		}

		private bool Donated => !string.IsNullOrWhiteSpace(ReceiptUrl);


		private void ShowDonationMessage()
		{
			UpdateDonationMenu();

			if (!Donated)
			{
				var result = new AlertWindow().ShowDialog();

				if (result ?? false)
				{

				}
			}
		}

		private void UpdateDonationMenu()
		{
			if (Donated)
			{
				ShowReceipt.IsEnabled = true;
				return;
			}
		}

		private void AddEvent(EventEntry ev)
		{
			var eventDay = ev.Date.Value.Date;
			var day = Tiles.Single(x => x.Date == eventDay);

			day.AddEvent(ev);

		}

		public static IEnumerable<DateTime> AllDatesInMonth(int year, int month)
		{
			int days = DateTime.DaysInMonth(year, month);
			for (int day = 1; day <= days; day++)
			{
				yield return new DateTime(year, month, day);
			}
		}

		private void Donate_Click(object sender, RoutedEventArgs e)
		{
			var donationWindow = new DonationForm();

			var result = donationWindow.ShowDialog();

			if (result ?? false)
			{

			}

			UpdateDonationMenu();
		}

		private void OpenReceipt(object sender, RoutedEventArgs e)
		{
			Process.Start(ReceiptUrl);
		}

	
		private void Btn_Prev_Click(object sender, RoutedEventArgs e)
		{
			Selected = Selected.AddMonths(-1);
			ShowMonth(Selected);

		}

		private void Btn_Next_Click(object sender, RoutedEventArgs e)
		{
			Selected = Selected.AddMonths(1);
			ShowMonth(Selected);
		}

		private void Support_Click(object sender, RoutedEventArgs e)
		{

			Process.Start(new ProcessStartInfo("https://github.com/turen1234/XRCalendar/issues"));

		}

		private void Contact_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(new ProcessStartInfo("mailto:Urensoft@outlook.com?subject=XR Calendar for Windows"));

		}

		private void Refresh_Click(object sender, RoutedEventArgs e)
		{
			RefreshCalendar();
		}

		private void Update_Click(object sender, RoutedEventArgs e)
		{
			CheckForUpdates(true);
		}

		private void UpdateVersion(WebClient client)
		{
			var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

			Directory.CreateDirectory(temp);


			var fn = Path.Combine(temp, "install.zip");

			client.DownloadFile(new Uri(RemoteInstall), fn);

			ZipFile.ExtractToDirectory(fn, temp);

			var installer = Path.Combine(temp, "setup.exe");


			if (!File.Exists(installer))
			{
				Directory.Delete(temp, true);
				throw new FileNotFoundException("Installer was not downloaded successfully");
			}

			var psi = new ProcessStartInfo( installer );

			Process.Start(psi);

			Environment.Exit(0);

		}
	}
}
