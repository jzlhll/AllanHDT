#region

#region

// ReSharper disable RedundantUsingDirective
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Garlic;
using Hearthstone_Deck_Tracker.Controls.Error;
using Hearthstone_Deck_Tracker.Plugins;
using Hearthstone_Deck_Tracker.Utility.Analytics;
using Hearthstone_Deck_Tracker.Utility.Extensions;

#endregion

#endregion

namespace Hearthstone_Deck_Tracker
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private static bool _createdReport;

		private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			if(e.Exception is MissingMethodException || e.Exception is MissingFieldException || e.Exception is MissingMemberException || e.Exception is TypeLoadException)
			{
				var plugin =
					PluginManager.Instance.Plugins.FirstOrDefault(p => new FileInfo(p.FileName).Name.Replace(".dll", "") == e.Exception.Source);
				if(plugin != null)
				{
					plugin.IsEnabled = false;
					var header = $"{plugin.NameAndVersion} 不适合HDT版本 {Helper.GetCurrentVersion().ToVersionString()}.";
					ErrorManager.AddError(header, "确保你在使用最新的插件和HDT.\n\n" + e.Exception);
					e.Handled = true;
					return;
				}
			}
			if(!_createdReport)
			{
				_createdReport = true;
				var stackTrace = e.Exception.StackTrace.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
				Google.TrackEvent(e.Exception.GetType().ToString().Split('.').Last(), stackTrace.Length > 0 ? stackTrace[0] : "", stackTrace.Length > 1 ? stackTrace[1] : "");
#if (!DEBUG)
				var date = DateTime.Now;
				var fileName = "Crash Reports\\" + $"Crash report {date.Day}{date.Month}{date.Year}-{date.Hour}{date.Minute}";

				if(!Directory.Exists("Crash Reports"))
					Directory.CreateDirectory("Crash Reports");

				using(var sr = new StreamWriter(fileName + ".txt", true))
				{
					sr.WriteLine("########## " + DateTime.Now + " ##########");
					sr.WriteLine(e.Exception);
					sr.WriteLine(Core.MainWindow.Options.OptionsTrackerLogging.TextBoxLog.Text);
				}

				MessageBox.Show(e.Exception.Message + "\n\n" + "出错啦！有crash.log生成在:\n\"" + Environment.CurrentDirectory + "\\" + fileName
								+ ".txt\"\n\nPlease \na) 请截图在 http://bbs.ngacn.cc/read.php?tid=9444162 原贴上回帖！\n.由于原作者代码和我汉化的原因可能导致出错，深感抱歉!",
								"噢！挂掉了！", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
            }
            e.Handled = true;
			Shutdown();
		}

		private void App_OnStartup(object sender, StartupEventArgs e)
		{
			ShutdownMode = ShutdownMode.OnExplicitShutdown;
			Core.Initialize();
		}
	}
}