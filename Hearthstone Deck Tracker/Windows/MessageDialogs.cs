#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HearthDb.Enums;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.FlyoutControls;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.HearthStats.API;
using Hearthstone_Deck_Tracker.Stats;
using Hearthstone_Deck_Tracker.Utility;
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
		private static string LocDeleteGameStatsTitle = "MessageDialogs_DeleteGameStats_Title";
		private static string LocDeleteGameStatsMultiTitle = "MessageDialogs_DeleteGameStats_Multi_Title";
		private static string LocDeleteGameStatsMultiText = "MessageDialogs_DeleteGameStats_Multi_Text";
		private static string LocDeleteGameStatsSure = "MessageDialogs_DeleteGameStats_Label_Sure";
		private static string LocDeleteGameStatsButtonDelete = "MessageDialogs_DeleteGameStats_Button_Delete";
		private static string LocDeleteGameStatsButtonCancel = "MessageDialogs_DeleteGameStats_Button_Cancel";

		private static string LocRestartTitle = "MessageDialogs_Restart_Title";
		private static string LocRestartText = "MessageDialogs_Restart_Text";
		private static string LocRestartButtonRestart = "MessageDialogs_Restart_Button_Restart";
		private static string LocRestartButtonLater = "MessageDialogs_Restart_Button_Later";

		private const string LocSavedFileText = "MessageDialogs_SavedFile_Title";
		private const string LocSavedFileButtonOk = "MessageDialogs_SavedFile_Button_Ok";
		private const string LocSavedFileButtonOpen = "MessageDialogs_SavedFile_Button_OpenFolder";
		
		private const string LocSaveUploadSaved = "MessageDialogs_SaveUpload_Text_Saved";
		private const string LocSaveUploadUploaded = "MessageDialogs_SaveUpload_Text_Uploaded";
		private const string LocSaveUploadButtonOk = "MessageDialogs_SaveUpload_Button_Ok";
		private const string LocSaveUploadButtonBrowser = "MessageDialogs_SaveUpload_Button_Browser";
		private const string LocSaveUploadButtonClipboard = "MessageDialogs_SaveUpload_Button_Clipboard";

		private const string LocScreenshotActionTitle = "MessageDialogs_ScrenshotAction_Title";
		private const string LocScreenshotActionDescription = "MessageDialogs_ScrenshotAction_Description";
		private const string LocScreenshotActionButtonSave = "MessageDialogs_ScrenshotAction_Button_Save";
		private const string LocScreenshotActionButtonSaveUpload = "MessageDialogs_ScrenshotAction_Button_SaveUpload";
		private const string LocScreenshotActionButtonUpload = "MessageDialogs_ScrenshotAction_Button_Upload";
		private const string LocScreenshotActionButtonCancel = "MessageDialogs_ScrenshotAction_Button_Cancel";

		private const string LocLogConfigTitle = "MessageDialogs_LogConfig_Title";
		private const string LocLogConfigDescription1 = "MessageDialogs_LogConfig_Description1";
		private const string LocLogConfigDescription2 = "MessageDialogs_LogConfig_Description2";
		private const string LocLogConfigDescription3 = "MessageDialogs_LogConfig_Description3";
		private const string LocLogConfigButtonInstructions = "MessageDialogs_LogConfig_Button_Instructions";
		private const string LocLogConfigButtonClose = "MessageDialogs_LogConfig_Button_Close";

		//LocUtil.Get()}

		public static async Task<MessageDialogResult> ShowDeleteGameStatsMessage(this MetroWindow window, GameStats stats)
			=> await window.ShowMessageAsync(LocUtil.Get(LocDeleteGameStatsTitle),
				stats + Environment.NewLine + Environment.NewLine + LocUtil.Get(LocDeleteGameStatsSure),
				AffirmativeAndNegative,
				new Settings
				{
					AffirmativeButtonText = LocUtil.Get(LocDeleteGameStatsButtonDelete),
					NegativeButtonText = LocUtil.Get(LocDeleteGameStatsButtonCancel)
				});

		public static async Task<MessageDialogResult> ShowDeleteMultipleGameStatsMessage(this MetroWindow window, int count)
			=> await window.ShowMessageAsync(LocUtil.Get(LocDeleteGameStatsMultiTitle),
				$"{LocUtil.Get(LocDeleteGameStatsMultiText)} ({count})." + Environment.NewLine
				+ Environment.NewLine + LocUtil.Get(LocDeleteGameStatsSure),
				AffirmativeAndNegative,
				new Settings
				{
					AffirmativeButtonText = LocUtil.Get(LocDeleteGameStatsButtonDelete),
					NegativeButtonText = LocUtil.Get(LocDeleteGameStatsButtonCancel)
				});

		public static async void ShowRestartDialog()
		{
			var result = await Core.MainWindow.ShowMessageAsync(LocUtil.Get(LocRestartTitle), LocUtil.Get(LocRestartText),
				AffirmativeAndNegative,
				new Settings()
				{
					AffirmativeButtonText = LocUtil.Get(LocRestartButtonRestart),
					NegativeButtonText = LocUtil.Get(LocRestartButtonLater)
				});
			if(result == MessageDialogResult.Affirmative)
				Core.MainWindow.Restart();
		}

		public static async Task ShowMessage(this MetroWindow window, string title, string message) => await window.ShowMessageAsync(title, message);

		public static async Task ShowSavedFileMessage(this MainWindow window, string fileName)
		{
			var result = await window.ShowMessageAsync("", 
						LocUtil.Get(LocSavedFileText) + Environment.NewLine + Environment.NewLine + fileName,
						AffirmativeAndNegative,
						new Settings
						{
							AffirmativeButtonText = LocUtil.Get(LocSavedFileButtonOk),
							NegativeButtonText = LocUtil.Get(LocSavedFileButtonOpen)
						});
			if(result == MessageDialogResult.Negative)
				Process.Start(Path.GetDirectoryName(fileName));
		}

		public static async Task ShowSavedAndUploadedFileMessage(this MainWindow window, string fileName, string url)
		{
			var sb = new StringBuilder();
			if(fileName != null)
			{
				sb.AppendLine(LocUtil.Get(LocSaveUploadSaved));
				sb.AppendLine(fileName);
				sb.AppendLine();
			}
			sb.AppendLine(LocUtil.Get(LocSaveUploadUploaded));
			sb.AppendLine(url);
			var result = await window.ShowMessageAsync("", sb.ToString(), AffirmativeAndNegativeAndSingleAuxiliary,
				new Settings
				{
					AffirmativeButtonText = LocUtil.Get(LocSaveUploadButtonOk),
					NegativeButtonText = LocUtil.Get(LocSaveUploadButtonBrowser),
					FirstAuxiliaryButtonText = LocUtil.Get(LocSaveUploadButtonClipboard)
				});
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
			var result = await window.ShowMessageAsync(LocUtil.Get(LocScreenshotActionTitle), LocUtil.Get(LocScreenshotActionDescription),
							AffirmativeAndNegativeAndDoubleAuxiliary, new Settings
							{
								AffirmativeButtonText = LocUtil.Get(LocScreenshotActionButtonSave),
								NegativeButtonText = LocUtil.Get(LocScreenshotActionButtonSaveUpload),
								FirstAuxiliaryButtonText = LocUtil.Get(LocScreenshotActionButtonUpload),
								SecondAuxiliaryButtonText = LocUtil.Get(LocScreenshotActionButtonCancel)
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
			var settings = new Settings
			{
				AffirmativeButtonText = LocUtil.Get(LocLogConfigButtonInstructions),
				NegativeButtonText = LocUtil.Get(LocLogConfigButtonClose)
			};
			var result = await window.ShowMessageAsync(LocUtil.Get(LocLogConfigTitle),
										LocUtil.Get(LocLogConfigDescription1) + Environment.NewLine + Environment.NewLine
										+ LocUtil.Get(LocLogConfigDescription2) + Environment.NewLine + Environment.NewLine
										+ LocUtil.Get(LocLogConfigDescription3),
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
			var message = "下列这些卡没有找到:\n";
			var totalDust = 0;
			var sets = new List<string>();
			foreach(var card in deck.MissingCards)
			{
				message += "\n• " + card.LocalizedName;
				if(card.Count == 2)
					message += " x2";

				if(card.Set == HearthDbConverter.SetConverter(CardSet.NAXX))
					sets.Add("and the Naxxramas DLC ");
				else if(card.Set == HearthDbConverter.SetConverter(CardSet.PROMO))
					sets.Add("and Promotion cards ");
				else if(card.Set == HearthDbConverter.SetConverter(CardSet.REWARD))
					sets.Add("and the Reward cards ");
				else if(card.Set == HearthDbConverter.SetConverter(CardSet.BRM))
					sets.Add("and the Blackrock Mountain DLC ");
				else if(card.Set == HearthDbConverter.SetConverter(CardSet.LOE))
					sets.Add("and the League of Explorers DLC ");
				else if(card.Set == HearthDbConverter.SetConverter(CardSet.KARA))
					sets.Add("and the One Night in Karazhan DLC ");
				else
					totalDust += card.DustCost * card.Count;
			}
			message += $"\n\n你需要 {totalDust} 尘 {string.Join("", sets.Distinct())}来合成这些缺少的卡";
			await window.ShowMessageAsync("导出不完整", message, Affirmative, new Settings {AffirmativeButtonText = "OK"});
		}

		public static async Task<bool> ShowAddGameDialog(this MetroWindow window, Deck deck)
		{
			if(deck == null)
				return false;
			var dialog = new AddGameDialog(deck);
			await window.ShowMetroDialogAsync(dialog, new MetroDialogSettings {AffirmativeButtonText = "保存", NegativeButtonText = "取消"});
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

		public static async Task<ImportingChoice?> ShowImportingChoiceDialog(this MetroWindow window)
		{
			var dialog = new ImportingChoiceDialog();
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
			await window.ShowMetroDialogAsync(dialog, new MetroDialogSettings {AffirmativeButtonText = "保存", NegativeButtonText = "取消"});
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
				                        new MetroDialogSettings {AffirmativeButtonText = "是 (一直)", NegativeButtonText = "不 (从不)"});
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
				Core.TrayIcon.ShowMessage("检测到新的竞技场卡组!");

			while(window.Visibility != Visibility.Visible || window.WindowState == WindowState.Minimized)
				await Task.Delay(100);

			var result = await window.ShowMessageAsync("检测到的新卡组！",
                                                 "您可以将此行为改为“自动保存和导入”或“手动”，在【选项】>【跟踪】>【输入】",
												 AffirmativeAndNegative, new Settings { AffirmativeButtonText = "导入", NegativeButtonText = "取消" });

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
