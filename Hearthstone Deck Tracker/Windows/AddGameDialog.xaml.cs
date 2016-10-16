#region

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Stats;
using Hearthstone_Deck_Tracker.Utility.Logging;
using MahApps.Metro.Controls.Dialogs;
using static System.Windows.Visibility;
using static Hearthstone_Deck_Tracker.Enums.GameMode;
using static Hearthstone_Deck_Tracker.Enums.YesNo;

#endregion

namespace Hearthstone_Deck_Tracker.FlyoutControls
{
	/// <summary>
	/// Interaction logic for AddGameDialog.xaml
	/// </summary>
	public partial class AddGameDialog : CustomDialog
	{
		private readonly Deck _deck;
		private readonly bool _editing;
		private readonly GameStats _game;
		private readonly TaskCompletionSource<GameStats> _tcs;

		public AddGameDialog(Deck deck)
		{
			InitializeComponent();
            ComboBoxResult.ItemsSource = new[]{"胜利","败北","弃局"};
            ComboBoxOpponent.ItemsSource = new[] { "德鲁伊", "猎人", "法师", "圣骑士", "牧师", "潜行者", "萨满", "术士", "战士" };
            ComboBoxMode.ItemsSource = new[] { "天梯", "休闲", "竞技场", "乱斗", "友谊", "练习" };
            ComboBoxFormat.ItemsSource = new[] { "标准", "狂野" };
            ComboBoxRegion.ItemsSource = new[] { "美国", "欧洲", "亚洲", "中国" };
            _tcs = new TaskCompletionSource<GameStats>();
			_editing = false;
			var lastGame = deck.DeckStats.Games.LastOrDefault();
			if(deck.IsArenaDeck)
			{
				ComboBoxMode.SelectedItem = GameModeConverter.convert(Arena);
				ComboBoxMode.IsEnabled = false;
			}
			else
			{
				ComboBoxMode.IsEnabled = true;
				TextBoxRank.IsEnabled = true;
				TextBoxLegendRank.IsEnabled = true;
				if(lastGame != null)
				{
                    ComboBoxFormat.SelectedItem = FormatConvert.convert_(lastGame.Format);
					ComboBoxMode.SelectedItem = GameModeConverter.convert(lastGame.GameMode);
					if(lastGame.GameMode == Ranked)
					{
						TextBoxRank.Text = lastGame.Rank.ToString();
						TextBoxLegendRank.Text = lastGame.LegendRank.ToString();
					}
				}
			}
			if(lastGame != null)
			{
				PanelRank.Visibility = PanelLegendRank.Visibility = lastGame.GameMode == Ranked ? Visible : Collapsed;
				PanelFormat.Visibility = lastGame.GameMode == Ranked || lastGame.GameMode == Casual ? Visible : Collapsed;
				TextBoxPlayerName.Text = lastGame.PlayerName;
				if(lastGame.Region != Region.UNKNOWN)
					ComboBoxRegion.SelectedItem = RegionConvert.convert(lastGame.Region);
			}
			_deck = deck;
			_game = new GameStats();
			BtnSave.Content = "添加游戏";
			Title = _deck.Name;
		}

		public AddGameDialog(GameStats game)
		{
			InitializeComponent();
			_tcs = new TaskCompletionSource<GameStats>();
			_editing = true;
			_game = game;
			if(game == null)
				return;
			ComboBoxResult.SelectedItem = GameResultConvert.convert(game.Result);
			HeroClass heroClass;
			if(!string.IsNullOrWhiteSpace(game.OpponentHero) && Enum.TryParse(game.OpponentHero, out heroClass))
				ComboBoxOpponent.SelectedItem = HeroClassConverter.convert(heroClass);
			ComboBoxMode.SelectedItem = GameModeConverter.convert(game.GameMode);
			ComboBoxFormat.SelectedItem = FormatConvert.convert_(game.Format);
			ComboBoxRegion.SelectedItem = RegionConvert.convert(game.Region);
			if(game.GameMode == Ranked)
			{
				TextBoxRank.Text = game.Rank.ToString();
				TextBoxLegendRank.Text = game.LegendRank.ToString();
			}
			PanelRank.Visibility = PanelLegendRank.Visibility = game.GameMode == Ranked ? Visible : Collapsed;
			PanelFormat.Visibility = game.GameMode == Ranked || game.GameMode == Casual ? Visible : Collapsed;
			ComboBoxCoin.SelectedItem = game.Coin ? Yes : No;
			ComboBoxConceded.SelectedItem = game.WasConceded ? Yes : No;
			TextBoxTurns.Text = game.Turns.ToString();
			TextBoxDuration.Text = game.Duration;
			TextBoxDuration.IsEnabled = false;
			TextBoxNote.Text = game.Note;
			TextBoxOppName.Text = game.OpponentName;
			TextBoxPlayerName.Text = game.PlayerName;
			BtnSave.Content = "保存";
			Title = "编辑游戏";
		}

		private void BtnSave_OnClick(object sender, RoutedEventArgs e)
		{
			BtnSave.IsEnabled = false;
			try
			{
				int duration;
				int.TryParse(TextBoxDuration.Text, out duration);
				int rank;
				int.TryParse(TextBoxRank.Text, out rank);
				int legendRank;
				int.TryParse(TextBoxLegendRank.Text, out legendRank);
				int turns;
				int.TryParse(TextBoxTurns.Text, out turns);
				if(!_editing)
				{
					_game.StartTime = DateTime.Now;
					_game.GameId = Guid.NewGuid();
					_game.EndTime = DateTime.Now.AddMinutes(duration);
					_game.PlayerHero = _deck.Class;
					_game.PlayerDeckVersion = _deck.SelectedVersion;
				}
				_game.Result = GameResultConvert.convert((string)ComboBoxResult.SelectedItem);
				_game.GameMode = GameModeConverter.convert((string)ComboBoxMode.SelectedItem);
				_game.OpponentHero = AllanAdd.MyUtils.translateClass2EN(((string)ComboBoxOpponent.SelectedValue));
				_game.Coin = (YesNo)ComboBoxCoin.SelectedValue == Yes;
				_game.Rank = rank;
				_game.LegendRank = legendRank;
				_game.Note = TextBoxNote.Text;
				_game.OpponentName = TextBoxOppName.Text;
				_game.PlayerName = TextBoxPlayerName.Text;
				_game.Turns = turns;
				_game.WasConceded = (YesNo)ComboBoxConceded.SelectedValue == Yes;
				_game.Region = RegionConvert.convert((string)ComboBoxRegion.SelectedItem);
				if(_game.GameMode == Casual || _game.GameMode == Ranked)
					_game.Format = FormatConvert.convert_((string)ComboBoxFormat.SelectedItem);
				_tcs.SetResult(_game);
			}
			catch(Exception ex)
			{
				Log.Error(ex);
				_tcs.SetResult(null);
			}
		}

		internal Task<GameStats> WaitForButtonPressAsync() => _tcs.Task;

		private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			if(!char.IsDigit(e.Text, e.Text.Length - 1))
				e.Handled = true;
		}

		private void ComboBoxMode_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(IsLoaded)
			{
				var ranked = e.AddedItems.Contains(Ranked);
				PanelRank.Visibility = PanelLegendRank.Visibility= ranked ? Visible : Collapsed;

				var format = ranked || e.AddedItems.Contains(Casual);
				PanelFormat.Visibility = format ? Visible : Collapsed;
			}
		}

		private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
		{
			BtnCancel.IsEnabled = false;
			_tcs.SetResult(null);
		}
	}
}