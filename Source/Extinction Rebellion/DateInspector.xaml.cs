using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Extinction_Rebellion
{
    /// <summary>
    /// Interaction logic for DateInspector.xaml
    /// </summary>
    public partial class DateInspector : Window
    {
        public DateInspector(DateTime date, EventEntry[] Events)
        {
            InitializeComponent();


			Title = date.ToString("ddd d MMMM yyyy");


			/*GridView myGridView = new GridView();
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

			EventList.View = myGridView;
			*/

			EventList.ItemsSource = Events;


			CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(EventList.ItemsSource);

			view.SortDescriptions.Add(new SortDescription("Time", ListSortDirection.Ascending));

			view.Refresh();

		}
	}
}
