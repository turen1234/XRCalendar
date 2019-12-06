using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Extinction_Rebellion
{
	/// <summary>
	/// Interaction logic for DateTile.xaml
	/// </summary>
	public partial class DateTile : UserControl
	{

		public DateTime Date;

		public DateTile(DateTime d)
		{

			InitializeComponent();

			if (d.Date == DateTime.Now.Date)
				DateTitle.Background = Brushes.LightGreen;


			DateTitle.Text = d.ToString("ddd d");



			GridView myGridView = new GridView();
			myGridView.AllowsColumnReorder = true;

			GridViewColumn gvc1 = new GridViewColumn();
			gvc1.DisplayMemberBinding = new Binding("Time");
			gvc1.Header = "Time";
			gvc1.Width = 40;
			myGridView.Columns.Add(gvc1);
			GridViewColumn gvc2 = new GridViewColumn();
			gvc2.DisplayMemberBinding = new Binding("Group");
			gvc2.Header = "Group";
			gvc2.Width = 50;
			myGridView.Columns.Add(gvc2);


			GridViewColumn gvc3 = new GridViewColumn();
			gvc3.DisplayMemberBinding = new Binding("Description");
			gvc3.Header = "Description";

			myGridView.Columns.Add(gvc3);
			

			DayEvents.View = myGridView;

			DayEvents.MouseDoubleClick += DoubleClick;

			Date = d;

			DayEvents.ItemsSource = Events;

			CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(DayEvents.ItemsSource);

			view.SortDescriptions.Add(new SortDescription("Time", ListSortDirection.Ascending));

		}

		private void DoubleClick(object sender, MouseButtonEventArgs e)
		{


			var line = (((ListView)sender).SelectedValue) as EventEntry;

			var link = line?.Link ;

			if (string.IsNullOrWhiteSpace(link))
				return;

			System.Diagnostics.Process.Start("https://facebook.com" + link);


		}

		private ObservableCollection<EventEntry> Events = new ObservableCollection<EventEntry>();

		private EventComparer comparer = new EventComparer();

		internal void AddEvent(EventEntry ev)
		{
			this.Dispatcher.Invoke(() =>
			{
				if (!Events.Contains(ev, comparer))
					Events.Add(ev);

				//Events = new ObservableCollection<EventEntry>( Events.OrderBy(x => x.Date));
			});

		}

		internal void Add(IEnumerable<EventEntry> events)
		{

			this.Dispatcher.Invoke(() =>
			{
				foreach (var e in events)
				{

					if(!Events.Contains(e,comparer))
						Events.Add(e);

					//Events = new ObservableCollection<EventEntry>(Events.OrderBy(x => x.Date));
				}
			});
		}

		private void DateTitle_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if(e.ClickCount ==2)
			{
				var d = new DateInspector(Date, Events.ToArray());

				d.Show();
			}
		}
	}
}
