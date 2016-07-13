﻿#region

using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Hearthstone_Deck_Tracker.Hearthstone;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using SystemColors = System.Windows.SystemColors;
using Hearthstone_Deck_Tracker.Utility.Logging;

#endregion

namespace Hearthstone_Deck_Tracker.FlyoutControls.Options.Overlay
{
	/// <summary>
	/// Interaction logic for DeckWindows.xaml
	/// </summary>
	public partial class OverlayDeckWindows
	{
		private GameV2 _game;
		private bool _initialized;

		public OverlayDeckWindows()
		{
			InitializeComponent();
		}

		public void Load(GameV2 game)
		{
			_game = game;
			CheckboxWindowsTopmost.IsChecked = Config.Instance.WindowsTopmost;
			CheckboxPlayerWindowOpenAutomatically.IsChecked = Config.Instance.PlayerWindowOnStart;
			//<!--allan add for graveryard-->
            CheckboxGraveyardWindowOpenAutomatically.IsChecked = Config.Instance.GraveYardWindowOnStart;
            CheckboxOpponentWindowOpenAutomatically.IsChecked = Config.Instance.OpponentWindowOnStart;
			CheckboxTimerTopmost.IsChecked = Config.Instance.TimerWindowTopmost;
			CheckboxTimerWindow.IsChecked = Config.Instance.TimerWindowOnStartup;
			CheckboxTimerTopmostHsForeground.IsChecked = Config.Instance.TimerWindowTopmostIfHsForeground;
			CheckboxTimerTopmostHsForeground.IsEnabled = Config.Instance.TimerWindowTopmost;
			CheckboxWinTopmostHsForeground.IsChecked = Config.Instance.WindowsTopmostIfHsForeground;
			CheckboxWinTopmostHsForeground.IsEnabled = Config.Instance.WindowsTopmost;
			ComboboxWindowBackground.SelectedItem = Config.Instance.SelectedWindowBackground;
			TextboxCustomBackground.IsEnabled = Config.Instance.SelectedWindowBackground == "自定义"; //Custom
			TextboxCustomBackground.Text = string.IsNullOrEmpty(Config.Instance.WindowsBackgroundHex)
				                               ? "#696969" : Config.Instance.WindowsBackgroundHex;
			UpdateAdditionalWindowsBackground();
			CheckboxWindowCardToolTips.IsChecked = Config.Instance.WindowCardToolTips;
			_initialized = true;
		}

		private void SaveConfig(bool updateOverlay)
		{
			Config.Save();
			if(updateOverlay)
				Core.Overlay.Update(true);
		}

		private void CheckboxWindowsTopmost_Checked(object sender, RoutedEventArgs e)
		{
			if(!_initialized)
				return;
			Config.Instance.WindowsTopmost = true;
			Core.Windows.PlayerWindow.Topmost = true;
			Core.Windows.OpponentWindow.Topmost = true;
			CheckboxWinTopmostHsForeground.IsEnabled = true;
			SaveConfig(true);
		}

		private void CheckboxWindowsTopmost_Unchecked(object sender, RoutedEventArgs e)
		{
			if(!_initialized)
				return;
			Config.Instance.WindowsTopmost = false;
			Core.Windows.PlayerWindow.Topmost = false;
			Core.Windows.OpponentWindow.Topmost = false;
			CheckboxWinTopmostHsForeground.IsEnabled = false;
			CheckboxWinTopmostHsForeground.IsChecked = false;
			SaveConfig(true);
		}

		private void CheckboxWinTopmostHsForeground_Checked(object sender, RoutedEventArgs e)
		{
			if(!_initialized)
				return;
			Config.Instance.WindowsTopmostIfHsForeground = true;
			Core.Windows.PlayerWindow.Topmost = false;
			Core.Windows.OpponentWindow.Topmost = false;
			SaveConfig(false);
		}

		private void CheckboxWinTopmostHsForeground_Unchecked(object sender, RoutedEventArgs e)
		{
			if(!_initialized)
				return;
			Config.Instance.WindowsTopmostIfHsForeground = false;
			if(Config.Instance.WindowsTopmost)
			{
				Core.Windows.PlayerWindow.Topmost = true;
				Core.Windows.OpponentWindow.Topmost = true;
			}
			SaveConfig(false);
		}

