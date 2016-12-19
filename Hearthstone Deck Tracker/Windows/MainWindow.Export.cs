#region

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HearthMirror;
using Hearthstone_Deck_Tracker.Controls;
using Hearthstone_Deck_Tracker.Exporting;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Utility.Extensions;
using Hearthstone_Deck_Tracker.Utility.Logging;
using MahApps.Metro.Controls.Dialogs;
using static MahApps.Metro.Controls.Dialogs.MessageDialogStyle;
using Clipboard = System.Windows.Clipboard;

#endregion

namespace Hearthstone_Deck_Tracker.Windows
{
	public partial class MainWindow
	{
		private void BtnExport_Click(object sender, RoutedEventArgs e)
		{
			var deck = DeckPickerList.SelectedDecks.FirstOrDefault() ?? DeckList.Instance.ActiveDeck;
			if(deck == null)
				return;
			ExportDeck(deck);
		}

		private async void ExportDeck(Deck deck)
		{
			if(Config.Instance.ShowExportingDialog)
			{
				var message = $"1) 创建一个新的或者打开一个存在的 【{AllanAdd.MyUtils.translateClass2CN(deck.Class)} 】卡组.\n\n2) 离开卡组创建界面.\n\n3) 点击【导出】并且不要动鼠标或者点它直到完成。";
				var result = await this.ShowMessageAsync("导出 " + deck.Name + " 到炉石", message, AffirmativeAndNegative, new MessageDialogs.Settings { AffirmativeButtonText = "导出" });
				if(result == MessageDialogResult.Negative)
					return;
			}
			HearthMirror.Objects.Deck openDeck;
			var settings = new MessageDialogs.Settings() {AffirmativeButtonText = "继续", NegativeButtonText = "取消"};
			while((openDeck = Reflection.GetEditedDeck()) == null)
			{
				var result = await this.ShowMessageAsync("没有找到打开的卡组", "继续之前请在炉石中打开一个编辑中的卡组", AffirmativeAndNegative, settings);
				if(result == MessageDialogResult.Negative)
					return;
			}
			string selectedClass;
			while((selectedClass = Database.GetCardFromId(openDeck.Hero).PlayerClass) != deck.Class)
			{
				var result = await this.ShowMessageAsync("错误的英雄", $"打开的英雄卡组与导出的卡组不匹配，请确认是否一致！", AffirmativeAndNegative, settings);
				if(result == MessageDialogResult.Negative)
					return;
				openDeck = Reflection.GetEditedDeck();
			}
			while(!deck.StandardViable && !openDeck.IsWild)
			{
				var result = await this.ShowMessageAsync("不是狂野卡组", "正在导入狂野卡组，但是打开的是标准模式！", AffirmativeAndNegative, settings);
				if(result == MessageDialogResult.Negative)
					return;
				openDeck = Reflection.GetEditedDeck();
			}
			var controller = await this.ShowProgressAsync("卡组创建中", "不要点击或者乱动鼠标！");
			Topmost = false;
			await Task.Delay(500);
			var success = await DeckExporter.Export(deck, async () =>
			{
				if(controller != null)
					await controller.CloseAsync();
				ActivateWindow();
				var result = await this.ShowMessageAsync("导入中断", "继续?", AffirmativeAndNegative,
					new MessageDialogs.Settings() { AffirmativeButtonText = "继续", NegativeButtonText = "取消" });
				if(result == MessageDialogResult.Affirmative)
					controller = await this.ShowProgressAsync("卡组创建中", "不要点击或者乱动鼠标！");
				return result == MessageDialogResult.Affirmative;
			});
			if(controller.IsOpen)
				await controller.CloseAsync();
			if(success)
			{
				var hsDeck = Reflection.GetEditedDeck();
				if(hsDeck != null)
				{
					var existingHsId = DeckList.Instance.Decks.Where(x => x.DeckId != deck.DeckId).FirstOrDefault(x => x.HsId == hsDeck.Id);
					if(existingHsId != null)
						existingHsId.HsId = 0;
					deck.HsId = hsDeck.Id;
					DeckList.Save();
				}
			}
			if(deck.MissingCards.Any())
				this.ShowMissingCardsMessage(deck);
		}

