using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
	/// Interaction logic for AlertWindow.xaml
	/// </summary>
	public partial class AlertWindow : Window
	{
		public AlertWindow()
		{
			InitializeComponent();



		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Task.Run(Countdown);
		}

		public async Task Countdown()
		{

			var waitTime = 5;
			int remaining = waitTime;
			for(int i = 0; i < waitTime; i++)
			{
				Dispatcher.Invoke(() =>
				{

					FinishButton.Content = $"OK ({remaining})";

				});
				Thread.Sleep(1000);
				remaining--;

			

			}

			Dispatcher.Invoke(() =>
			{
				FinishButton.Content = $"OK";
				FinishButton.IsEnabled = true;
			});

		}

		private void FinishButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
