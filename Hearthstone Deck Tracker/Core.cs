#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Hearthstone_Deck_Tracker.Controls.Stats;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.HearthStats.API;
using Hearthstone_Deck_Tracker.HsReplay;
using Hearthstone_Deck_Tracker.LogReader;
using Hearthstone_Deck_Tracker.Plugins;
using Hearthstone_Deck_Tracker.Utility;
using Hearthstone_Deck_Tracker.Utility.Analytics;
using Hearthstone_Deck_Tracker.Utility.Extensions;
using Hearthstone_Deck_Tracker.Utility.HotKeys;
using Hearthstone_Deck_Tracker.Utility.LogConfig;
using Hearthstone_Deck_Tracker.Utility.Logging;
using Hearthstone_Deck_Tracker.Windows;
using MahApps.Metro.Controls.Dialogs;
using Hearthstone_Deck_Tracker.Utility.Themes;
using Hearthstone_Deck_Tracker.Utility.Updating;
using WPFLocalizeExtension.Engine;

#endregion

namespace Hearthstone_Deck_Tracker
{
	public static class Core
	{
		internal const int UpdateDelay = 100;
		private static TrayIcon _trayIcon;
		private static OverlayWindow _overlay;
		private static Overview _statsOverview;
		private static int _updateRequestsPlayer;
		private static int _updateRequestsOpponent;
		private static DateTime _startUpTime;
		public static Version Version { get; set; }
		public static GameV2 Game { get; set; }
		public static MainWindow MainWindow { get; set; }

		public static Overview StatsOverview => _statsOverview ?? (_statsOverview = new Overview());

		public static bool Initialized { get; private set; }

		public static TrayIcon TrayIcon => _trayIcon ?? (_trayIcon = new TrayIcon());

		public static OverlayWindow Overlay => _overlay ?? (_overlay = new OverlayWindow(Game));

		internal static bool UpdateOverlay { get; set; } = true;
		internal static bool Update { get; set; }
		internal static bool CanShutdown { get; set; }
		//allan add
        private static void CopyFolder(string from, string to)
        {
            to = to + "\\";
            if (!Directory.Exists(to))
                Directory.CreateDirectory(to);

            // 子文件夹
            foreach (string sub in Directory.GetDirectories(from))
                CopyFolder(sub + "\\", to + Path.GetFileName(sub) + "\\");

            // 文件
            foreach (string file in Directory.GetFiles(from))
                File.Copy(file, to + Path.GetFileName(file), true);
        }

