using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;

namespace Hearthstone_Deck_Tracker.Windows
{
	public partial class OverlayWindow
	{
		private const string DeckPanelCards = "卡牌";
		private const string DeckPanelDrawChances = "抽牌几率";
		private const string DeckPanelCardCounter = "卡牌计数器";
		private const string DeckPanelFatigueCounter = "疲劳计数器";
		private const string DeckPanelDeckTitle = "卡组名字";
		private const string DeckPanelWins = "胜场";
		private const string DeckPanelWinrate = "胜率";

		public void UpdatePlayerLayout()
		{
			StackPanelPlayer.Children.Clear();
			foreach(var item in Config.Instance.PanelOrderPlayer)
			{
				switch(item)
				{
					case DeckPanelDrawChances:
						StackPanelPlayer.Children.Add(CanvasPlayerChance);
						break;
					case DeckPanelCardCounter:
						StackPanelPlayer.Children.Add(CanvasPlayerCount);
						break;
					case DeckPanelFatigueCounter:
						StackPanelPlayer.Children.Add(LblPlayerFatigue);
						break;
					case DeckPanelDeckTitle:
						StackPanelPlayer.Children.Add(LblDeckTitle);
						break;
					case DeckPanelWins:
						StackPanelPlayer.Children.Add(LblWins);
						break;
					case DeckPanelCards:
						StackPanelPlayer.Children.Add(ViewBoxPlayer);
						break;
				}
			}
		}

		public void UpdateOpponentLayout()
		{
			StackPanelOpponent.Children.Clear();
			foreach (var item in Config.Instance.PanelOrderOpponent)
			{
				switch (item)
				{
					case DeckPanelDrawChances:
						StackPanelOpponent.Children.Add(CanvasOpponentChance);
						break;
					case DeckPanelCardCounter:
						StackPanelOpponent.Children.Add(CanvasOpponentCount);
						break;
					case DeckPanelFatigueCounter:
						StackPanelOpponent.Children.Add(LblOpponentFatigue);
						break;
					case DeckPanelWinrate:
						StackPanelOpponent.Children.Add(LblWinRateAgainst);
						break;
					case DeckPanelCards:
						StackPanelOpponent.Children.Add(ViewBoxOpponent);
						break;
				}
			}
		}

		private void SetWinRates()
		{
			var selectedDeck = DeckList.Instance.ActiveDeck;
			if (selectedDeck == null)
				return;

			LblWins.Text = $"{selectedDeck.WinLossString} ({selectedDeck.WinPercentString})";

			if (!string.IsNullOrEmpty(_game.Opponent.Class))
			{
				var winsVs = selectedDeck.GetRelevantGames().Count(g => g.Result == GameResult.Win && g.OpponentHero == _game.Opponent.Class);
				var lossesVs = selectedDeck.GetRelevantGames().Count(g => g.Result == GameResult.Loss && g.OpponentHero == _game.Opponent.Class);
				var percent = (winsVs + lossesVs) > 0 ? Math.Round(winsVs * 100.0 / (winsVs + lossesVs), 0).ToString() : "-";
				LblWinRateAgainst.Text = $"VS {translateClass2CN(_game.Opponent.Class)}: {winsVs}-{lossesVs} ({percent}%)";
			}
		}

        private string translateClass2CN(string s)
        {
            s = s.ToLowerInvariant();
            if (s.Equals("hunter"))
            {
                return "猎人";
            }
            else if (s.Equals("paladin"))
            {
                return "圣骑士";
            }
            else if (s.Equals("priest"))
            {
                return "牧师";
            }
            else if (s.Equals("warrior"))
            {
                return "战士";
            }
            else if (s.Equals("warlock"))
            {
                return "术士";
            }
            else if (s.Equals("druid"))
            {
                return "德鲁伊";
            }
            else if (s.Equals("mage"))
            {
                return "法师";
            }
            else if (s.Equals("shaman"))
            {
                return "萨满";
            }
            else if (s.Equals("rogue"))
            {
                return "潜行者";
            }
            return s;
        }

        private void SetDeckTitle() => LblDeckTitle.Text = DeckList.Instance.ActiveDeck?.Name ?? "";

		private void SetOpponentCardCount(int cardCount, int cardsLeftInDeck)
		{
			LblOpponentCardCount.Text = cardCount.ToString();
			LblOpponentDeckCount.Text = cardsLeftInDeck.ToString();

			if (cardsLeftInDeck <= 0)
			{
				LblOpponentFatigue.Text = "下一抽疲劳: " + (_game.Opponent.Fatigue + 1);

				LblOpponentDrawChance2.Text = "0%";
				LblOpponentDrawChance1.Text = "0%";
				LblOpponentHandChance2.Text = cardCount <= 0 ? "0%" : "100%";
				;
				LblOpponentHandChance1.Text = cardCount <= 0 ? "0%" : "100%";
				return;
			}
			LblOpponentFatigue.Text = "";

			var handWithoutCoin = cardCount - (_game.Opponent.HasCoin ? 1 : 0);

			var holdingNextTurn2 = Math.Round(100.0f * Helper.DrawProbability(2, (cardsLeftInDeck + handWithoutCoin), handWithoutCoin + 1), 1);
			var drawNextTurn2 = Math.Round(200.0f / cardsLeftInDeck, 1);
			LblOpponentDrawChance2.Text = (cardsLeftInDeck == 1 ? 100 : drawNextTurn2) + "%";
			LblOpponentHandChance2.Text = holdingNextTurn2 + "%";

			var holdingNextTurn = Math.Round(100.0f * Helper.DrawProbability(1, (cardsLeftInDeck + handWithoutCoin), handWithoutCoin + 1), 1);
			var drawNextTurn = Math.Round(100.0f / cardsLeftInDeck, 1);
			LblOpponentDrawChance1.Text = drawNextTurn + "%";
			LblOpponentHandChance1.Text = holdingNextTurn + "%";
		}

		private void SetCardCount(int cardCount, int cardsLeftInDeck)
		{
			LblCardCount.Text = cardCount.ToString();
			LblDeckCount.Text = cardsLeftInDeck.ToString();

			if (cardsLeftInDeck <= 0)
			{
				LblPlayerFatigue.Text = "下一抽疲劳: " + (_game.Player.Fatigue + 1);

				LblDrawChance2.Text = "0%";
				LblDrawChance1.Text = "0%";
				return;
			}
			LblPlayerFatigue.Text = "";

			var drawNextTurn2 = Math.Round(200.0f / cardsLeftInDeck, 1);
			LblDrawChance2.Text = (cardsLeftInDeck == 1 ? 100 : drawNextTurn2) + "%";
			LblDrawChance1.Text = Math.Round(100.0f / cardsLeftInDeck, 1) + "%";
		}

		public void UpdatePlayerCards(List<Card> cards, bool reset) => ListViewPlayer.Update(cards, reset);

		public void UpdateOpponentCards(List<Card> cards, bool reset) => ListViewOpponent.Update(cards, reset);
	}
}
