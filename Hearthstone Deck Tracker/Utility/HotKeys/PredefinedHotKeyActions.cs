#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Hearthstone_Deck_Tracker.Exporting;
using Hearthstone_Deck_Tracker.Utility.Extensions;
using Hearthstone_Deck_Tracker.Utility.Logging;
using MahApps.Metro.Controls.Dialogs;
using Clipboard = System.Windows.Clipboard;

#endregion

namespace Hearthstone_Deck_Tracker.Utility.HotKeys
{
	public class PredefinedHotKeyActionInfo
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string MethodName { get; set; }
	}

	public class PredefinedHotKeyActions
	{
		public static IEnumerable<PredefinedHotKeyActionInfo> PredefinedActionNames
		{
			get
			{
				return
					typeof(PredefinedHotKeyActions).GetMethods()
					                               .Where(x => x.GetCustomAttributes(typeof(PredefinedHotKeyActionAttribute), false).Any())
					                               .Select(x =>
					                               {
						                               var attr =
							                               ((PredefinedHotKeyActionAttribute)
							                                x.GetCustomAttributes(typeof(PredefinedHotKeyActionAttribute), false)[0]);
						                               return new PredefinedHotKeyActionInfo
						                               {
							                               MethodName = x.Name,
							                               Title = attr.Title,
							                               Description = attr.Description
						                               };
					                               });
			}
		}

		[PredefinedHotKeyAction("切换界面", "切换界面显示和关闭(如果游戏运行中)")]
		public static void ToggleOverlay()
		{
			if(!Core.Game.IsRunning)
				return;
			Config.Instance.HideOverlay = !Config.Instance.HideOverlay;
			Config.Save();
			Core.Overlay.UpdatePosition();
		}

		[PredefinedHotKeyAction("切换界面:卡牌标记",
            "在界面上切换卡牌标记和回合数显示和关闭(如果游戏运行中)")]
		public static void ToggleOverlayCardMarks()
		{
			if(!Core.Game.IsRunning)
				return;
			Config.Instance.HideOpponentCardMarks = !Config.Instance.HideOpponentCardMarks;
			Config.Instance.HideOpponentCardAge = Config.Instance.HideOpponentCardMarks;
			Config.Save();
			Core.Overlay.UpdatePosition();
		}

		[PredefinedHotKeyAction("切换界面:奥秘", "在界面上切换奥秘的显示和关闭(如果游戏运行中)")]
		public static void ToggleOverlaySecrets()
		{
			if(!Core.Game.IsRunning)
				return;
			Config.Instance.HideSecrets = !Config.Instance.HideSecrets;
			Config.Save();
			Core.Overlay.UpdatePosition();
		}

		[PredefinedHotKeyAction("切换界面:计时器", "在界面上切换计时器的显示和关闭(如果游戏运行中)")]
		public static void ToggleOverlayTimer()
		{
			if(!Core.Game.IsRunning)
				return;
			Config.Instance.HideTimers = !Config.Instance.HideTimers;
			Config.Save();
			Core.Overlay.UpdatePosition();
		}

		[PredefinedHotKeyAction("切换界面:攻击图标", "在界面上切换攻击图标的显示和关闭(如果游戏运行中)")
		]
		public static void ToggleOverlayAttack()
		{
			if(!Core.Game.IsRunning)
				return;
			Config.Instance.HidePlayerAttackIcon = !Config.Instance.HidePlayerAttackIcon;
			Config.Instance.HideOpponentAttackIcon = Config.Instance.HidePlayerAttackIcon;
			Config.Save();
			Core.Overlay.UpdatePosition();
		}

		[PredefinedHotKeyAction("切换【无卡组模式】", "激活【无卡组模式】或激活最后使用的卡组。")]
		public static void ToggleNoDeckMode()
		{
			if(DeckList.Instance.ActiveDeck == null)
				Core.MainWindow.SelectLastUsedDeck();
			else
				Core.MainWindow.SelectDeck(null, true);
		}

		[PredefinedHotKeyAction("导出卡组",
            "激活【无卡组模式】或激活最后使用的卡组。 这将不会显示任何对话框在主窗口中。")]
		public static void ExportDeck()
		{
			if(DeckList.Instance.ActiveDeck != null && Core.Game.IsInMenu)
				DeckExporter.Export(DeckList.Instance.ActiveDeckVersion).Forget();
		}

		[PredefinedHotKeyAction("编辑激活的卡组", "打开编辑激活的卡组对话框（如果有的话）和让该软件切到前台。")]
		public static void EditDeck()
		{
			if(DeckList.Instance.ActiveDeck == null)
				return;
			Core.MainWindow.SetNewDeck(DeckList.Instance.ActiveDeck, true);
			Core.MainWindow.ActivateWindow();
		}

		[PredefinedHotKeyAction("从游戏中导入:竞技场", "")]
		public static void ImportFromArena()
		{
			Core.MainWindow.StartArenaImporting().Forget();
			Core.MainWindow.ActivateWindow();
		}

		[PredefinedHotKeyAction("从游戏中导入:构筑", "")]
		public static void ImportFromConstructed()
		{
			Core.MainWindow.ShowImportDialog(false);
			Core.MainWindow.ActivateWindow();
		}

		[PredefinedHotKeyAction("从网络导入", "开始将所有对话从网络导入")]
		public static void ImportFromWeb()
		{
			Core.MainWindow.ImportDeck();
			Core.MainWindow.ActivateWindow();
		}

		[PredefinedHotKeyAction("从网络导入：剪贴板", "开始网络导入进程没有任何窗口。")]
		public static void ImportFromWebClipboard()
		{
			var clipboard = Clipboard.ContainsText() ? Clipboard.GetText() : "could not get text from clipboard";
			Core.MainWindow.ImportDeck(clipboard);
			Core.MainWindow.ActivateWindow();
		}

		[PredefinedHotKeyAction("从网络导入：高亮",
            "这发出了一个“ctrl+c”命令之前启动导入：只强调URL并按下热键。"
            )]
		public static async void ImportFromWebHighlight()
		{
			SendKeys.SendWait("^c");
			await Task.Delay(200);
			var clipboard = Clipboard.ContainsText() ? Clipboard.GetText() : "could not get text from clipboard";
			Core.MainWindow.ImportDeck(clipboard);
			Core.MainWindow.ActivateWindow();
		}

		[PredefinedHotKeyAction("截图",
            "创建一个游戏的截图和界面（和其他的一切在它的前面）。有一个选项来自动上传到Imgur。"
            )]
		public static async void Screenshot()
		{
			var handle = User32.GetHearthstoneWindow();
			if(handle == IntPtr.Zero)
				return;
			var rect = User32.GetHearthstoneRect(false);
			var bmp = await ScreenCapture.CaptureHearthstoneAsync(new Point(0, 0), rect.Width, rect.Height, handle, false, false);
			if(bmp == null)
			{
				Log.Error("There was an error capturing hearthstone.");
				return;
			}
			using(var mem = new MemoryStream())
			{
				var encoder = new PngBitmapEncoder();
				bmp.Save(mem, ImageFormat.Png);
				encoder.Frames.Add(BitmapFrame.Create(mem));
				await Core.MainWindow.SaveOrUploadScreenshot(encoder, "Hearthstone " + DateTime.Now.ToString("MM-dd-yy hh-mm-ss"));
			}
			Core.MainWindow.ActivateWindow();
		}

		[PredefinedHotKeyAction("游戏截图",
            "创建一个游戏截图。有一个选项来自动上传到Imgur。"
            )]
		public static async void GameScreenshot()
		{
			var handle = User32.GetHearthstoneWindow();
			if(handle == IntPtr.Zero)
				return;
			var rect = User32.GetHearthstoneRect(false);
			var bmp = await ScreenCapture.CaptureHearthstoneAsync(new Point(0, 0), rect.Width, rect.Height, handle, false, true);
			if(bmp == null)
			{
				Log.Error("There was an error capturing hearthstone.");
				return;
			}
			using(var mem = new MemoryStream())
			{
				var encoder = new PngBitmapEncoder();
				bmp.Save(mem, ImageFormat.Png);
				encoder.Frames.Add(BitmapFrame.Create(mem));
				await Core.MainWindow.SaveOrUploadScreenshot(encoder, "Hearthstone " + DateTime.Now.ToString("MM-dd-yy hh-mm-ss"));
			}
			Core.MainWindow.ActivateWindow();
		}

		[PredefinedHotKeyAction("备注对话框", "为当前（运行）游戏带来备注对话框。")]
		public static void NoteDialog()
		{
			if(Core.Game.IsRunning && !Core.Game.IsInMenu)
				new NoteDialog(Core.Game.CurrentGameStats).Show();
		}

		[PredefinedHotKeyAction("开始炉石游戏", "打开Battle.net launcher 并且打开炉石.")]
		public static void StartHearthstone()
		{
			if(Core.MainWindow.BtnStartHearthstone.IsEnabled)
				Helper.StartHearthstoneAsync().Forget();
		}

		[PredefinedHotKeyAction("显示主窗口", "打开主窗口")]
		public static void ShowMainWindow()
		{
			Core.MainWindow.ActivateWindow();
		}

		[PredefinedHotKeyAction("显示统计", "打开统计窗口或者弹出。")]
		public static void ShowStats()
		{
			Core.MainWindow.ShowStats(false, false);
		}

		[PredefinedHotKeyAction("重载卡组", "重置该游戏到上次开始")]
		public static void ReloadDeck()
		{
			if(DeckList.Instance.ActiveDeck == null)
				Core.MainWindow.SelectDeck(null, true);
			else
				Core.MainWindow.SelectLastUsedDeck();
		}

		[PredefinedHotKeyAction("关闭程序", "关闭程序")]
		public static void CloseHdt()
		{
			Core.MainWindow.Close();
		}
	}
}