        public static async void Initialize()
		{
			LocalizeDictionary.Instance.Culture = CultureInfo.GetCultureInfo("en-US");
			_startUpTime = DateTime.UtcNow;
			Log.Info($"HDT: {Helper.GetCurrentVersion()}, Operating System: {Helper.GetWindowsVersion()}, .NET Framework: {Helper.GetInstalledDotNetVersion()}");
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
			Config.Load();
			var splashScreenWindow = new SplashScreenWindow();
#if(SQUIRREL)
			if(Config.Instance.CheckForUpdates)
			{
				var updateCheck = Updater.StartupUpdateCheck(splashScreenWindow);
				while(!updateCheck.IsCompleted)
				{
					await Task.Delay(500);
					if(splashScreenWindow.SkipUpdate)
						break;
				}
			}
#endif
			splashScreenWindow.ShowConditional();
			Log.Initialize();
			ConfigManager.Run();
			LocUtil.UpdateCultureInfo();
			var newUser = ConfigManager.PreviousVersion == null;
			LogConfigUpdater.Run().Forget();
			LogConfigWatcher.Start();
			Helper.UpdateAppTheme();
			ThemeManager.Run();
			ResourceMonitor.Run();
			Game = new GameV2();
			LoginType loginType;
			var loggedIn = HearthStatsAPI.LoadCredentials();
			if(!loggedIn && Config.Instance.ShowLoginDialog)
			{
				var loginWindow = new LoginWindow();
				splashScreenWindow.Close();
				loginWindow.ShowDialog();
				if(loginWindow.LoginResult == LoginType.None)
				{
					Application.Current.Shutdown();
					return;
				}
				loginType = loginWindow.LoginResult;
				splashScreenWindow = new SplashScreenWindow();
				splashScreenWindow.ShowConditional();
			}
			else
				loginType = loggedIn ? LoginType.AutoLogin : LoginType.AutoGuest;
			MainWindow = new MainWindow();
			MainWindow.LoadConfigSettings();
			MainWindow.Show();
			splashScreenWindow.Close();

			if(Config.Instance.DisplayHsReplayNoteLive && ConfigManager.PreviousVersion != null && ConfigManager.PreviousVersion < new Version(1, 1, 0))
				MainWindow.FlyoutHsReplayNote.IsOpen = true;

			if(ConfigManager.UpdatedVersion != null)
			{
#if(!SQUIRREL)
				Updater.Cleanup();
#endif
				MainWindow.FlyoutUpdateNotes.IsOpen = true;
				MainWindow.UpdateNotesControl.SetHighlight(ConfigManager.PreviousVersion);
			}
			NetDeck.CheckForChromeExtention();
			DataIssueResolver.Run();

#if(!SQUIRREL)
			Helper.CopyReplayFiles();
#endif
			BackupManager.Run();

			if(Config.Instance.PlayerWindowOnStart)
				Windows.PlayerWindow.Show();
//<!--allan add for graveryard-->
            if (Config.Instance.GraveYardWindowOnStart) {
                Windows.GraveryWindow.Show();
            }
			if(Config.Instance.OpponentWindowOnStart)
				Windows.OpponentWindow.Show();
			if(Config.Instance.TimerWindowOnStartup)
				Windows.TimerWindow.Show();

			if(Config.Instance.HearthStatsSyncOnStart && HearthStatsAPI.IsLoggedIn)
				HearthStatsManager.SyncAsync(background: true);
            //allan add for plugins use
            //if (!Config.Instance.ALLAN_LAST_SAVE_VERSION.Equals(Helper.getAllanCurrentVersionStr())) {
            //    if (Directory.Exists("AllanPlugins"))
            //    {
            //        Log.Debug("Config do it replace plugin.");
            //        if (Directory.Exists("Plugins")) Directory.Delete("Plugins", true);
            //        Directory.CreateDirectory("Plugins");
            //        string appDataPluginDir = Path.Combine(Config.AppDataPath, "Plugins");
            //        if (!Directory.Exists(appDataPluginDir))
            //            Directory.CreateDirectory(appDataPluginDir);

            //        string appDataanyfin = Path.Combine(Config.AppDataPath, "anyfin.xml");
            //        if (File.Exists(appDataanyfin)) File.Delete(appDataanyfin);

            //        string appDataPluginXml = Path.Combine(Config.AppDataPath, "plugins.xml");
            //        if (File.Exists(appDataPluginXml)) File.Delete(appDataPluginXml);

            //        CopyFolder("AllanPlugins", appDataPluginDir);
            //        Config.Instance.ALLAN_LAST_SAVE_VERSION = Helper.getAllanCurrentVersionStr();
            //        Config.Save();
            //    }
            //}
            //allan add
			PluginManager.Instance.LoadPlugins();
			MainWindow.Options.OptionsTrackerPlugins.Load();
			PluginManager.Instance.StartUpdateAsync();

			UpdateOverlayAsync();

			if(Config.Instance.ShowCapturableOverlay)
			{
				Windows.CapturableOverlay = new CapturableOverlayWindow();
				Windows.CapturableOverlay.Show();
			}

			if(LogConfigUpdater.LogConfigUpdateFailed)
				MainWindow.ShowLogConfigUpdateFailedMessage().Forget();
			else if(LogConfigUpdater.LogConfigUpdated && Game.IsRunning)
			{
				MainWindow.ShowMessageAsync("炉石需要重启", "log.config文件被改变了，HDT可能工作不正常直到炉石重启.");
				Overlay.ShowRestartRequiredWarning();
			}
			LogReaderManager.Start(Game).Forget();

			NewsUpdater.UpdateAsync();
			HotKeyManager.Load();

			if(Helper.HearthstoneDirExists && Config.Instance.StartHearthstoneWithHDT && !Game.IsRunning)
				Helper.StartHearthstoneAsync().Forget();

			ApiWrapper.UpdateAccountStatus().Forget();

			Initialized = true;

			Influx.OnAppStart(Helper.GetCurrentVersion(), loginType, newUser, (int)(DateTime.UtcNow - _startUpTime).TotalSeconds);
		}

