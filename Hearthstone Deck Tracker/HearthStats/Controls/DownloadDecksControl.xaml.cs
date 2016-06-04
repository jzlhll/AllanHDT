﻿#region

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.HearthStats.API;
using Hearthstone_Deck_Tracker.Windows;
using MahApps.Metro.Controls.Dialogs;

#endregion

namespace Hearthstone_Deck_Tracker.HearthStats.Controls
{
	/// <summary>
	/// Interaction logic for DownloadDecksControl.xaml
	/// </summary>
	public partial class DownloadDecksControl : UserControl
	{
		private bool _done;
		private List<Deck> _selectedDecks;

		public DownloadDecksControl()
		{
			InitializeComponent();
		}

		public async Task<List<Deck>> LoadDecks(IEnumerable<Deck> decks)
		{
			_selectedDecks = decks.ToList();

			ListViewHearthStats.Items.Clear();
			foreach(var deck in _selectedDecks)
				ListViewHearthStats.Items.Add(deck);

			_done = false;
			while(!_done)
				await Task.Delay(100);

			return _selectedDecks;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			_selectedDecks = ListViewHearthStats.Items.Cast<Deck>().ToList();
			_done = true;
			Core.MainWindow.FlyoutHearthStatsDownload.IsOpen = false;
		}

		private async void BtnDeleteRemoteDeck_OnClick(object sender, RoutedEventArgs e)
		{
			var btn = sender as Button;
			var deck = btn?.DataContext as Deck;
			if(deck == null)
				return;

			//show warning
			var result =
				await
				Core.MainWindow.ShowMessageAsync("删除 " + deck.Name,
                                                 "这将永远的删除卡组和所有相关数据。你肯定吗？",
				                                 MessageDialogStyle.AffirmativeAndNegative,
				                                 new MessageDialogs.Settings {AffirmativeButtonText = "删除", NegativeButtonText = "取消"});
			if(result == MessageDialogResult.Affirmative)
			{
				var deleted = await HearthStatsManager.DeleteDeckAsync(deck, false, true);
				if(deleted)
					ListViewHearthStats.Items.Remove(deck);
			}
		}

		private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
		{
			_selectedDecks = new List<Deck>();
			_done = true;
			Core.MainWindow.FlyoutHearthStatsDownload.IsOpen = false;
		}
	}
}