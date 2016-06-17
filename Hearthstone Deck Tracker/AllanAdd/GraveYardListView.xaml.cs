using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Hearthstone_Deck_Tracker.AllanAdd
{
    /// <summary>
    /// GraveYardListView.xaml 的交互逻辑
    /// </summary>
    public partial class GraveYardListView : Window
    {
        public GraveYardListView()
        {
            InitializeComponent();
        }

        public void UpdateDeckList(Hearthstone.Deck selected)
        {
            ListViewDeck.ItemsSource = null;
            if (selected == null)
                return;
            ListViewDeck.ItemsSource = selected.GetSelectedDeckVersion().Cards;
            Helper.SortCardCollection(ListViewDeck.Items, Config.Instance.CardSortingClassFirst);
        }
    }
}
