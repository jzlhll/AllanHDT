#region

using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Hearthstone_Deck_Tracker.Plugins;
using Hearthstone_Deck_Tracker.Utility.Extensions;
using Hearthstone_Deck_Tracker.Windows;

#endregion

namespace Hearthstone_Deck_Tracker.FlyoutControls.Options.Tracker
{
	/// <summary>
	/// Interaction logic for TrackerPlugins.xaml
	/// </summary>
	public partial class TrackerPlugins : UserControl
	{
		public TrackerPlugins()
		{
			InitializeComponent();
		}

		public void Load()
		{
			ListBoxPlugins.ItemsSource = PluginManager.Instance.Plugins;
			if(ListBoxPlugins.Items.Count > 0)
				ListBoxPlugins.SelectedIndex = 0;
			else
				GroupBoxDetails.Visibility = Visibility.Hidden;
		}

		private void ListBoxPlugins_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
		}

		private void ButtonSettings_OnClick(object sender, RoutedEventArgs e)
		{
			(ListBoxPlugins.SelectedItem as PluginWrapper)?.OnButtonPress();
		}

		private void ButtonAvailablePlugins_OnClick(object sender, RoutedEventArgs e) => Helper.TryOpenUrl(@"https://github.com/HearthSim/Hearthstone-Deck-Tracker/wiki/Available-Plugins");

		private void ButtonOpenPluginsFolder_OnClick(object sender, RoutedEventArgs e)
		{
			var dir = PluginManager.PluginDirectory;
			if(!dir.Exists)
			{
				try
				{
					dir.Create();
				}
				catch(Exception)
				{
					Core.MainWindow.ShowMessage("错误",
												$"插件目录不能找到并且不能创建。请手动在目录{dir}下，创建一个目录名字叫做'Plugins'。").Forget();
				}
			}
			Helper.TryOpenUrl(dir.FullName);
		}

        private async void ButtonTryToClean_OnClick(object sender, RoutedEventArgs e)
        {
            
            PluginManager.SavePluginsSettings();
            PluginManager.Instance.UnloadPlugins();
            if (Directory.Exists("AllanPlugins"))
            {
    
                string appDataPluginDir = Path.Combine(Config.AppDataPath, "Plugins");
                if (!Directory.Exists(appDataPluginDir))
                    Directory.CreateDirectory(appDataPluginDir);

                string appDataArena = Path.Combine(Config.AppDataPath, "ArenaHelper");
                string appDataCollectionTracker = Path.Combine(Config.AppDataPath, "CollectionTracker");
                string appDataanyfin = Path.Combine(Config.AppDataPath, "anyfin.xml");

                if (Directory.Exists(appDataArena))
                {
                    Directory.Delete(appDataArena, true);
                }
                if (Directory.Exists(appDataCollectionTracker))
                {
                    Directory.Delete(appDataCollectionTracker, true);
                }
                if (File.Exists(appDataanyfin)) File.Delete(appDataanyfin);

                string appDataPluginXml = Path.Combine(Config.AppDataPath, "plugins.xml");
                if (File.Exists(appDataPluginXml)) File.Delete(appDataPluginXml);

                CopyFolder("AllanPlugins", appDataPluginDir);
                bool isOk = true;
                try
                {
                    if (Directory.Exists("Plugins")) Directory.Delete("Plugins", true);
                    Directory.CreateDirectory("Plugins");
                }
                catch
                {
                    isOk = false;
                    await Core.MainWindow.ShowMessage("提示",
                                                "稍后程序会自动关闭，但是，请手动删除解压程序的Plugins目录即可完成清理！");
                }
                if (isOk) {
                    await Core.MainWindow.ShowMessage("提示",
                                                "马上程序会自动关闭！");
                }
                Application.Current.Shutdown();
            }
        }

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
    }
}