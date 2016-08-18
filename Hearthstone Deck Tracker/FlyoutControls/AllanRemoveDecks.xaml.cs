#region

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.HearthStats.API;
using Hearthstone_Deck_Tracker.Utility.Extensions;

using Hearthstone_Deck_Tracker.Stats;

#endregion

namespace Hearthstone_Deck_Tracker
{
	/// <summary>
	/// Interaction logic for TagControl.xaml
	/// </summary>
	public partial class AllanRemoveDecks
	{
		private bool _initialized;
        private ObservableCollection<Version> versions = new ObservableCollection<Version>();
        private List<string> selectedVs = new List<string>();
        public AllanRemoveDecks()
		{
			InitializeComponent();
            
		}

        public void loadCurrentVersions() {
            var deck = Core.MainWindow.DeckPickerList.SelectedDecks.FirstOrDefault();
            string selectedstr = deck.Version.ShortVersionString;
        
            versions.Clear();
            selectedVs.Clear();
            if (deck == null)
                return;
            foreach (var d in deck.VersionsIncludingSelf)
            {
                if (selectedstr != d.ShortVersionString)
                {
                    versions.Add(new Version(d.ShortVersionString, false));
                }
                
            }
            ListboxAllVersions.ItemsSource = versions;
        }

        private class Version
        {
            public Version(string name, bool? selected = false)
            {
                Name = name;
                Selected = selected;
            }

            public string Name { get; set; }
            public bool? Selected { get; set; }

            public override bool Equals(object obj)
            {
                var other = obj as Version;
                if (other == null)
                    return false;
                return other.Name == Name;
            }

            public override int GetHashCode() => Name.GetHashCode();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var originalSource = (DependencyObject)e.OriginalSource;
            while ((originalSource != null) && !(originalSource is CheckBox))
                originalSource = VisualTreeHelper.GetParent(originalSource);

            if (originalSource == null)
                return;

            var selectedValue = (originalSource as CheckBox).Content.ToString();
            addString(selectedValue);
            print();
        }

        private void addString(string added) {

            foreach (string s in selectedVs) {
                if (s == added) {
                    return;
                }
            }

            selectedVs.Add(added);
        }
       
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var originalSource = (DependencyObject)e.OriginalSource;
            while ((originalSource != null) && !(originalSource is CheckBox))
                originalSource = VisualTreeHelper.GetParent(originalSource);

            if (originalSource == null)
                return;
            var selectedValue = (originalSource as CheckBox).Content.ToString();
            selectedVs.Remove(selectedValue);
            print();
        }

        private void BtnDeteleVersions_Click(object sender, RoutedEventArgs e)
        {
            if (selectedVs.Count == 0) {
                return;
            }
            Deck deck = Core.MainWindow.DeckPickerList.SelectedDecks.FirstOrDefault();
            //if (selectedVs.Count == deck.Versions.Count + 1) {
            //    MessageBox.Show("必须留下一个版本", "提示", MessageBoxButton.OK);
            //    return;
            //}
            var msgbxoResult = MessageBox.Show("选中的版本将从所有卡组中移除！", "确定?", MessageBoxButton.YesNo,
                                               MessageBoxImage.Exclamation);
            if (msgbxoResult != MessageBoxResult.Yes)
                return;

            foreach (string v in selectedVs)
            {
                for (int j = 0; j < deck.Versions.Count; j++) {
                    if (deck.Versions[j].Version.ShortVersionString.Contains(v))
                    {
                        deck.Versions.RemoveAt(j);
                        break;
                    }
                }
            }

            var vers = deck.VersionsIncludingSelf;
            deck.SelectVersion(vers[vers.Count - 1]);
            DeckList.Save();
            Core.MainWindow.DeckPickerList.UpdateDecks(forceUpdate: new[] { deck });
            Core.MainWindow.UpdateDeckList(deck);
            Core.MainWindow.ManaCurveMyDecks.UpdateValues();
            if (deck.Equals(DeckList.Instance.ActiveDeck))
                Core.MainWindow.UseDeck(deck);
            loadCurrentVersions();
            
        }
         
        private void print() {
            //foreach (string s in selectedVs) {
            //    Log.Info("selected " + s);
            //}
            //foreach (var d in DeckList.Instance.Decks) {
            //    Log.Info("DeckList " + d.NameAndVersion);
            //    foreach (var c in d.Versions) {
            //        Log.Info("DeckList versions " + c.NameAndVersion + ";"  + c.ToString());
            //    }
            //}
            //deck?.VersionsIncludingSelf
            //deck.SelectVersion(version);
            // DeckList.Save();
            //DeckPickerList.UpdateDecks(forceUpdate: new[] { deck });
            // UpdateDeckList(deck);
            // ManaCurveMyDecks.UpdateValues();
            //  if (deck.Equals(DeckList.Instance.ActiveDeck))
            //      UseDeck(deck);

        }
    }
}