		private void CheckboxTimerTopmost_Checked(object sender, RoutedEventArgs e)
		{
			if(!_initialized)
				return;
			Config.Instance.TimerWindowTopmost = true;
			Core.Windows.TimerWindow.Topmost = true;
			CheckboxTimerTopmostHsForeground.IsEnabled = true;
			SaveConfig(true);
		}

		private void CheckboxTimerTopmost_Unchecked(object sender, RoutedEventArgs e)
		{
			if(!_initialized)
				return;
			Config.Instance.TimerWindowTopmost = false;
			Core.Windows.TimerWindow.Topmost = false;
			CheckboxTimerTopmostHsForeground.IsEnabled = false;
			CheckboxTimerTopmostHsForeground.IsChecked = false;
			SaveConfig(true);
		}

		private void CheckboxTimerWindow_Checked(object sender, RoutedEventArgs e)
		{
			if(!_initialized)
				return;
			Core.Windows.TimerWindow.Show();
			Core.Windows.TimerWindow.Activate();
			Config.Instance.TimerWindowOnStartup = true;
			SaveConfig(true);
		}

		private void CheckboxTimerWindow_Unchecked(object sender, RoutedEventArgs e)
		{
			if(!_initialized)
				return;
			Core.Windows.TimerWindow.Hide();
			Config.Instance.TimerWindowOnStartup = false;
			SaveConfig(true);
		}

		private void CheckboxTimerTopmostHsForeground_Checked(object sender, RoutedEventArgs e)
		{
			if(!_initialized)
				return;
			Config.Instance.TimerWindowTopmostIfHsForeground = true;
			Core.Windows.TimerWindow.Topmost = false;
			SaveConfig(false);
		}

		private void CheckboxTimerTopmostHsForeground_Unchecked(object sender, RoutedEventArgs e)
		{
			if(!_initialized)
				return;
			Config.Instance.TimerWindowTopmostIfHsForeground = false;
			if(Config.Instance.TimerWindowTopmost)
				Core.Windows.TimerWindow.Topmost = true;
			SaveConfig(false);
		}

		private void ComboboxWindowBackground_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(!_initialized)
				return;
			TextboxCustomBackground.IsEnabled = ComboboxWindowBackground.SelectedItem.ToString() == "自定义"; //Custom
			Config.Instance.SelectedWindowBackground = ComboboxWindowBackground.SelectedItem.ToString();
			UpdateAdditionalWindowsBackground();
		}

		private void TextboxCustomBackground_TextChanged(object sender, TextChangedEventArgs e)
		{
			if(!_initialized || ComboboxWindowBackground.SelectedItem.ToString() != "自定义") //Custom
                return;
			var background = Helper.BrushFromHex(TextboxCustomBackground.Text);
			if(background != null)
			{
				UpdateAdditionalWindowsBackground(background);
				Config.Instance.WindowsBackgroundHex = TextboxCustomBackground.Text;
				SaveConfig(false);
			}
		}

		private void CheckboxWindowCardToolTips_Checked(object sender, RoutedEventArgs e)
		{
			if(!_initialized)
				return;
			Config.Instance.WindowCardToolTips = true;
			SaveConfig(false);
		}

		private void CheckboxWindowCardToolTips_Unchecked(object sender, RoutedEventArgs e)
		{
			if(!_initialized)
				return;
			Config.Instance.WindowCardToolTips = false;
			SaveConfig(false);
		}

		private void CheckboxPlayerWindowOpenAutomatically_Checked(object sender, RoutedEventArgs e)
		{
			if(!_initialized)
				return;
			Core.Windows.PlayerWindow.Show();
			Core.Windows.PlayerWindow.Activate();
			Core.Windows.PlayerWindow.SetCardCount(_game.Player.HandCount, _game.IsInMenu ? 30 : _game.Player.DeckCount);
			Config.Instance.PlayerWindowOnStart = true;
			Config.Save();
		}

