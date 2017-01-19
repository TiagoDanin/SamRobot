using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Data.Json;
using Windows.System.Profile;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.ApplicationModel;

namespace SamRobot
{ 
	// App By Tiago Danin
	// Chatbot API by Program-o.com
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();
			if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
			{
				var statusBar = StatusBar.GetForCurrentView();
				var colorBar = Application.Current.Resources["SystemControlBackgroundChromeMediumBrush"] as SolidColorBrush;

				statusBar.BackgroundColor = colorBar.Color;
				statusBar.BackgroundOpacity = 1;
			}
		}

		private async void programO(string Input, bool key)
		{
			string url = "http://api.program-o.com/v2/chatbot/";
			var deviceInformation = new EasClientDeviceInformation();
			string Id = deviceInformation.Id.ToString();
			if (Input == "" || Input.Replace(" ", "") == "") {
				if (key == false) {
					Input = "Hi!";
					txtOutputTwo.Text = Input;
				} else {
					Input = "Hi!";
				}
			} else {
				txtOutputTwo.Text = Input;
			}

			string param = "?bot_id=6&convo_id=sam" + Id.Substring(0, 12) + "&format=json&say=" + Input;

			// Send Request
			Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
			Uri requestUri = new Uri(url + param);
			Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
			string httpResponseBody = "";

			try
			{
				httpResponse = await httpClient.GetAsync(requestUri);
				httpResponse.EnsureSuccessStatusCode();
				httpResponseBody = await httpResponse.Content.ReadAsStringAsync();

				JsonObject root = JsonValue.Parse(httpResponseBody).GetObject();
				string text = Convert.ToString(root["botsay"]);

				txtOutput.Text = ((text.Replace("\"", "")).Replace("Program-O", "Sam")).Replace("\\","");

			}
			catch
			{
				Random r = new Random();
				int nr = r.Next(1, 3);
				if (nr == 1) {
					txtOutput.Text = "Check your internet connection!";
				} else {
					txtOutput.Text = "Error can't connect to my ship!";
				}
			}

			txtInput.Text = "";
		}

		private void SendText(object sender, RoutedEventArgs e)
		{
			string Input = txtInput.Text;
			programO(Input, false);
		}

		private void Delete(object sender, RoutedEventArgs e)
		{
			txtInput.Text = "";
		}

		private void About(object sender, RoutedEventArgs e)
		{
			Package package = Package.Current;
			PackageId packageId = package.Id;
			PackageVersion version = packageId.Version;

			string textAbout = $"I'm Sam a chatbot \nDeveloped by Tiago Danin \nChatbot API by Program-o.com \nGithub TiagoDanin/SamRobot \nVersion {version.Major}.{version.Minor}.{version.Build}";
			txtOutputTwo.Text = "";
			txtOutput.Text = textAbout;
		}

		private void txtInputKey(object sender, KeyRoutedEventArgs e)
		{
			bool check = false;
			if (e.Key == Windows.System.VirtualKey.Enter) {
				check = true;
			}

			if (check) {
				check = false;
				string Input = txtInput.Text;
				programO(Input, true);
			}
		}
	}
}
