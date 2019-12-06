using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
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
	/// Interaction logic for DonationForm.xaml
	/// </summary>
	public partial class DonationForm : Window
	{




		private bool Live = false;

		public DonationForm()
		{
			InitializeComponent();


#if DEBUG
			Title = Title + " (DEV MODE)";

#else
			Live = true;
			
#endif
		}


		string ValidateCreditCard()
		{
			var value = Regex.Replace(CreditCard.Text, @"\s", "", RegexOptions.Multiline);

			if (Regex.Match(value, @"[^\d]").Success)
				return null;


			if (value.Length == 16)
				return value;

			return null;

		}

		string ValidateCSV()
		{
			var value = Regex.Replace(CVC.Text, @"\s", "", RegexOptions.Multiline);


			if (Regex.Match(value, @"[^\d]").Success)
				return null;


			if (value.Length == 3)
				return value;

			return null;
		}


		float? ValidateAmount()
		{
			var value = Regex.Replace(Amount.Text, @"\s", "", RegexOptions.Multiline);


			float amount = 0.0f;



			if (float.TryParse(Amount.Text, out amount))
			{
				if (amount > 1.0f)
					return amount;

			}

			return null;

		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{





			var cc = ValidateCreditCard();
			var cvc = ValidateCSV();
			var amount = ValidateAmount();

			var name = FullName.Text;

			var expiry_month = Expiry_M.Text?.TrimStart('0');
			var expiry_year = Expiry_Y.Text?.TrimStart('0');


			bool invalid = false;

			if (cc == null)
			{
				invalid = true;
				CreditCard.BorderBrush = Brushes.Red;
			}
			else
				CreditCard.BorderBrush = Brushes.Black;


			if (cvc == null)
			{
				invalid = true;
				CVC.BorderBrush = Brushes.Red;
			}
			else
				CVC.BorderBrush = Brushes.Black;

			if (amount == null)
			{
				invalid = true;
				Amount.BorderBrush = Brushes.Red;
			}
			else
				Amount.BorderBrush = Brushes.Black;


			if (string.IsNullOrWhiteSpace(name))
			{
				invalid = true;
				FullName.BorderBrush = Brushes.Red;

			}
			else
				FullName.BorderBrush = Brushes.Black;

			int month, year;
			bool monthFailed = !int.TryParse(expiry_month, out month);
			bool yearFailed = !int.TryParse(expiry_year, out year);

			if (monthFailed || (month < 1 || month > 12))
			{
				invalid = true;
				Expiry_M.BorderBrush = Brushes.Red;
			}
			else
			{
				Expiry_M.BorderBrush = Brushes.Black;
			}


			if (yearFailed)
			{
				invalid = true;
				Expiry_Y.BorderBrush = Brushes.Red;
			}
			else
			{
				Expiry_Y.BorderBrush = Brushes.Black;
			}

			var now = DateTime.Now;

			if(!invalid)
			{
				year += 2000;
				if (year < now.Year)
				{
					invalid = true;
					Expiry_Y.BorderBrush = Brushes.Red;
				}
				else if(year == now.Year && month < now.Month)
				{
					invalid = true;
					Expiry_M.BorderBrush = Brushes.Red;
				}
				else
				{
					Expiry_M.BorderBrush = Brushes.Black;
					Expiry_Y.BorderBrush = Brushes.Black;
				}
			}


			var dollars = amount.Value.ToString("c2");

			var result = MessageBox.Show($"Are you sure you wish to donate {dollars}?");


			if (invalid || result != MessageBoxResult.OK)
				return;


			Checkout_Button.IsEnabled = false;

			Task.Run(() => Checkout(cc, cvc, amount.Value, name, month, year));



		}


		private const string testKey = "rk_test_46dLu9MfCMmy0G1a45kMU9zn00IroaDdFD";
		private const string liveKey = "rk_live_USVhmsHnhFpkM4afSg42ifun00m28TjKwZ";

		private async Task Checkout(string cc, string cvc, float amount, string name, int exp_month, int exp_year)
		{
			try
			{

				// Create the HttpContent for the form to be posted.
				var requestContent = new FormUrlEncodedContent(new[] {
					new KeyValuePair<string, string>("card[name]", name),
					new KeyValuePair<string, string>("card[number]", cc),
					new KeyValuePair<string, string>("card[exp_month]", exp_month.ToString("00")),
					new KeyValuePair<string, string>("card[exp_year]", exp_year.ToString()),
					new KeyValuePair<string, string>("card[cvc]", cvc)
				});

				var cardResponse = await Curl(requestContent, "https://api.stripe.com/v1/tokens");

				var a = cardResponse.GetValue("id")?.ToString();

				var b = cardResponse.GetValue("object")?.ToString();

				if (string.IsNullOrWhiteSpace(a) || b != "token")
					throw new Exception("Invalid Credit Card Token");

				var charge = new FormUrlEncodedContent(new[] {

					new KeyValuePair<string, string>("amount", ((int)(amount*100.0f)).ToString()),
					new KeyValuePair<string, string>("currency", "aud"),
					new KeyValuePair<string, string>("source", a),
					new KeyValuePair<string, string>("description", "Thank you for your kind Donation!"),
				});

				var chargeResponse = await Curl(charge, "https://api.stripe.com/v1/charges");

				if (chargeResponse == null)
					throw new Exception("Invalid response from server, please try again later");

				var status = chargeResponse.GetValue("status")?.ToString();

				switch(status)
				{
					case "succeeded":
						PaymentSuccessful(chargeResponse);
						break;

					default:
						throw new Exception("Payment has failed, " + chargeResponse.GetValue("failure_message")?.ToString());
				}
				
			}
			catch (Exception e)
			{
				MessageBox.Show("Payment Error: " + e.Message, "Error");

			}

			Dispatcher.Invoke(() =>
			{
				Checkout_Button.IsEnabled = true;

			});


		}

		private void PaymentSuccessful(JObject response)
		{

			var receipt = response.GetValue("receipt_url")?.ToString();


			if (!string.IsNullOrWhiteSpace(receipt))
			{
				var value = Properties.Settings.Default["receipt_url"] = receipt;

				Properties.Settings.Default.Save();


				if (MessageBox.Show($"Thank you for your support! Click OK to open your receipt, otherwise this can later be opened from the Donation menu", "Payment Success") == MessageBoxResult.OK)
				{

					Process.Start(receipt);

				}
			}
			
				
		}

		private async Task<JObject> Curl(FormUrlEncodedContent content, string endpoint)
		{
			var client = new HttpClient();

			var authToken = Encoding.ASCII.GetBytes(Live ? liveKey : testKey);

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
					 Convert.ToBase64String(authToken));

			// Get the response.
			HttpResponseMessage response = await client.PostAsync(
				endpoint,
				content);

			// Get the response content.
			HttpContent responseContent = response.Content;

			// Get the stream of the content.
			using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
			{
				// Write the output.

				var json = await reader.ReadToEndAsync();

				var result =  JObject.Parse(json);

				var error = result.GetValue("error")?["message"]?.ToString();

				if (error != null)
				{
					throw new Exception(error);
				}

				return result;
			}


		}



	}

}
