﻿#region

using System.Linq;
using HearthDb;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone.Entities;
using static HearthDb.Enums.GameTag;

#endregion

namespace Hearthstone_Deck_Tracker.Utility
{
	public static class WotogCounterHelper
	{
		public static Entity PlayerCthun => Core.Game.Player.PlayerEntities.FirstOrDefault(x => x.CardId == CardIds.Collectible.Neutral.Cthun && x.Info.OriginalZone != null);
		public static Entity PlayerCthunProxy => Core.Game.Player.PlayerEntities.FirstOrDefault(x => x.CardId == CardIds.NonCollectible.Neutral.Cthun);
		public static Entity PlayerYogg => Core.Game.Player.PlayerEntities.FirstOrDefault(x => x.CardId == CardIds.Collectible.Neutral.YoggSaronHopesEnd && x.Info.OriginalZone != null);
		public static Entity PlayerArcaneGiant => Core.Game.Player.PlayerEntities.FirstOrDefault(x => x.CardId == CardIds.Collectible.Neutral.ArcaneGiant && x.Info.OriginalZone != null);
		public static Entity OpponentCthun => Core.Game.Opponent.PlayerEntities.FirstOrDefault(x => x.CardId == CardIds.Collectible.Neutral.Cthun);
		public static Entity OpponentCthunProxy => Core.Game.Opponent.PlayerEntities.FirstOrDefault(x => x.CardId == CardIds.NonCollectible.Neutral.Cthun);
		public static bool PlayerSeenCthun => Core.Game.PlayerEntity?.HasTag(SEEN_CTHUN) ?? false;
		public static bool OpponentSeenCthun => Core.Game.OpponentEntity?.HasTag(SEEN_CTHUN) ?? false;
		public static bool? CthunInDeck => DeckContains(CardIds.Collectible.Neutral.Cthun);
		public static bool? YoggInDeck => DeckContains(CardIds.Collectible.Neutral.YoggSaronHopesEnd);
		public static bool? ArcaneGiantInDeck => DeckContains(CardIds.Collectible.Neutral.ArcaneGiant);

		public static bool ShowPlayerCthunCounter => !Core.Game.IsInMenu && (Config.Instance.PlayerCthunCounter == DisplayMode.一直
					|| Config.Instance.PlayerCthunCounter == DisplayMode.自动 && PlayerSeenCthun);

		public static bool ShowPlayerSpellsCounter => !Core.Game.IsInMenu && (
			Config.Instance.PlayerSpellsCounter == DisplayMode.一直
				|| (Config.Instance.PlayerSpellsCounter == DisplayMode.自动 && YoggInDeck.HasValue && (PlayerYogg != null || YoggInDeck.Value))
				|| (Config.Instance.PlayerSpellsCounter == DisplayMode.自动 && ArcaneGiantInDeck.HasValue && (PlayerArcaneGiant != null || ArcaneGiantInDeck.Value))
			);

		public static bool ShowOpponentCthunCounter => !Core.Game.IsInMenu && (Config.Instance.OpponentCthunCounter == DisplayMode.一直
                    || Config.Instance.OpponentCthunCounter == DisplayMode.自动 && OpponentSeenCthun);

		public static bool ShowOpponentSpellsCounter => !Core.Game.IsInMenu && Config.Instance.OpponentSpellsCounter == DisplayMode.一直;

		private static bool? DeckContains(string cardId) => DeckList.Instance.ActiveDeck?.Cards.Any(x => x.Id == cardId);
	}
}