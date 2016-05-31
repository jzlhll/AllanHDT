using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Hearthstone_Deck_Tracker.Hearthstone;
using static System.Windows.Visibility;

namespace Hearthstone_Deck_Tracker.Controls
{
	public partial class DeckSetIcons : UserControl
	{
		public DeckSetIcons()
		{
			InitializeComponent();
		}

		public Brush Fill
		{
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}

		public static readonly DependencyProperty FillProperty =
			DependencyProperty.Register("Fill", typeof(Brush), typeof(DeckSetIcons),
				new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

		public void Update(Deck deck)
		{
			RectIconOg.Visibility = deck?.ContainsSet(Core.WHISPERS_OF_THE_OLD_GODS) ?? false ? Visible : Collapsed;
			RectIconLoe.Visibility = deck?.ContainsSet(Core.LEAGUE_OF_EXPLORERS) ?? false ? Visible : Collapsed;
			RectIconTgt.Visibility = deck?.ContainsSet(Core.THE_GRAND_TOURNAMENT) ?? false ? Visible : Collapsed;
			RectIconBrm.Visibility = deck?.ContainsSet(Core.BLACKROCK_MOUNTAIN) ?? false ? Visible : Collapsed;
			RectIconGvg.Visibility = deck?.ContainsSet(Core.GOBLINS_VS_GNOMES) ?? false ? Visible : Collapsed;
			RectIconNaxx.Visibility = deck?.ContainsSet(Core.CURSE_OF_NAXXRAMAS) ?? false ? Visible : Collapsed;
		}
	}
}