		private static async void UpdateOverlayAsync()
		{
#if(!SQUIRREL)
			if(Config.Instance.CheckForUpdates)
				Updater.CheckForUpdates(true);
#endif
			var hsForegroundChanged = false;
			var useNoDeckMenuItem = TrayIcon.NotifyIcon.ContextMenu.MenuItems.IndexOfKey("startHearthstone");
			while(UpdateOverlay)
			{
				if(Config.Instance.CheckForUpdates)
					Updater.CheckForUpdates();
				if(User32.GetHearthstoneWindow() != IntPtr.Zero)
				{
					if(Game.CurrentRegion == Region.UNKNOWN)
					{
						//game started
						Game.CurrentRegion = Helper.GetCurrentRegion();
						if(Game.CurrentRegion != Region.UNKNOWN)
						{
							BackupManager.Run();
							Game.MetaData.HearthstoneBuild = null;
						}
					}
					Overlay.UpdatePosition();

					if(!Game.IsRunning)
					{
						Overlay.Update(true);
						Windows.CapturableOverlay?.UpdateContentVisibility();
					}

					MainWindow.BtnStartHearthstone.Visibility = Visibility.Collapsed;
					TrayIcon.NotifyIcon.ContextMenu.MenuItems[useNoDeckMenuItem].Visible = false;

					Game.IsRunning = true;

					Helper.GameWindowState = User32.GetHearthstoneWindowState();
					Windows.CapturableOverlay?.Update();
					if(User32.IsHearthstoneInForeground() && Helper.GameWindowState != WindowState.Minimized)
					{
						if(hsForegroundChanged)
						{
							Overlay.Update(true);
							if(Config.Instance.WindowsTopmostIfHsForeground && Config.Instance.WindowsTopmost)
							{
								//if player topmost is set to true before opponent:
								//clicking on the playerwindow and back to hs causes the playerwindow to be behind hs.
								//other way around it works for both windows... what?
								Windows.OpponentWindow.Topmost = true;
								Windows.PlayerWindow.Topmost = true;
								Windows.TimerWindow.Topmost = true;
							}
							hsForegroundChanged = false;
						}
					}
					else if(!hsForegroundChanged)
					{
						if(Config.Instance.WindowsTopmostIfHsForeground && Config.Instance.WindowsTopmost)
						{
							Windows.PlayerWindow.Topmost = false;
							Windows.OpponentWindow.Topmost = false;
							Windows.TimerWindow.Topmost = false;
						}
						hsForegroundChanged = true;
					}
				}
				else if(Game.IsRunning)
				{
					Game.IsRunning = false;
					Overlay.ShowOverlay(false);
					if(Windows.CapturableOverlay != null)
					{
						Windows.CapturableOverlay.UpdateContentVisibility();
						await Task.Delay(100);
						Windows.CapturableOverlay.ForcedWindowState = WindowState.Minimized;
						Windows.CapturableOverlay.WindowState = WindowState.Minimized;
					}
					Log.Info("Exited game");
					Game.CurrentRegion = Region.UNKNOWN;
					Log.Info("Reset region");
					await Reset();
					Game.IsInMenu = true;
					Game.InvalidateMatchInfoCache();
					Overlay.HideRestartRequiredWarning();
					Helper.ClearCachedHearthstoneBuild();
					TurnTimer.Instance.Stop();

					MainWindow.BtnStartHearthstone.Visibility = Visibility.Visible;
					TrayIcon.NotifyIcon.ContextMenu.MenuItems[useNoDeckMenuItem].Visible = true;

					if(Config.Instance.CloseWithHearthstone)
						MainWindow.Close();
				}

				if(Config.Instance.NetDeckClipboardCheck.HasValue && Config.Instance.NetDeckClipboardCheck.Value && Initialized
				   && !User32.IsHearthstoneInForeground())
					NetDeck.CheckForClipboardImport();

				await Task.Delay(UpdateDelay);
			}
			CanShutdown = true;
		}

