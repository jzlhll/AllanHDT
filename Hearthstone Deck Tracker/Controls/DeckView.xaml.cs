using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HearthDb.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Utility.Extensions;
using static HearthDb.CardIds.Collectible;
using static System.Windows.Visibility;

namespace Hearthstone_Deck_Tracker.Controls
{
	public partial class DeckView
	{
		private readonly string _allTags;

		public DeckView(Deck deck, bool deckOnly = false)
		{
			InitializeComponent();
			_allTags = deck.TagList.ToLowerInvariant().Replace("-", "");
			ListViewPlayer.Update(deck.Cards.ToSortedCardList(), true);

			if(deckOnly)
			{
				DeckTitleContainer.Visibility = Collapsed;
				DeckFormatPanel.Visibility = Collapsed;
				SetDustPanel.Visibility = Collapsed;
			}
			else
			{
                DeckTitlePanel.Background = DeckHeaderBackground(deck.Class);
				LblDeckTitle.Text = deck.Name;
                LblDeckTag.Text = GetTagText(deck);
                LblDeckFormat.Text = GetFormatText(deck);
				LblDustCost.Text = TotalDust(deck).ToString();
				ShowFormatIcon(deck);
				SetIcons.Update(deck);
			}

            BrandContainer.Visibility = Hidden;
        }

        private ImageBrush DeckHeaderBackground(string deckClass)
		{
			var heroId = ClassToID(deckClass);
			var drawingGroup = new DrawingGroup();
			drawingGroup.Children.Add(new ImageDrawing(new BitmapImage(new Uri(
				$"Images/Bars/{heroId}.png", UriKind.Relative)), new Rect(54, 0, 130, 100)));
			//drawingGroup.Children.Add(new ImageDrawing(new BitmapImage(new Uri(
			//	"Images/Themes/Bars/dark/fade.png", UriKind.Relative)), new Rect(0, 0, 183, 34)));
			return new ImageBrush {
				ImageSource = new DrawingImage(drawingGroup),
				AlignmentX = AlignmentX.Left,
				Stretch = Stretch.Fill
            };
		}

        private string GetTagText(Deck deck)
		{
			var predefined = new List<string>() {
				"中速",
                "动物园",
				"快攻",
				"控制",
				"节奏",
				"组合",
                "打脸",
                "疲劳",
                "乱斗"
            };
            string ret = "";
            if (deck.Tags.Count > 0)
                foreach (var tag in predefined)
                    if (_allTags.Contains(tag)) // tag.ToLowerInvariant()
                        ret = tag;
            return ret + translateClass2CN(deck.Class);
		}
        private string translateClass2CN(string s)
        {
            if (s.Equals("Hunter"))
            {
                return "猎";
            }
            else if (s.Equals("Paladin"))
            {
                return "骑";
            }
            else if (s.Equals("Priest"))
            {
                return "牧";
            }
            else if (s.Equals("Warrior"))
            {
                return "战";
            }
            else if (s.Equals("Warlock"))
            {
                return "术";
            }
            else if (s.Equals("Druid"))
            {
                return "德";
            }
            else if (s.Equals("Mage"))
            {
                return "法";
            }
            else if (s.Equals("Shaman"))
            {
                return "萨";
            }
            else if (s.Equals("Rogue"))
            {
                return "贼";
            }
            return s;
        }
        private string GetFormatText(Deck deck)
		{
            if (deck.IsArenaDeck)
                return "竞技场";// "Arena";
            if (_allTags.Contains("brawl"))
                return "乱斗";// "Brawl";
            if (_allTags.Contains("adventure") || _allTags.Contains("pve"))
                return "冒险";//"Adventure";
            if (deck.StandardViable)
                return "标准";// "Standard";
            return "狂野";// "Wild";
		}

		private void ShowFormatIcon(Deck deck)
		{
			RectIconStandard.Visibility = Collapsed;
			RectIconWild.Visibility = Collapsed;
			RectIconArena.Visibility = Collapsed;
			RectIconBrawl.Visibility = Collapsed;
			RectIconAdventure.Visibility = Collapsed;

			if(deck.IsArenaDeck)
				RectIconArena.Visibility = Visible;
			else if(_allTags.Contains("乱斗"))
				RectIconBrawl.Visibility = Visible;
			else if(_allTags.Contains("冒险") || _allTags.Contains("pve"))
				RectIconAdventure.Visibility = Visible;
			else if(deck.StandardViable)
				RectIconStandard.Visibility = Visible;
			else
				RectIconWild.Visibility = Visible;
		}

		private int TotalDust(Deck deck)
		{
			var nonCraftableSets = new[]
			{
				CardSet.KARA,
				CardSet.NAXX,
				CardSet.BRM,
				CardSet.LOE,
				CardSet.CORE
			}.Select(HearthDbConverter.SetConverter).ToList();
			var nonCraftableCards = new List<string>() {
				Neutral.Cthun,
				Neutral.BeckonerOfEvil
			};

			return deck.Cards
				.Where(c => !nonCraftableSets.Contains(c.Set) && !nonCraftableCards.Contains(c.Id))
				.Sum(c => c.DustCost * c.Count);
		}

		private string ClassToID(string klass)
		{
			switch(klass.ToLowerInvariant())
			{
				case "druid":
					return Druid.MalfurionStormrage;
				case "hunter":
					return Hunter.Rexxar;
				case "mage":
					return Mage.JainaProudmoore;
				case "paladin":
					return Paladin.UtherLightbringer;
				case "priest":
					return Priest.AnduinWrynn;
				case "rogue":
					return Rogue.ValeeraSanguinar;
				case "shaman":
					return Shaman.Thrall;
				case "warlock":
					return Warlock.Guldan;
				case "warrior":
				default:
					return Warrior.GarroshHellscream;
			}
		}
	}
}