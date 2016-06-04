#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Navigation;
using Hearthstone_Deck_Tracker.Annotations;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.HearthStats.API;
using Hearthstone_Deck_Tracker.Utility.Logging;
using MahApps.Metro.Controls.Dialogs;

#endregion

namespace Hearthstone_Deck_Tracker
{
	/// <summary>
	/// Interaction logic for StartupWindow.xaml
	/// </summary>
	public partial class LoginWindow : INotifyPropertyChanged
	{
		private readonly bool _initialized;
		private ProgressDialogController _controller;

		public LoginWindow()
		{
			InitializeComponent();
			CheckBoxRememberLogin.IsChecked = Config.Instance.RememberHearthStatsLogin;
			_initialized = true;
		}

		public LoginType LoginResult { get; private set; } = LoginType.None;

		public event PropertyChangedEventHandler PropertyChanged;

		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) => Helper.TryOpenUrl(e.Uri.AbsoluteUri);

		private async void BtnLogin_Click(object sender, RoutedEventArgs e)
		{
			var email = TextBoxEmail.Text;
			if(string.IsNullOrEmpty(email) || !Regex.IsMatch(email, @".*@.*\..*"))
			{
				DisplayLoginError("请输入有效的电子邮件地址");
				return;
			}
			if(string.IsNullOrEmpty(TextBoxPassword.Password))
			{
				DisplayLoginError("请输入密码");
				return;
			}
			IsEnabled = false;
			_controller = await this.ShowProgressAsync("登陆中.", "");
			var result = await HearthStatsAPI.LoginAsync(TextBoxEmail.Text, TextBoxPassword.Password);
			TextBoxPassword.Clear();
			if(result.Success)
			{
				LoginResult = LoginType.Login;
				Close();
			}
			else if(result.Message.Contains("401"))
				DisplayLoginError("错误的信息");
			else
				DisplayLoginError(result.Message);
		}

		private async void DisplayLoginError(string error)
		{
			TextBlockErrorMessage.Text = error;
			TextBlockErrorMessage.Visibility = Visibility.Visible;
			IsEnabled = true;
			if(_controller?.IsOpen ?? false)
				await _controller.CloseAsync();
		}

		private void CheckBoxRememberLogin_Checked(object sender, RoutedEventArgs e)
		{
			if(!_initialized)
				return;
			Config.Instance.RememberHearthStatsLogin = true;
			Config.Save();
		}

		private void CheckBoxRememberLogin_OnUnchecked(object sender, RoutedEventArgs e)
		{
			if(!_initialized)
				return;
			Config.Instance.RememberHearthStatsLogin = false;
			Config.Save();
			try
			{
				if(File.Exists(Config.Instance.HearthStatsFilePath))
					File.Delete(Config.Instance.HearthStatsFilePath);
			}
			catch(Exception ex)
			{
				Log.Error("Error deleting hearthstats credentials file\n" + ex);
			}
		}

		private async void BtnRegister_Click(object sender, RoutedEventArgs e)
		{
			if(!CheckBoxPrivacyPolicy.IsChecked == true)
				return;

			var email = TextBoxRegisterEmail.Text;
			if(string.IsNullOrEmpty(email) || !Regex.IsMatch(email, @".*@.*\..*"))
			{
				DisplayLoginError("请输入有效的电子邮件地址");
				return;
			}
			if(string.IsNullOrEmpty(TextBoxRegisterPassword.Password))
			{
				DisplayLoginError("请输入密码");
				return;
			}
			if(TextBoxRegisterPassword.Password.Length < 6)
			{
				DisplayLoginError("密码至少6位");
				return;
			}
			if(string.IsNullOrEmpty(TextBoxRegisterPasswordConfirm.Password))
			{
				DisplayLoginError("重复输入密码");
				return;
			}
			if(!TextBoxRegisterPassword.Password.Equals(TextBoxRegisterPasswordConfirm.Password))
			{
				DisplayLoginError("两次不匹配");
				return;
			}
			IsEnabled = false;
			_controller = await this.ShowProgressAsync("注册中...", "");
			var result = await HearthStatsAPI.RegisterAsync(email, TextBoxRegisterPassword.Password);
			if(result.Success)
			{
				_controller.SetTitle("登陆中...");
				result = await HearthStatsAPI.LoginAsync(email, TextBoxRegisterPassword.Password);
			}
			else if(result.Message.Contains("422"))
				DisplayLoginError("Email已被注册");
			else
				DisplayLoginError(result.Message);
			TextBoxRegisterPassword.Clear();
			TextBoxRegisterPasswordConfirm.Clear();
			if(result.Success)
			{
				LoginResult = LoginType.Register;
				Close();
			}
		}

		private void CheckBoxPrivacyPolicy_Checked(object sender, RoutedEventArgs e) => BtnRegister.IsEnabled = true;

		private void CheckBoxPrivacyPolicy_OnUnchecked(object sender, RoutedEventArgs e) => BtnRegister.IsEnabled = false;

		private void Button_Continue(object sender, RoutedEventArgs e)
		{
			Log.Info("Continuing...");
			LoginResult = LoginType.Guest;
			Config.Instance.ShowLoginDialog = false;
			Config.Save();
			Close();
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void BtnShowRegister_OnClick(object sender, RoutedEventArgs e) => TabControlLoginRegister.SelectedIndex = 1;
	}
}