#region

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.FlyoutControls;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.HearthStats.API;
using Hearthstone_Deck_Tracker.Stats;
using Hearthstone_Deck_Tracker.Utility.Extensions;
using Hearthstone_Deck_Tracker.Utility.Logging;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using static System.StringComparison;
using static MahApps.Metro.Controls.Dialogs.MessageDialogStyle;

#endregion

namespace Hearthstone_Deck_Tracker.Windows
{
	public static class MessageDialogs
	{
		public static async Task<MessageDialogResult> ShowDeleteGameStatsMessage(this MetroWindow window, GameStats stats)
			=> await window.ShowMessageAsync("删除游戏", $"{stats.Result} vs {stats.OpponentHero}\n从 {stats.StartTime}\n\n确定吗?",
				AffirmativeAndNegative, new Settings {AffirmativeButtonText = "Yes", NegativeButtonText = "No"});

		public static async Task<MessageDialogResult> ShowDeleteMultipleGameStatsMessage(this MetroWindow window, int count)
			=> await window.ShowMessageAsync("删除游戏", $"删除选中的游戏 ({count}) 个.\n\nAre you sure?",
				AffirmativeAndNegative, new Settings {AffirmativeButtonText = "Yes", NegativeButtonText = "No"});

		public static async Task ShowUpdateNotesMessage(this MetroWindow window)
		{
			var result = await window.ShowMessageAsync("上传成功", "", AffirmativeAndNegative,
							new Settings {AffirmativeButtonText = "Show update notes", NegativeButtonText = "Close"});
			if(result == MessageDialogResult.Affirmative)
				Helper.TryOpenUrl(@"https://github.com/HearthSim/Hearthstone-Deck-Tracker/releases");
		}

		public static async void ShowRestartDialog()
		{
			var result =
				await Core.MainWindow.ShowMessageAsync("需要重启.", "HDT汉化版需要重启才能生效.",
					MessageDialogStyle.AffirmativeAndNegative,
					new MessageDialogs.Settings() { AffirmativeButtonText = "立刻重启", NegativeButtonText = "稍后" });
			if(result == MessageDialogResult.Affirmative)
				Core.MainWindow.Restart();
		}

		public static async Task ShowMessage(this MetroWindow window, string title, string message) => await window.ShowMessageAsync(title, message);

		public static async Task ShowSavedFileMessage(this MainWindow window, string fileName)
		{
			var result = await window.ShowMessageAsync("", $"保存到\n\"{fileName}\"", AffirmativeAndNegative,
							new Settings {NegativeButtonText = "Open folder"});
			if(result == MessageDialogResult.Negative)
				Process.Start(Path.GetDirectoryName(fileName));
		}

		public static async Task ShowSavedAndUploadedFileMessage(this MainWindow window, string fileName, string url)
		{
			var sb = new StringBuilder();
			if(fileName != null)
				sb.AppendLine($"Saved to\n\"{fileName}\"");
			sb.AppendLine($"Uploaded to\n{url}");
			var result = await window.ShowMessageAsync("", sb.ToString(), AffirmativeAndNegativeAndSingleAuxiliary,
							new Settings {NegativeButtonText = "打开浏览器", FirstAuxiliaryButtonText = "拷贝了url到剪贴板"});
			if(result == MessageDialogResult.Negative)
				Helper.TryOpenUrl(url);
			else if(result == MessageDialogResult.FirstAuxiliary)
			{
				try
				{
					Clipboard.SetText(url);
				}
				catch(Exception ex)
				{
					Log.Error("Error copying url to clipboard: " + ex);
				}
			}
		}

		public static async Task<SaveScreenshotOperation> ShowScreenshotUploadSelectionDialog(this MainWindow window)
		{
			var result = await window.ShowMessageAsync("选择操作", "\"上传\" 将会自动传图片到imgur.com",
							AffirmativeAndNegativeAndDoubleAuxiliary, new Settings
							{
								AffirmativeButtonText = "保存",
								NegativeButtonText = "保存并上传",
								FirstAuxiliaryButtonText = "上传",
								SecondAuxiliaryButtonText = "取消"
							});
			return new SaveScreenshotOperation
			{
				Cancelled =  result == MessageDialogResult.SecondAuxiliary,
				SaveLocal = result != MessageDialogResult.FirstAuxiliary,
				Upload = result != MessageDialogResult.Affirmative
			};
		}

