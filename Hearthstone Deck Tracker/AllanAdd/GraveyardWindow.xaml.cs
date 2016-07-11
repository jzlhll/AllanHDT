﻿#region

using System;
using Hearthstone_Deck_Tracker.Enums.Hearthstone;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Hearthstone_Deck_Tracker.Annotations;
using Hearthstone_Deck_Tracker.Hearthstone;
using Point = System.Drawing.Point;
using Hearthstone_Deck_Tracker.Hearthstone.Entities;
using System.Windows.Media;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Enums;
using System.Threading.Tasks;
using Hearthstone_Deck_Tracker.Utility.Logging;
using Hearthstone_Deck_Tracker.AllanAdd;
using System.Threading;
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
        private GuessDeckWorker mGuessWorker;

        public GraveyardWindow(GameV2 game, List<Card> forScreenshot = null)
        {
            InitializeComponent();
            Left = 800;
            Height = 500;
            _game = game;
            try {
                if (Config.Instance.GraveYardWindowLocation != null)
                {
                    string[] ss = Config.Instance.GraveYardWindowLocation.Split(':');
                    Left = int.Parse(ss[0]);
                    Height = int.Parse(ss[1]);
                    ss = null;
                }
            }
            catch(Exception e)
            {
                //do nothing
            }
            Topmost = Config.Instance.WindowsTopmost;

            var titleBarCorners = new[]
            {
                new Point((int)Left + 5, (int)Top + 5),
                new Point((int)(Left + Width) - 5, (int)Top + 5),
                new Point((int)Left + 5, (int)(Top + TitlebarHeight) - 5),
                new Point((int)(Left + Width) - 5, (int)(Top + TitlebarHeight) - 5)
            };

            if (Core.Game.CurrentMode == Mode.GAMEPLAY)
            {
                updatePlayerGraveListv(false);
                //todo UpdateOppoDeckCards(false);
            }
            //graveTitle.Background = new SolidColorBrush(Colors.LightGray);
            //graveTitle.Foreground = new SolidColorBrush(Colors.Black);
            //oppoTitle.Background = new SolidColorBrush(Colors.Transparent);
            Log.Debug("beginnnnn");
            generateGuessOpponentDecks();
            Log.Debug("overrrrrr");
        }

        private void getDeck() {
            mGuessWorker = new GuessDeckWorker();
        }

        private void generateGuessOpponentDecks() {
            Thread mythread = new Thread(getDeck);
            mythread.Start();
        }

        public double PlayerDeckMaxHeight => ActualHeight - PlayerLabelsHeight;

        public double PlayerLabelsHeight => 42;//graveTitle.ActualHeight + 42;

        public bool ShowToolTip => Config.Instance.WindowCardToolTips;

        public event PropertyChangedEventHandler PropertyChanged;

        private void UpdatePlayerLayout()
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
            mGuessWorker.release();
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

        public void UpdateOppoDeckCards(bool reset) => UpdateOppoDeckListv(reset);
        public void UpdateGraveyardCards(bool reset) => updatePlayerGraveListv(reset);

        private List<Card> mGraveyardList = new List<Card>();
       // private List<Card> mOppoDeckList = new List<Card>();

        private int[] myIds;

        private void generateMYIds() {
            if (_game.Entities.Values.Count() == 0)
            {
                myIds = null;
                return;
            }
            myIds = new int[30];
            int i = 0;
            
            foreach (var card in _game.Entities.Values) {
                myIds[i] = card.Id;
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

        private async Task<List<Card>> updateOppoDeckInternal() {
            List<Card> oppoCards = new List<Card>(Core.Game.Opponent.OpponentCardList);
            if (Core.Game.CurrentMode != Mode.GAMEPLAY)
            {
                return null;
            }
            int oppoSize = oppoCards.Count();
            for (int i = 0; i < oppoCards.Count(); i++)
            {
                if (oppoCards.ElementAt(i).IsCreated)
                {
                    oppoCards.RemoveAt(i);
                    i--;
                }
            }
            await Task.Delay(10);
            return oppoCards;
        }

        private async void UpdateOppoDeckListv(bool reset)
        {
            List<Card> cards = await updateOppoDeckInternal(); //TODO 不用每次都操作吧？需要做一个重复就不搞的操作。
            if (cards == null || cards.Count == 0) {
                return;
            }
            Log.Info("UpdateOppoDeckListv started!");
            foreach (var s in cards) {
                Log.Info("oppoCard= " + s + " " + s.Count);
            }
            //if(cards != null) ListViewOppoDeck.Update(cards, reset);
            //开始进行匹配
            int len = mGuessWorker.mDeckList.Count;
            int cl = cards.Count;
            Dictionary<int ,int> dictionary = new Dictionary < int,int>();//创建集合
            int max = 0;
            for (int i = 0; i < len;i++) {
                var adeck = mGuessWorker.mDeckList.ElementAt(i);
                if (adeck.heroId != GuessDeckWorker.convertClassToHeroId(_game.Opponent.Class)) {
                    continue;
                }
                Log.Info("deck---" + adeck.convertedDeck + "\r\ndeck over---");
                int countMatch = 0;
                for (int j = 0; j < cl; j++)
                {
                    if (adeck.convertedDeck.Contains(cards.ElementAt(j).Name + "\r\n")) //加上换行免得比配到半名字
                    {
                        countMatch++;
                        if (cards.ElementAt(j).Count > 1 && adeck.convertedDeck.Contains(cards.ElementAt(j).Name + " x 2")) {
                            //TODO 测试下是否生成和不生成混合在了一起
                            countMatch++;
                        }
                    }
                }
                dictionary.Add(i, countMatch);
                if (max < countMatch) {
                    max = countMatch;
                }
                Log.Info("matched---" + countMatch + "\r\nmatched---");
            }
            //var result2 = from pair in dictionary orderby pair.Value select pair;
            Log.Info("UpdateOppoDeckListv sorted!");
            dictionary = dictionary.OrderByDescending(r => r.Value).ToDictionary(r => r.Key, r => r.Value);
            var list = dictionary.ToList();
            foreach (var l in list) {
                Log.Info(mGuessWorker.mDeckList.ElementAt(l.Key).convertedDeck + " matched " + l.Value);
            }
            Log.Info("UpdateOppoDeckListv end!");
            //end
            if (reset)
            {
                myIds = null;
                if(cards != null) cards.Clear();
            }
        }

         

        private void updatePlayerGraveInternal() {
            IEnumerable<Entity> graveOrgList = Core.Game.Player.Graveyard;
            mGraveyardList.Clear();

            int orgSize = graveOrgList.Count();

            for (int i = 0; i < orgSize; i++)
            {
                //每一个传入的卡
                Card newGrave = Database.GetCardFromId(graveOrgList.ElementAt(i).CardId);

                bool isWorked = false;
                foreach (var grave in mGraveyardList)
                { //修改后的卡数组
                    if ((newGrave.Id.Equals(grave.Id) && (graveOrgList.ElementAt(i).Id <= 68 || Config.Instance.GraveYardWindowIfCreated)))
                    { //如果是同一张卡并且id相同增加count
                        isWorked = true;
                        grave.Count++;
                        //是一个新的id
                        break;
                    }
                }
                if (!isWorked)
                {
                    if (graveOrgList.ElementAt(i).Id <= 68 && !graveOrgList.ElementAt(i).Info.Created && !graveOrgList.ElementAt(i).Info.Stolen)
                    {
                        mGraveyardList.Add(newGrave);
                    }
                    else if (Config.Instance.GraveYardWindowIfCreated && isCardIdEndOfNumber(graveOrgList.ElementAt(i).CardId))
                    { //创建出来的卡,还要过滤掉e,t等结尾
                        newGrave.HighlightInHand = true;
                        mGraveyardList.Add(newGrave);
                    }
                }
            }

            if (mGraveyardList.Count() > 0)
            {
                mGraveyardList = Utility.Extensions.CardListExtensions.ToSortedCardList(mGraveyardList);
            }
        }

        private void updatePlayerGraveListv(bool reset)
        {
            if (Core.Game.CurrentGameMode == GameMode.Spectator)
            {
                return;
            }

            if (reset)
            {
                myIds = null;
                //mOppoDeckList.Clear();
                mGraveyardList.Clear();
            }
            else {
                updatePlayerGraveInternal();
            }

            if (mGraveyardList != null) ListViewGraveyard.Update(mGraveyardList, reset);
        }

        private bool isCardIdEndOfNumber(string cardId)
        {
            char ch = cardId.ElementAt(cardId.Count() - 1);

            if (ch >= '0' && ch <= '9')
            {
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
            ListViewGraveyard.Visibility = Visibility.Visible;
          //  graveTitle.Visibility = Visibility.Visible;
            UpdatePlayerLayout();
        }

        private void graveTitle_Click(object sender, RoutedEventArgs e)
        {
            StackPanelMain.Visibility = Visibility.Visible;
            StackPanelMainOppo.Visibility = Visibility.Hidden;
            //graveTitle.Background = new SolidColorBrush(Colors.LightGray);
           // graveTitle.Foreground = new SolidColorBrush(Colors.Black);
           // oppoTitle.Background = new SolidColorBrush(Colors.Transparent);
           // oppoTitle.Foreground = new SolidColorBrush(Colors.White);
        }

        //private void oppoTitle_Click(object sender, RoutedEventArgs e)
        //{
        //    if (true) { //todo 
        //        return;
        //    }
        //    StackPanelMainOppo.Visibility = Visibility.Visible;
        //    StackPanelMain.Visibility = Visibility.Hidden;
        //   // graveTitle.Background = new SolidColorBrush(Colors.Transparent);
        //  //  graveTitle.Foreground = new SolidColorBrush(Colors.White);
        //  //  oppoTitle.Background = new SolidColorBrush(Colors.LightGray);
        //  //  oppoTitle.Foreground = new SolidColorBrush(Colors.Black);
        //}

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            Config.Instance.GraveYardWindowLocation = "" + Left + ":" + Height;
            Config.Save();
            mGraveyardList.Clear();
            mGraveyardList = null;
            myIds = null;
        }
    }
}