		private static bool _resetting;
		public static async Task Reset()
		{

			if(_resetting)
			{
				Log.Warn("Reset already in progress.");
				return;
			}
			_resetting = true;
			var stoppedReader = await LogReaderManager.Stop();
			Game.Reset();
			if(DeckList.Instance.ActiveDeck != null)
			{
				Game.IsUsingPremade = true;
				MainWindow.UpdateMenuItemVisibility();
			}
			await Task.Delay(1000);
			if(stoppedReader)
				LogReaderManager.Restart();
			Overlay.HideSecrets();
			Overlay.Update(false);
			UpdatePlayerCards(true);
			_resetting = false;
		}

		internal static async void UpdatePlayerCards(bool reset = false)
		{
			_updateRequestsPlayer++;
			await Task.Delay(100);
			_updateRequestsPlayer--;
			if(_updateRequestsPlayer > 0)
				return;
			Overlay.UpdatePlayerCards(new List<Card>(Game.Player.PlayerCardList), reset);
			if(Windows.PlayerWindow.IsVisible)
				Windows.PlayerWindow.UpdatePlayerCards(new List<Card>(Game.Player.PlayerCardList), reset);
		}
		//allan add
        internal static async void UpdateGraveyardCards(bool reset = false)
        {
            await Task.Delay(100);
            if (Windows.GraveryWindow.IsVisible)
            {
                Windows.GraveryWindow.UpdateGraveyardCards(reset);//TODO 修改了方式 <!--allan add for graveryard-->
            }
        }//allan add end
		internal static async void UpdateOpponentCards(bool reset = false)
		{
			_updateRequestsOpponent++;
			await Task.Delay(100);
			_updateRequestsOpponent--;
			if(_updateRequestsOpponent > 0)
				return;
			Overlay.UpdateOpponentCards(new List<Card>(Game.Opponent.OpponentCardList), reset);
			if(Windows.OpponentWindow.IsVisible)
				Windows.OpponentWindow.UpdateOpponentCards(new List<Card>(Game.Opponent.OpponentCardList), reset);
		}


		public static class Windows
		{
			private static PlayerWindow _playerWindow;
			private static OpponentWindow _opponentWindow;
			private static TimerWindow _timerWindow;
			private static StatsWindow _statsWindow;
            private static GraveyardWindow _graveryWindow;//<!--allan add for graveryard-->
            public static GraveyardWindow GraveryWindow => _graveryWindow ?? (_graveryWindow = new GraveyardWindow(Game));
            public static PlayerWindow PlayerWindow => _playerWindow ?? (_playerWindow = new PlayerWindow(Game));
			public static OpponentWindow OpponentWindow => _opponentWindow ?? (_opponentWindow = new OpponentWindow(Game));
			public static TimerWindow TimerWindow => _timerWindow ?? (_timerWindow = new TimerWindow(Config.Instance));
			public static StatsWindow StatsWindow => _statsWindow ?? (_statsWindow = new StatsWindow());
			public static CapturableOverlayWindow CapturableOverlay;
		}
	}
}
