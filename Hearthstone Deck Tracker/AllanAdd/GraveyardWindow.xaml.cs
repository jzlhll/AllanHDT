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
                Left = Config.Instance.PlayerWindowLeft.Value + 300;
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
            StackPanelMain.Children.Add(graveTitle);
            StackPanelMain.Children.Add(ViewBoxGraveyard);
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

        public void UpdateGraveyardCards(IEnumerable<Entity> cards, bool reset) => updateListv(cards, reset);

        private List<Card> mGraveyardList = new List<Card>();

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

        private void printOppoOrgGrave() {

            IEnumerable<Entity> oppoGraveOrgList = Core.Game.Opponent.Graveyard;
            int oppoSize = oppoGraveOrgList.Count();

            Utility.Logging.Log.Info("AllanLog:orgSzie " + oppoSize);
            for (int i = 0; i < oppoSize; i++)
            {
                Card newGrave = Database.GetCardFromId(oppoGraveOrgList.ElementAt(i).CardId);
                //Utility.Logging.Log.Info("\nAllanLog: oppo newGrave id= " + newGrave.Id + " Name " + newGrave.LocalizedName
                //    + " entity id " + oppoGraveOrgList.ElementAt(i).Id + " CardId " + oppoGraveOrgList.ElementAt(i).CardId);
                Utility.Logging.Log.Info("AllanLog:OPPO ElementAt(" + i + ")" + oppoGraveOrgList.ElementAt(i).ToString());
                Utility.Logging.Log.Info("AllanLog:OPPO newGrave " + newGrave.LocalizedName + " isCreated " + newGrave.IsCreated);
                Utility.Logging.Log.Info("AllanLog:OPPO end");
                bool isWorked = false;
                
                if (!isWorked)
                {
                    //if (isDeckById(graveOrgList.ElementAt(i).Id))
                    if (true)
                    {
                        mGraveyardList.Add(newGrave);
                    }
                }
            }
        }

        private void updateListv(IEnumerable<Entity> graveOrgList, bool reset)
        {

            printOppoOrgGrave();
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
                    if(graveOrgList.ElementAt(i).Id <= 68 && !graveOrgList.ElementAt(i).Info.Created && !graveOrgList.ElementAt(i).Info.Stolen)
                    {
                        mGraveyardList.Add(newGrave);
                    }
                }
            }

            if (mGraveyardList.Count() > 0) {
                Utility.Extensions.CardListExtensions.ToSortedCardList(mGraveyardList);
            }
            ListViewGraveyard.Update(mGraveyardList, reset);
            if (reset) {
                myIds = null;
            }
        }

        //private void updateListv(IEnumerable<Entity> graveOrgList, bool reset)
        //{
        //    mGraveyardList = new List<Card> { };
        //    mGraveyardList.Clear();

        //    int size = graveOrgList.Count();
        //    for (int i = 0; i < size; i++)
        //    {
        //        //每一个传入的卡
        //        Entity newGrave = graveOrgList.ElementAt(i);//Database.GetCardFromId(graveOrgList.ElementAt(i).CardId);

        //        foreach (var grave in mGraveyardList)
        //        { //修改后的卡数组
        //            if (newGrave.Id.Equals(grave.Id))
        //            { //如果是同一张卡并且id相同增加count
        //                grave.Count++;
        //                //是一个新的id
        //                break;
        //            }
        //            else
        //            {
        //                Card c;
        //                if (newGrave.Id > 60)
        //                {
        //                    c = newGrave.Card;
        //                    c.IsCreated = true;
        //                    c.HighlightInHand = true;
        //                }
        //                else
        //                {
        //                    c = newGrave.Card;
        //                    c.IsCreated = false;
        //                    c.HighlightInHand = false;
        //                }
        //                mGraveyardList.Add(c);
        //                break;
        //            }
        //        }
        //    }

        //    ListViewGraveyard.Update(mGraveyardList, reset);
        //}

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

    }
}