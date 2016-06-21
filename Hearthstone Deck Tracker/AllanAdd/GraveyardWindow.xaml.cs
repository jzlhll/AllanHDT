#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using Hearthstone_Deck_Tracker.Annotations;
using Hearthstone_Deck_Tracker.Hearthstone;
using Panel = System.Windows.Controls.Panel;
using Point = System.Drawing.Point;
using Hearthstone_Deck_Tracker.Hearthstone.Entities;
using System.Windows.Media;

#endregion

namespace Hearthstone_Deck_Tracker
{
    /// <summary>
    /// Interaction logic for GraveyardWindow.xaml
    /// </summary>
    public partial class GraveyardWindow : INotifyPropertyChanged
    {
        private readonly GameV2 _game;
        private bool _appIsClosing;
        private int _updateRequests;

        public GraveyardWindow(GameV2 game, List<Card> forScreenshot = null)
        {
            InitializeComponent();

            _game = game;
            Height = Config.Instance.PlayerWindowHeight;
            if (Config.Instance.PlayerWindowLeft.HasValue)
            {
                Left = Config.Instance.PlayerWindowLeft.Value + 200;
                Utility.Logging.Log.Info("AllanLog:Left~~~" + Config.Instance.PlayerWindowLeft.Value);
            }
            else {
                Utility.Logging.Log.Info("AllanLog:No!~~~~~~");
            }
            if (Config.Instance.PlayerWindowTop.HasValue)
                Top = Config.Instance.PlayerWindowTop.Value;
            Topmost = Config.Instance.WindowsTopmost;

            var titleBarCorners = new[]
            {
                new Point((int)Left + 5, (int)Top + 5),
                new Point((int)(Left + Width) - 5, (int)Top + 5),
                new Point((int)Left + 5, (int)(Top + TitlebarHeight) - 5),
                new Point((int)(Left + Width) - 5, (int)(Top + TitlebarHeight) - 5)
            };
            if (!Screen.AllScreens.Any(s => titleBarCorners.Any(c => s.WorkingArea.Contains(c))))
            {
                Top = 100;
                Left = 50;
            }

            if (forScreenshot != null)
            {
                graveTitle.Visibility = Visibility.Collapsed;
                ListViewGraveyard.Update(forScreenshot, true);

                Height = 34 * ListViewGraveyard.Items.Count;
            }
            updateListv(false);
            graveTitle.Background = new SolidColorBrush(Colors.LightGray);
            graveTitle.Foreground = new SolidColorBrush(Colors.Black);
            oppoTitle.Background = new SolidColorBrush(Colors.Transparent);
        }

        public double PlayerDeckMaxHeight => ActualHeight - PlayerLabelsHeight;

        public double PlayerLabelsHeight => graveTitle.ActualHeight + 42;

        public bool ShowToolTip => Config.Instance.WindowCardToolTips;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Update()
        {
            ListViewGraveyard.Visibility = Visibility.Visible;
            graveTitle.Visibility = Visibility.Visible;
        }

        public void UpdatePlayerLayout()
        {
            StackPanelMain.Children.Clear();
            StackPanelMain.Children.Add(ViewBoxGraveyard);
            StackPanelMainOppo.Children.Clear();
            StackPanelMainOppo.Children.Add(ViewBoxOppoDeck);
            OnPropertyChanged(nameof(PlayerDeckMaxHeight));
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_appIsClosing)
                return;
            e.Cancel = true;
            Hide();
        }

        private void GraveyardWindow_OnActivated(object sender, EventArgs e) => Topmost = true;

        internal void Shutdown()
        {
            _appIsClosing = true;
            Close();
        }

        private void GraveyardWindow_OnDeactivated(object sender, EventArgs e)
        {
            if (!Config.Instance.WindowsTopmost)
                Topmost = false;
        }
        public void UpdateOppoDeckCards(List<Card> cards, bool reset) => UpdateOppoDeckListv(cards, reset);
        public void UpdateGraveyardCards(bool reset) => updateListv(reset);

        private List<Card> mGraveyardList = new List<Card>();
       // private List<Card> mOppoDeckList = new List<Card>();

        private int[] myIds;

        public void generateMYIds() {
            Utility.Logging.Log.Info("AllanLog:generateMYIds");
            if (_game.Entities.Values.Count() == 0)
            {
                Utility.Logging.Log.Info("AllanLog:generateMyids = 0");
                myIds = null;
                return;
            }
            myIds = new int[30];
            int i = 0;
            
            foreach (var card in _game.Entities.Values) {
                myIds[i] = card.Id;
                Utility.Logging.Log.Info("AllanLog:generateMyids myid " + myIds[i] + " " + card.Card.LocalizedName);
                i++;
            }
        }

        private bool isDeckById(int id) {
            if (myIds == null)
            {
                generateMYIds();
                return false;
            }
            if (true) { //DEBUG
                int i = 0;
                foreach (var card in _game.Entities.Values)
                {
                    Utility.Logging.Log.Info("AllanLog:show myid " + myIds[i] + " " + card.Card.LocalizedName);
                    i++;
                }
            }
            foreach (int i in myIds) {
                if (i == id) {
                    return true;
                }
            }
            return false;
        }