		public static async Task ShowLogConfigUpdateFailedMessage(this MetroWindow window)
		{
			var settings = new Settings {AffirmativeButtonText = "show instructions", NegativeButtonText = "close"};
			var result = await window.ShowMessageAsync("有一个问题在更新log.config中",
                                        "新的log.config设定为HDT需要正常使用的话，\n\n需要使用管理员启动HDT汉化版。\n\n如果需要帮助，点击“显示说明”如何手动更新。",
										AffirmativeAndNegative, settings);
			if(result == MessageDialogResult.Affirmative)
				Helper.TryOpenUrl("https://github.com/HearthSim/Hearthstone-Deck-Tracker/wiki/Setting-up-the-log.config");
		}

		public static async void ShowMissingCardsMessage(this MetroWindow window, Deck deck)
		{
			if(!deck.MissingCards.Any())
			{
				await window.ShowMessageAsync("没有丢失的卡",
                        "当你最后一次导出到这个卡组的时候，没有一张牌被遗失。（或者你最近没有导出这个卡组）",
						Affirmative, new Settings {AffirmativeButtonText = "OK"});
				return;
			}
			var message = "The following cards were not found:\n";
			var totalDust = 0;
			var sets = new string[5];
			foreach(var card in deck.MissingCards)
			{
				message += "\n• " + card.LocalizedName;
				if(card.Count == 2)
					message += " x2";

				if(card.Set.Equals("CURSE OF NAXXRAMAS", CurrentCultureIgnoreCase))
					sets[0] = "and the Naxxramas DLC ";
				else if(card.Set.Equals("PROMOTION", CurrentCultureIgnoreCase))
					sets[1] = "and Promotion cards ";
				else if(card.Set.Equals("REWARD", CurrentCultureIgnoreCase))
					sets[2] = "and the Reward cards ";
				else if(card.Set.Equals("BLACKROCK MOUNTAIN", CurrentCultureIgnoreCase))
					sets[3] = "and the Blackrock Mountain DLC ";
				else if(card.Set.Equals("LEAGUE OF EXPLORERS", CurrentCultureIgnoreCase))
					sets[4] = "and the League of Explorers DLC ";
				else
					totalDust += card.DustCost * card.Count;
			}
			message += $"\n\nYou need {totalDust} dust {string.Join("", sets)}to craft the missing cards.";
			await window.ShowMessageAsync("导出不完整", message, Affirmative, new Settings {AffirmativeButtonText = "OK"});
		}

		public static async Task<bool> ShowAddGameDialog(this MetroWindow window, Deck deck)
		{
			if(deck == null)
				return false;
			var dialog = new AddGameDialog(deck);
			await window.ShowMetroDialogAsync(dialog, new MetroDialogSettings {AffirmativeButtonText = "save", NegativeButtonText = "cancel"});
			var game = await dialog.WaitForButtonPressAsync();
			await window.HideMetroDialogAsync(dialog);
			if(game == null)
				return false;
			deck.DeckStats.AddGameResult(game);
			if(Config.Instance.HearthStatsAutoUploadNewGames)
			{
				if(game.GameMode == GameMode.Arena)
					HearthStatsManager.UploadArenaMatchAsync(game, deck, true, true).Forget();
				else
					HearthStatsManager.UploadMatchAsync(game, deck.GetSelectedDeckVersion(), true, true).Forget();
			}
			DeckStatsList.Save();
			Core.MainWindow.DeckPickerList.UpdateDecks(forceUpdate: new[] {deck});
			return true;
		}

		public static async Task<DeckType?> ShowDeckTypeDialog(this MetroWindow window)
		{
			var dialog = new DeckTypeDialog();
			await window.ShowMetroDialogAsync(dialog);
			var type = await dialog.WaitForButtonPressAsync();
			await window.HideMetroDialogAsync(dialog);
			return type;
		}

