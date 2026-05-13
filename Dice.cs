using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace Yahtzee
{
    class Dice
    {
        public BitmapImage[] SideImage { get; set; } = new BitmapImage[7];
        public int Side { get; set; } = 1;
        public bool IsToRoll { get; set; } = true;

        public Dice()
        {
            SideImage[1] = new BitmapImage(new Uri("pack://application:,,,/images/one.png", UriKind.Absolute));
            SideImage[2] = new BitmapImage(new Uri("pack://application:,,,/images/two.png", UriKind.Absolute));
            SideImage[3] = new BitmapImage(new Uri("pack://application:,,,/images/three.png", UriKind.Absolute));
            SideImage[4] = new BitmapImage(new Uri("pack://application:,,,/images/four.png", UriKind.Absolute));
            SideImage[5] = new BitmapImage(new Uri("pack://application:,,,/images/five.png", UriKind.Absolute));
            SideImage[6] = new BitmapImage(new Uri("pack://application:,,,/images/six.png", UriKind.Absolute));
        }
    }
}
