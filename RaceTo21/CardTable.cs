using System;
using System.Collections.Generic;
using System.Windows;

namespace RaceTo21
{
    public class CardTable
    {
        public void AnnounceWinner(Player player, List<Player> players)
        {
            if (player != null)
            {
                MessageBoxResult result = MessageBox.Show(player.GetName().ToString()+" wins!");
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Everyone busted!");
            }
        }
    }
}