using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Yahtzee
{
    /// <summary>
    /// Interaction logic for StatsWindow.xaml
    /// </summary>
    public partial class StatsWindow : Window
    {
        public StatsWindow()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Redraw();
        }

        private void Redraw()
        {
            Clear();

            // load stats from Properties.Settings.Default
            int numberOfGames = Properties.Settings.Default.NumberOfGames;
            double averageScore = Properties.Settings.Default.AverageScore;
            int lastScore = Properties.Settings.Default.LastScore;
            DateTime lastDate = Properties.Settings.Default.LastDate;
            int lowScore = Properties.Settings.Default.LowScore;
            DateTime lowDate = Properties.Settings.Default.LowDate;
            int[] highScore = new int[10];
            DateTime[] highDate = new DateTime[10];
            highScore[0] = Properties.Settings.Default.HighScore1;
            highDate[0] = Properties.Settings.Default.HighDate1;
            highScore[1] = Properties.Settings.Default.HighScore2;
            highDate[1] = Properties.Settings.Default.HighDate2;
            highScore[2] = Properties.Settings.Default.HighScore3;
            highDate[2] = Properties.Settings.Default.HighDate3;
            highScore[3] = Properties.Settings.Default.HighScore4;
            highDate[3] = Properties.Settings.Default.HighDate4;
            highScore[4] = Properties.Settings.Default.HighScore5;
            highDate[4] = Properties.Settings.Default.HighDate5;
            highScore[5] = Properties.Settings.Default.HighScore6;
            highDate[5] = Properties.Settings.Default.HighDate6;
            highScore[6] = Properties.Settings.Default.HighScore7;
            highDate[6] = Properties.Settings.Default.HighDate7;
            highScore[7] = Properties.Settings.Default.HighScore8;
            highDate[7] = Properties.Settings.Default.HighDate8;
            highScore[8] = Properties.Settings.Default.HighScore9;
            highDate[8] = Properties.Settings.Default.HighDate9;
            highScore[9] = Properties.Settings.Default.HighScore10;
            highDate[9] = Properties.Settings.Default.HighDate10;

            TextBlock tb = new TextBlock();
            tb.Text = numberOfGames.ToString();
            tb.FontSize = 16;
            tb.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(tb, 0);
            Grid.SetColumn(tb, 1);
            OuterGrid.Children.Add(tb);

            tb = new TextBlock();
            if (averageScore == 0)
                tb.Text = "—";
            else
                tb.Text = averageScore.ToString();
            tb.FontSize = 16;
            tb.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(tb, 1);
            Grid.SetColumn(tb, 1);
            OuterGrid.Children.Add(tb);

            tb = new TextBlock();
            if (lastScore == 0)
                tb.Text = "—";
            else
                tb.Text = lastScore.ToString();
            tb.FontSize = 16;
            tb.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(tb, 2);
            Grid.SetColumn(tb, 1);
            OuterGrid.Children.Add(tb);

            tb = new TextBlock();
            if (lastScore == 0)
                tb.Text = "—";
            else
                tb.Text = lastDate.ToString();
            tb.FontSize = 16;
            tb.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(tb, 3);
            Grid.SetColumn(tb, 1);
            OuterGrid.Children.Add(tb);

            tb = new TextBlock();
            if (lowScore == 0)
                tb.Text = "—";
            else
                tb.Text = lowScore.ToString();
            tb.FontSize = 16;
            tb.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(tb, 4);
            Grid.SetColumn(tb, 1);
            OuterGrid.Children.Add(tb);

            tb = new TextBlock();
            if (lowScore == 0)
                tb.Text = "—";
            else
                tb.Text = lowDate.ToString();
            tb.FontSize = 16;
            tb.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetRow(tb, 5);
            Grid.SetColumn(tb, 1);
            OuterGrid.Children.Add(tb);

            for (int i=0; i<10; i++)
            {
                tb = new TextBlock();
                tb.Text = (i+1).ToString();
                tb.FontSize = 16;
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(tb, i+2);
                Grid.SetColumn(tb, 0);
                InnerGrid.Children.Add(tb);

                tb = new TextBlock();
                if (highScore[i] == 0)
                    tb.Text = "—";
                else
                    tb.Text = highScore[i].ToString();
                tb.FontSize = 16;
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(tb, i+2);
                Grid.SetColumn(tb, 1);
                InnerGrid.Children.Add(tb);

                tb = new TextBlock();
                if (highScore[i] == 0)
                    tb.Text = "—";
                else
                    tb.Text = highDate[i].ToString();
                tb.FontSize = 16;
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(tb, i+2);
                Grid.SetColumn(tb, 2);
                InnerGrid.Children.Add(tb);
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();
            Redraw();
        }

        private void Clear()
        {
            var toRemove1 = OuterGrid.Children
                .OfType<UIElement>()
                .Where(e => Grid.GetColumn(e) == 1 && Grid.GetRow(e) < 6)  // or Grid.GetColumn(e) == 3
                .ToList();

            foreach (var element in toRemove1)
                OuterGrid.Children.Remove(element);

            var toRemove2 = InnerGrid.Children
                .OfType<UIElement>()
                .Where(e => Grid.GetRow(e) >= 2)  // or Grid.GetColumn(e) == 3
                .ToList();

            foreach (var element in toRemove2)
                InnerGrid.Children.Remove(element);
        }
    }
}
