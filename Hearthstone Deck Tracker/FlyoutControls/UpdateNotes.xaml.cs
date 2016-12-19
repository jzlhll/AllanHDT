#region

using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Hearthstone_Deck_Tracker.Annotations;
using Hearthstone_Deck_Tracker.Controls.Information;
using Hearthstone_Deck_Tracker.HearthStats.API;
using Hearthstone_Deck_Tracker.Utility;
using Hearthstone_Deck_Tracker.Utility.Extensions;
using Hearthstone_Deck_Tracker.Utility.Logging;
using Hearthstone_Deck_Tracker.Windows;

#endregion

namespace Hearthstone_Deck_Tracker.FlyoutControls
{
	/// <summary>
	/// Interaction logic for UpdateNotes.xaml
	/// </summary>
	public partial class UpdateNotes : INotifyPropertyChanged
	{
		private bool _continueToHighlight;

		public UpdateNotes()
		{
			InitializeComponent();
		}

		public void SetHighlight(Version previousVersion)
		{
			if(previousVersion == null)
				return;
			UserControl infoControl = null;
			if(previousVersion < new Version(0, 13, 18))
				infoControl = new CardThemesInfo();
#if(!SQUIRREL)
			if(previousVersion < new Version(0, 15, 14) && Config.Instance.SaveConfigInAppData != false && Config.Instance.SaveDataInAppData != false)
			{
				ContentControlHighlight.Content = new SquirrelInfo();
				ButtonContinue.Visibility = Visibility.Collapsed;
				_continueToHighlight = true;
				return;
			}
#endif
			if(infoControl == null)
				return;
			ContentControlHighlight.Content = infoControl;
			TabControl.SelectedIndex = 1;
		}

		private string _releaseNotes;
		public string ReleaseNotes => _releaseNotes ?? (_releaseNotes = GetReleaseNotes());

		private string GetReleaseNotes()
		{
			try
			{
				string releaseNotes;
				using(var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Hearthstone_Deck_Tracker.Resources.CHANGELOG.md"))
				using(var reader = new StreamReader(stream))
					releaseNotes = reader.ReadToEnd();
				releaseNotes = Regex.Replace(releaseNotes, "\n", "\n\n");
				releaseNotes = Regex.Replace(releaseNotes, "#(\\d+)", "[#$1](https://github.com/HearthSim/Hearthstone-Deck-Tracker/issues/$1)");
				return releaseNotes;
			}
			catch(Exception ex)
			{
				Log.Error(ex);
				return null;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void ButtonShowGithub_OnClick(object sender, RoutedEventArgs e)
		{
			const string url = "https://github.com/HearthSim/Hearthstone-Deck-Tracker/releases";
			if (!Helper.TryOpenUrl(url))
				Core.MainWindow.ShowMessage("打不开浏览器", $"你可以找到releases在这里 \"{url}\"").Forget();
		}

		private void ButtonPaypal_Click(object sender, RoutedEventArgs e)
		{
			if (!Helper.TryOpenUrl("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=PZDMUT88NLFYJ"))
				Core.MainWindow.ShowMessage("打不开浏览器", "您还可以找到在GitHub的页面底部的链接！").Forget();
		}

		private void ButtonPatreon_Click(object sender, RoutedEventArgs e)
		{
			const string url = "https://www.patreon.com/HearthstoneDeckTracker";
			if (!Helper.TryOpenUrl(url))
				Core.MainWindow.ShowMessage("打不开浏览器", "你可以在这里找到patreon(赞助)页：" + url).Forget();
		}

		private void ButtonClose_Click(object sender, RoutedEventArgs e)
		{
			if(_continueToHighlight)
			{
				TabControl.SelectedIndex = 1;
				Core.MainWindow.FlyoutUpdateNotes.Header = null;
				Core.MainWindow.FlyoutUpdateNotes.HeaderTemplate = null;
				Core.MainWindow.FlyoutUpdateNotes.BeginAnimation(HeightProperty,
					new DoubleAnimation(Core.MainWindow.FlyoutUpdateNotes.ActualHeight, 400, TimeSpan.FromMilliseconds(250)));
			}
			else
				Core.MainWindow.FlyoutUpdateNotes.IsOpen = false;
		}

		private void ButtonContinue_OnClick(object sender, RoutedEventArgs e) => TabControl.SelectedIndex = 0;
	}
}