		private void BtnScreenhot_Click(object sender, RoutedEventArgs e) => CaptureScreenshot(true);

		private void BtnScreenhotWithInfo_Click(object sender, RoutedEventArgs e) => CaptureScreenshot(false);

		private async void CaptureScreenshot(bool deckOnly)
		{
			var selectedDeck = DeckPickerList.SelectedDecks.FirstOrDefault();
			if(selectedDeck == null)
				return;
			Log.Info("Creating screenshot of " + selectedDeck.GetSelectedDeckVersion().GetDeckInfo());

			var deck = selectedDeck.GetSelectedDeckVersion();
			var cards = 35 * deck.Cards.Count;
			var height = (deckOnly ? 0 : 124) + cards;
			var width = 219;

			DeckView control = new DeckView(deck, deckOnly);
			control.Measure(new Size(width, height));
			control.Arrange(new Rect(new Size(width, height)));
			control.UpdateLayout();
			Log.Debug($"Screenshot: {control.ActualWidth} x {control.ActualHeight}");

			RenderTargetBitmap bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
			bmp.Render(control);
			var encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(bmp));

			await SaveOrUploadScreenshot(encoder, deck.Name);
		}

		public async Task SaveOrUploadScreenshot(PngBitmapEncoder pngEncoder, string proposedFileName)
		{
			if(pngEncoder != null)
			{
				var saveOperation = await this.ShowScreenshotUploadSelectionDialog();
				if(saveOperation.Cancelled)
					return;
				var tmpFile = new FileInfo(Path.Combine(Config.Instance.DataDir, $"tmp{DateTime.Now.ToFileTime()}.png"));
				var fileName = saveOperation.SaveLocal
					               ? Helper.ShowSaveFileDialog(Helper.RemoveInvalidFileNameChars(proposedFileName), "png") : tmpFile.FullName;
				if(fileName != null)
				{
					string imgurUrl = null;
					using(var ms = new MemoryStream())
					using(var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
					{
						pngEncoder.Save(ms);
						ms.WriteTo(fs);
						if(saveOperation.Upload)
						{
							var controller = await this.ShowProgressAsync("上传中...", "");
							imgurUrl = await Imgur.Upload(Config.Instance.ImgurClientId, ms, proposedFileName);
							await controller.CloseAsync();
						}
					}

					if(imgurUrl != null)
					{
						await this.ShowSavedAndUploadedFileMessage(saveOperation.SaveLocal ? fileName : null, imgurUrl);
						Log.Info("Uploaded screenshot to " + imgurUrl);
					}
					else
						await this.ShowSavedFileMessage(fileName);
					Log.Info("Saved screenshot to: " + fileName);
				}
				if(tmpFile.Exists)
				{
					try
					{
						tmpFile.Delete();
					}
					catch(Exception ex)
					{
						Log.Error(ex);
					}
				}
			}
		}

		private async void BtnSaveToFile_OnClick(object sender, RoutedEventArgs e)
		{
			var selectedDecks = DeckPickerList.SelectedDecks;
			if (selectedDecks.Count > 1)
			{
				if(selectedDecks.Count > 10)
				{
					var result = await
						this.ShowMessageAsync("导出多个卡组！", $"你将要导出 {selectedDecks.Count} 个卡组.确定吗?",
											  AffirmativeAndNegative);
					if(result != MessageDialogResult.Affirmative)
						return;
				}
				var dialog = new FolderBrowserDialog();
				if(dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
					return;
				foreach(var deck in DeckPickerList.SelectedDecks)
				{
					//Helper.GetValidFilePath avoids overwriting files and properly handles duplicate deck names
					var saveLocation = Path.Combine(dialog.SelectedPath, Helper.GetValidFilePath(dialog.SelectedPath, deck.Name, "xml"));
					XmlManager<Deck>.Save(saveLocation, deck.GetSelectedDeckVersion());
					Log.Info($"Saved {deck.GetSelectedDeckVersion().GetDeckInfo()} to file: {saveLocation}");
				}
				await this.ShowSavedFileMessage(dialog.SelectedPath);

			}
			else if(selectedDecks.Count > 0)
			{
				var deck = selectedDecks.First();
				var fileName = Helper.ShowSaveFileDialog(Helper.RemoveInvalidFileNameChars(deck.Name), "xml");
				if(fileName == null)
					return;
				XmlManager<Deck>.Save(fileName, deck.GetSelectedDeckVersion());
				await this.ShowSavedFileMessage(fileName);
				Log.Info($"Saved {deck.GetSelectedDeckVersion().GetDeckInfo()} to file: {fileName}");
			}
		}

		private void BtnClipboard_OnClick(object sender, RoutedEventArgs e)
		{
			var deck = DeckPickerList.SelectedDecks.FirstOrDefault();
			if(deck == null)
				return;
			//Clipboard.SetText(Helper.DeckToIdString(deck.GetSelectedDeckVersion()));
            Clipboard.SetDataObject(Helper.DeckToIdString(deck.GetSelectedDeckVersion()));
            this.ShowMessage("", "拷贝id字串到剪贴板").Forget();
			Log.Info("Copied " + deck.GetSelectedDeckVersion().GetDeckInfo() + " to clipboard");
		}

		private async void BtnClipboardNames_OnClick(object sender, RoutedEventArgs e)
		{
			var deck = DeckPickerList.SelectedDecks.FirstOrDefault();
			if(deck == null || !deck.GetSelectedDeckVersion().Cards.Any())
				return;

			var english = true;
			if(Config.Instance.SelectedLanguage != "enUS")
			{
				try
				{
					english = await this.ShowLanguageSelectionDialog();
				}
				catch(Exception ex)
				{
					Log.Error(ex);
				}
			}
			try
			{
				var names =
					deck.GetSelectedDeckVersion()
					    .Cards.ToSortedCardList()
					    .Select(c => (english ? c.Name : c.LocalizedName) + (c.Count > 1 ? " x " + c.Count : ""))
					    .Aggregate((c, n) => c + Environment.NewLine + n);
                //Clipboard.SetText(names);
                Clipboard.SetDataObject(names);
                this.ShowMessage("", "拷贝名字到剪贴板").Forget();
				Log.Info("Copied " + deck.GetDeckInfo() + " names to clipboard");
			}
			catch(Exception ex)
			{
				Log.Error(ex);
				this.ShowMessage("", "拷贝卡牌名字们中，出错了！").Forget();
			}
		}

		private async void BtnExportFromWeb_Click(object sender, RoutedEventArgs e)
		{
			var result = await ImportDeckFromUrl();
			if(result.WasCancelled)
				return;
			if(result.Deck != null)
				ExportDeck(result.Deck);
			else
				await this.ShowMessageAsync("没有找到卡组", "不能从 " + Environment.NewLine + result.Url+" 找到卡组");
		}

		internal void MenuItemMissingDust_OnClick(object sender, RoutedEventArgs e)
		{
			var deck = DeckPickerList.SelectedDecks.FirstOrDefault();
			if(deck == null)
				return;
			this.ShowMissingCardsMessage(deck);
		}

		public void BtnOpenHearthStats_Click(object sender, RoutedEventArgs e)
		{
			var deck = DeckPickerList.SelectedDecks.FirstOrDefault();
			if(deck == null || !deck.HasHearthStatsId)
				return;
			Helper.TryOpenUrl(deck.HearthStatsUrl);
		}
	}
}