		private void CheckboxPlayerWindowOpenAutomatically_Unchecked(object sender, RoutedEventArgs e)
		{
			if(!_initialized)
				return;
			Core.Windows.PlayerWindow.Hide();
			Config.Instance.PlayerWindowOnStart = false;
			Config.Save();
		}

		private void CheckboxOpponentWindowOpenAutomatically_Checked(object sender, RoutedEventArgs e)
		{
			if(!_initialized)
				return;
			Core.Windows.OpponentWindow.Show();
			Core.Windows.OpponentWindow.Activate();
			Core.Windows.OpponentWindow.SetOpponentCardCount(_game.Opponent.HandCount, _game.IsInMenu ? 30 : _game.Opponent.DeckCount, _game.Opponent.HasCoin);
			Config.Instance.OpponentWindowOnStart = true;
			Config.Save();
		}

		private void CheckboxOpponentWindowOpenAutomatically_Unchecked(object sender, RoutedEventArgs e)
		{
			if(!_initialized)
				return;
			Core.Windows.OpponentWindow.Hide();
			Config.Instance.OpponentWindowOnStart = false;
			Config.Save();
		}

		internal void UpdateAdditionalWindowsBackground(Brush brush = null)
		{
			var background = brush;
            try {
                switch (ComboboxWindowBackground.SelectedItem.ToString())
                {
                    case "主题"://Theme
                        background = Background;
                        break;
                    case "亮色": //Light
                        background = SystemColors.ControlLightBrush;
                        break;
                    case "暗色"://Dark
                        background = SystemColors.ControlDarkDarkBrush;
                        break;
                }

                if (background == null)
                {
                    var hexBackground = Helper.BrushFromHex(TextboxCustomBackground.Text);
                    if (hexBackground != null)
                    {
                        Core.Windows.PlayerWindow.Background = hexBackground;
                        Core.Windows.GraveryWindow.Background = hexBackground;//<!--allan add for graveryard-->
                        Core.Windows.OpponentWindow.Background = hexBackground;
                        Core.Windows.TimerWindow.Background = hexBackground;
                    }
                }
                else
                {
                    Core.Windows.PlayerWindow.Background = background;
                    Core.Windows.OpponentWindow.Background = background;
                    Core.Windows.GraveryWindow.Background = background;//<!--allan add for graveryard-->
                    Core.Windows.TimerWindow.Background = background;
                }
                
            } catch (System.NullReferenceException e) {
                MessageBox.Show("如果出现该报错,一般重新运行就可以了!!!如果一直报错，请删除 "
                                + Config.Instance.ConfigPath + "删除config.xml文件，再次启动HDT！", "重启HDT！");
                if (System.IO.File.Exists(Config.Instance.ConfigPath)) {
                    System.IO.File.Delete(Config.Instance.ConfigPath);
                }
                Application.Current.Shutdown();
            }
		}
		//<!--allan add for graveryard-->
        private void CheckboxGraveyardWindowOpenAutomatically_Checked(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
                return;
            Core.Windows.GraveryWindow.Show();
            Core.Windows.GraveryWindow.Activate();
            Config.Instance.GraveYardWindowOnStart = true;
            Config.Save();
        }

        private void CheckboxGraveyardWindowOpenAutomatically_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!_initialized)
                return;
            Core.Windows.GraveryWindow.Hide();
            Config.Instance.GraveYardWindowOnStart = false;
            Config.Save();
        }

        private void CheckboxGraveyardWindowIfWithGenerate_Checked(object sender, RoutedEventArgs e)
        {
            Config.Instance.GraveYardWindowIfCreated = true;
            if(Core.Windows.GraveryWindow.Visibility == Visibility.Visible) Core.Windows.GraveryWindow.UpdateGraveyardCards(false);
            Config.Save();
        }

        private void CheckboxGraveyardWindowIfWithGenerate_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Core.Windows.GraveryWindow.Visibility == Visibility.Visible) Core.Windows.GraveryWindow.UpdateGraveyardCards(false);
            Config.Instance.GraveYardWindowIfCreated = false;
            Config.Save();
        }
    }
}