		public static async Task<bool> ShowEditGameDialog(this MetroWindow window, GameStats game)
		{
			if(game == null)
				return false;
			var dialog = new AddGameDialog(game);
			await window.ShowMetroDialogAsync(dialog, new MetroDialogSettings {AffirmativeButtonText = "save", NegativeButtonText = "cancel"});
			var result = await dialog.WaitForButtonPressAsync();
			await window.HideMetroDialogAsync(dialog);
			if(result == null)
				return false;
			if(Config.Instance.HearthStatsAutoUploadNewGames && HearthStatsAPI.IsLoggedIn)
			{
				var deck = DeckList.Instance.Decks.FirstOrDefault(d => d.DeckId == game.DeckId);
				if(deck != null)
				{
					if(game.GameMode == GameMode.Arena)
						HearthStatsManager.UpdateArenaMatchAsync(game, deck, true, true);
					else
						HearthStatsManager.UpdateMatchAsync(game, deck.GetVersion(game.PlayerDeckVersion), true, true);
				}
			}
			DeckStatsList.Save();
			Core.MainWindow.DeckPickerList.UpdateDecks();
			return true;
		}

		public static async Task<bool> ShowCheckHearthStatsMatchDeletionDialog(this MetroWindow window)
		{
			if(Config.Instance.HearthStatsAutoDeleteMatches.HasValue)
				return Config.Instance.HearthStatsAutoDeleteMatches.Value;
			var dialogResult =
				await
				window.ShowMessageAsync("删除对战在HearthStats上?", "你可以在任何时候更改此设置hearthstats菜单。",
				                        AffirmativeAndNegative,
				                        new MetroDialogSettings {AffirmativeButtonText = "yes (always)", NegativeButtonText = "no (never)"});
			Config.Instance.HearthStatsAutoDeleteMatches = dialogResult == MessageDialogResult.Affirmative;
			Core.MainWindow.MenuItemCheckBoxAutoDeleteGames.IsChecked = Config.Instance.HearthStatsAutoDeleteMatches;
			Config.Save();
			return Config.Instance.HearthStatsAutoDeleteMatches != null && Config.Instance.HearthStatsAutoDeleteMatches.Value;
		}

		public static async Task<bool> ShowLanguageSelectionDialog(this MetroWindow window)
		{
			var english = await
				window.ShowMessageAsync("语言选择", "", AffirmativeAndNegative,
										new Settings
										{
											AffirmativeButtonText = Helper.LanguageDict.First(x => x.Value == "enUS").Key,
											NegativeButtonText = Helper.LanguageDict.First(x => x.Value == Config.Instance.SelectedLanguage).Key
										}) == MessageDialogResult.Affirmative;
			return english;
		}

		private static bool _awaitingMainWindowOpen;
		public static async void ShowNewArenaDeckMessageAsync(this MetroWindow window, HearthMirror.Objects.Deck deck)
		{
			if(_awaitingMainWindowOpen)
				return;
			_awaitingMainWindowOpen = true;

			if(window.WindowState == WindowState.Minimized)
				Core.TrayIcon.ShowMessage("New arena deck detected!");

			while(window.Visibility != Visibility.Visible || window.WindowState == WindowState.Minimized)
				await Task.Delay(100);

			var result = await window.ShowMessageAsync("检测到的新卡组！",
                                                 "您可以将此行为改为“自动保存和导入”或“手动”，在【选项】>【跟踪】>【输入】",
												 AffirmativeAndNegative, new Settings { AffirmativeButtonText = "Import", NegativeButtonText = "Cancel" });

			if(result == MessageDialogResult.Affirmative)
			{
				Log.Info("...saving new arena deck.");
				Core.MainWindow.ImportArenaDeck(deck);
			}
			else
				Log.Info("...discarded by user.");
			Core.Game.IgnoredArenaDecks.Add(deck.Id);
			_awaitingMainWindowOpen = false;
		}

		public class Settings : MetroDialogSettings
		{
			public Settings()
			{
				AnimateHide = AnimateShow = Config.Instance.UseAnimations;
			}
		}
	}

	public class SaveScreenshotOperation
	{
		public bool Cancelled { get; set; }
		public bool SaveLocal { get; set; }
		public bool Upload { get; set; }
	}
}
