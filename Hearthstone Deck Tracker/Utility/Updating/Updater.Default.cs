#if(!SQUIRREL)
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Hearthstone_Deck_Tracker.Utility.Logging;
using Hearthstone_Deck_Tracker.Windows;
using MahApps.Metro.Controls.Dialogs;

namespace Hearthstone_Deck_Tracker.Utility.Updating
{
	internal static partial class Updater
	{
		private static bool _showingUpdateMessage;
		private static GitHub.Release _release;
		private static bool TempUpdateCheckDisabled { get; set; }

		private static bool ShouldCheckForUpdates()
			=> Config.Instance.CheckForUpdates && !TempUpdateCheckDisabled && !Core.Game.IsRunning && !_showingUpdateMessage
				&& DateTime.Now - _lastUpdateCheck >= new TimeSpan(0, 10, 0);

		public static async void CheckForUpdates(bool force = false)
		{
			if(!force && !ShouldCheckForUpdates())
				return;
			_lastUpdateCheck = DateTime.Now;
			_release = await GetLatestRelease(false);
			if(_release != null)
			{
				StatusBar.Visibility = Visibility.Visible;
				ShowNewUpdateMessage(false);
			}
			else if(Config.Instance.CheckForBetaUpdates)
			{
				_release = await GetLatestRelease(true);
				if(_release != null)
					ShowNewUpdateMessage(true);
			}
		}

		private static async void ShowNewUpdateMessage(bool beta)
		{
			if(_showingUpdateMessage)
				return;
			_showingUpdateMessage = true;
			//Allan Add
            var settings = new MessageDialogs.Settings { AffirmativeButtonText = "下载", NegativeButtonText = "稍后" };
            if (_release == null)
            {
                _showingUpdateMessage = false;
                return;
            }
            try
            {
                await Task.Delay(3000);
                Core.MainWindow.ActivateWindow();
                while (Core.MainWindow.Visibility != Visibility.Visible || Core.MainWindow.WindowState == WindowState.Minimized)
                    await Task.Delay(100);
                var betaString = beta ? " BETA版" : "";
                GitHub.AllanRelease allanRel = await GitHub.GetAllAllanRelease();
                var result =
                    await
                        Core.MainWindow.ShowMessageAsync("新的更新" + betaString + "到了!",
                            "点击 \"下载\" 开始自动下载.\r\n" + allanRel.Assets[0].Title + "\r\n" + allanRel.Assets[0].Body,
                            MessageDialogStyle.AffirmativeAndNegative, settings);
			//Allan Add end
				if(result == MessageDialogResult.Affirmative)
					StartUpdate();
				else
					TempUpdateCheckDisabled = true;

				_showingUpdateMessage = false;
			}
			catch(Exception e)
			{
				_showingUpdateMessage = false;
				Log.Error("Error showing new update message\n" + e);
			}
		}

		internal static async void StartUpdate()
		{
			Log.Info("Starting update...");
			if(_release == null || DateTime.Now - _lastUpdateCheck > new TimeSpan(0, 10, 0))
				_release = await GetLatestRelease(Config.Instance.CheckForBetaUpdates);
			if(_release == null)
			{
				Log.Error("Could not get latest version. Not updating.");
				return;
			}
			//Allan Add
			try
			{
                if (_release.Assets.Count == 1) {
                    Process.Start("HDTUpdate.exe", $"{Process.GetCurrentProcess().Id} {_release.Assets[0].Url}");
                } else if (_release.Assets.Count == 2) {
                    bool b = (DateTime.Now.Minute % 2) == 0;
                    if (b)
                    {
                        Process.Start("HDTUpdate.exe", $"{Process.GetCurrentProcess().Id} {_release.Assets[0].Url} {_release.Assets[1].Url}");
                    }
                    else {
                        Process.Start("HDTUpdate.exe", $"{Process.GetCurrentProcess().Id} {_release.Assets[1].Url} {_release.Assets[0].Url}");
                    }
                }
                else if (_release.Assets.Count == 3)
                {
                    switch (DateTime.Now.Minute % 3) {
                        case 0:
                            Process.Start("HDTUpdate.exe", $"{Process.GetCurrentProcess().Id} {_release.Assets[0].Url} {_release.Assets[1].Url} {_release.Assets[2].Url}");
                            break;
                        case 1:
                            Process.Start("HDTUpdate.exe", $"{Process.GetCurrentProcess().Id} {_release.Assets[1].Url} {_release.Assets[0].Url} {_release.Assets[2].Url}");
                            break;
                        case 2:
                            Process.Start("HDTUpdate.exe", $"{Process.GetCurrentProcess().Id} {_release.Assets[2].Url} {_release.Assets[0].Url} {_release.Assets[1].Url}");
                            break;
                    }
                }
				//Allan Add end
				Core.MainWindow.Close();
				Application.Current.Shutdown();
			}
			catch(Exception ex)
			{
				Log.Error("Error starting updater\n" + ex);
				Helper.TryOpenUrl($"{_release.Assets[0].Url}");
			}
		}

		public static void Cleanup()
		{
			try
			{
				if(File.Exists("HDTUpdate_new.exe"))
				{
					if(File.Exists("HDTUpdate.exe"))
						File.Delete("HDTUpdate.exe");
					File.Move("HDTUpdate_new.exe", "HDTUpdate.exe");
				}
			}
			catch(Exception e)
			{
				Log.Error("Error updating updater\n" + e);
			}
			try
			{
				//updater used pre v0.9.6
				if(File.Exists("Updater.exe"))
					File.Delete("Updater.exe");
			}
			catch(Exception e)
			{
				Log.Error("Error deleting Updater.exe\n" + e);
			}
		}

		//Allan Add modify
        private static async Task<GitHub.Release> GetLatestRelease(bool beta)
        {
            //var currentVersion = Helper.GetCurrentVersion();

            //if(currentVersion == null)
            //	return null;
            var exFiles = Directory.GetFiles(Environment.CurrentDirectory);
            if (exFiles != null)
            {
                try
                {
                    foreach (var file in exFiles)
                    {
                        Log.Error("file " + file);
                        if (file.Contains("更新说明") && !file.Contains(Helper.getAllanCurrentDateStr()))
                        {
                            File.Delete(file);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error("删除失败。" + e);
                }
            }

            if (File.Exists(Environment.CurrentDirectory + "\\HDT汉化高级版.exe"))
            {
                try
                {
                    File.Delete(Environment.CurrentDirectory + "\\HDT汉化高级版.exe");
                }
                catch (Exception e)
                {
                    Log.Warn("" + e);
                    MessageBox.Show("建议关闭HDT，点击Hearthstone Deck Tracker.exe运行,要删除HDT汉化高级版.exe。", "重启");
                }
            }
            return await GitHub.CheckForUpdate("jzlhll", "AllanHDT", Helper.GetAllanCurrentVersion(), beta);
        }
    }
}

#endif