        private void UpdateOppoDeckListv(List<Card> oppoCards, bool reset) {
            //mOppoDeckList.Clear();
            int oppoSize = oppoCards.Count();
            Utility.Logging.Log.Info("AllanLog:orgSzie " + oppoSize);
            for (int i = 0; i < oppoCards.Count(); i++)
            {
              
                Utility.Logging.Log.Info("AllanLog:OPPO ElementAt(" + i + ") isCreated " + oppoCards.ElementAt(i).IsCreated + " ");
                Utility.Logging.Log.Info("AllanLog:OPPO end");
                 
                //if (isDeckById(graveOrgList.ElementAt(i).Id))
                if (oppoCards.ElementAt(i).IsCreated)
                {
                    oppoCards.RemoveAt(i);
                    i--;
                }
            }
            ListViewOppoDeck.Update(oppoCards, reset);
            if (reset)
            {
                myIds = null;
                //mOppoDeckList.Clear();
            }
        }

        private void updateListv(bool reset)
        {
            IEnumerable<Entity> graveOrgList = Core.Game.Player.Graveyard;
            mGraveyardList.Clear();

            int orgSize = graveOrgList.Count();
            
            Utility.Logging.Log.Info("AllanLog:orgSzie " + orgSize);
            for (int i = 0; i < orgSize; i++)
            {
                //每一个传入的卡
                Card newGrave = Database.GetCardFromId(graveOrgList.ElementAt(i).CardId);
                //Utility.Logging.Log.Info("\nAllanLog:newGrave id= " + newGrave.Id + " Name " + newGrave.LocalizedName 
                //    + " AllanLog:entity id " + graveOrgList.ElementAt(i).Id + " CardId " + graveOrgList.ElementAt(i).CardId);
                bool isWorked = false;
                Utility.Logging.Log.Info("AllanLog:Player ElementAt(" + i + ")" + graveOrgList.ElementAt(i).ToString());
                Utility.Logging.Log.Info("AllanLog:Player newGrave " + newGrave.LocalizedName + " isCreated " + newGrave.IsCreated);
                Utility.Logging.Log.Info("AllanLog:Player end");
                foreach (var grave in mGraveyardList)
                { //修改后的卡数组
                    if (newGrave.Id.Equals(grave.Id))
                    { //如果是同一张卡并且id相同增加count
                        Utility.Logging.Log.Info("AllanLog:Count++ ");
                        isWorked = true;
                        grave.Count++;
                        //是一个新的id
                        break;
                    }
                }
                if (!isWorked)
                {
                    //if (isDeckById(graveOrgList.ElementAt(i).Id))
                    if (graveOrgList.ElementAt(i).Id <= 68 && !graveOrgList.ElementAt(i).Info.Created && !graveOrgList.ElementAt(i).Info.Stolen)
                    {
                        mGraveyardList.Add(newGrave);
                    }
                    else if(Config.Instance.GraveYardWindowIfCreated && isCardIdEndOfNumber(graveOrgList.ElementAt(i).CardId)) { //创建出来的卡,还要过滤掉e,t等结尾
                        newGrave.HighlightInHand = true;
                        mGraveyardList.Add(newGrave);
                    }
                }
            }

            if (mGraveyardList.Count() > 0) {
                mGraveyardList = Utility.Extensions.CardListExtensions.ToSortedCardList(mGraveyardList);
               
            }
            ListViewGraveyard.Update(mGraveyardList, reset);
            if (reset) {
                myIds = null;
                //mOppoDeckList.Clear();
                mGraveyardList.Clear();
            }
        }

        private bool isCardIdEndOfNumber(string cardId) {
            char ch = cardId.ElementAt(cardId.Count() - 1);
            Utility.Logging.Log.Info("ch end " + ch);
            if (ch >= '0'&& ch <= '9') {
                return true;
            }

            return false;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void GraveyardWindow_OnSizeChanged(object sender, SizeChangedEventArgs e) => OnPropertyChanged(nameof(PlayerDeckMaxHeight));

        private void GraveyardWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Utility.Logging.Log.Info("AllanLog:GraveyardWindow_OnLoaded update");
            Update();
            UpdatePlayerLayout();
        }

        private void graveTitle_Click(object sender, RoutedEventArgs e)
        {
             StackPanelMain.Visibility = Visibility.Visible;
            StackPanelMainOppo.Visibility = Visibility.Hidden;
            updateListv(false);
            graveTitle.Background = new SolidColorBrush(Colors.LightGray);
            graveTitle.Foreground = new SolidColorBrush(Colors.Black);
            oppoTitle.Background = new SolidColorBrush(Colors.Transparent);
            oppoTitle.Foreground = new SolidColorBrush(Colors.White);
        }

        private void oppoTitle_Click(object sender, RoutedEventArgs e)
        {
            StackPanelMainOppo.Visibility = Visibility.Visible;
            StackPanelMain.Visibility = Visibility.Hidden;
            graveTitle.Background = new SolidColorBrush(Colors.Transparent);
            graveTitle.Foreground = new SolidColorBrush(Colors.White);
            oppoTitle.Background = new SolidColorBrush(Colors.LightGray);
            oppoTitle.Foreground = new SolidColorBrush(Colors.Black);
        }
    }
}