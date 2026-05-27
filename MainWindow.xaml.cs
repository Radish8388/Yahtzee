using System.Reflection.Emit;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Yahtzee
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random _random = new Random();
        Dice[] _dice = new Dice[5];
        Image[] _diceImage = new Image[5];
        double _diceSize = 50;
        double _rollRowX, _rollRowY;
        double _keepRowX, _keepRowY;
        bool _diceLoaded = false;
        int _rollCount = 0;
        int _turnCount = 0;
        TextBlock _rollLabel = new TextBlock();
        TextBlock _keepLabel = new TextBlock();
        TextBlock _countLabel = new TextBlock();
        int[] _scores = new int[21];
        bool[] _used = new bool[21];
        DateTime _gameCompletion;
        int _yahtzeeCount = 0;

        public MainWindow()
        {
            InitializeComponent();
        }
        #region window and button events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // load the properties from disk
            Properties.Settings.Default.Reload();
            this.Left = Properties.Settings.Default.WindowLeft;
            this.Top = Properties.Settings.Default.WindowTop;
            this.Width = Properties.Settings.Default.WindowWidth;
            this.Height = Properties.Settings.Default.WindowHeight;

            double screenWidth = SystemParameters.WorkArea.Width;
            double screenHeight = SystemParameters.WorkArea.Height;

            // ensure window size doesn't exceed screen size
            if (this.Width > screenWidth) this.Width = screenWidth;
            if (this.Height > screenHeight) this.Height = screenHeight;

            // ensure window is not off the left or top
            if (this.Left < 0) this.Left = 0;
            if (this.Top < 0) this.Top = 0;

            // ensure window is not off the right or bottom
            if (this.Left + this.Width > screenWidth)
                this.Left = screenWidth - this.Width;
            if (this.Top + this.Height > screenHeight)
                this.Top = screenHeight - this.Height;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.WindowLeft = this.Left;
            Properties.Settings.Default.WindowTop = this.Top;
            Properties.Settings.Default.WindowWidth = this.Width;
            Properties.Settings.Default.WindowHeight = this.Height;
            Properties.Settings.Default.Save();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            LoadDice();
            NewGame();
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            NewGame();
        }

        private void StatsButton_Click(object sender, RoutedEventArgs e)
        {
            StatsWindow dialog = new StatsWindow();
            dialog.Owner = this;
            bool? result = dialog.ShowDialog(); // Modal dialog
        }

        private void RollDiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (_rollCount >= 3 || _turnCount >= 13) return;
            for (int i = 0; i < _dice.Length; i++)
            {
                if (_dice[i].IsToRoll)
                    _dice[i].Side = _random.Next(1, 7);
            }
            _rollCount++;
            _countLabel.Text = $"Rolls Remaining: {3-_rollCount}";
            Redraw();
            ShowToolTips();
            if (IsYahtzee())
                MessageBox.Show("YAHTZEE!!!", "", MessageBoxButton.OK, MessageBoxImage.None);
            if (_rollCount == 3)
                Instructions.Text = "Click a yellow box to enter your score.";
            else
                Instructions.Text = "Either 1) click a yellow box to enter your score, or 2) move the dice and then roll again.";
        }
        #endregion
        #region scorecard events
        private void Aces_Score_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_used[1] && _rollCount > 0)
            {
                int score = 0;
                for (int i = 0; i < _dice.Length; i++)
                    if (_dice[i].Side == 1)
                        score += 1;
                _scores[1] = score;
                _used[1] = true;
                EndTurn();
            }
        }

        private void Twos_Score_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_used[2] && _rollCount > 0)
            {
                int score = 0;
                for (int i = 0; i < _dice.Length; i++)
                    if (_dice[i].Side == 2)
                        score += 2;
                _scores[2] = score;
                _used[2] = true;
                EndTurn();
            }
        }

        private void Threes_Score_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_used[3] && _rollCount > 0)
            {
                int score = 0;
                for (int i = 0; i < _dice.Length; i++)
                    if (_dice[i].Side == 3)
                        score += 3;
                _scores[3] = score;
                _used[3] = true;
                EndTurn();
            }
        }

        private void Fours_Score_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_used[4] && _rollCount > 0)
            {
                int score = 0;
                for (int i = 0; i < _dice.Length; i++)
                    if (_dice[i].Side == 4)
                        score += 4;
                _scores[4] = score;
                _used[4] = true;
                EndTurn();
            }
        }

        private void Fives_Score_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_used[5] && _rollCount > 0)
            {
                int score = 0;
                for (int i = 0; i < _dice.Length; i++)
                    if (_dice[i].Side == 5)
                        score += 5;
                _scores[5] = score;
                _used[5] = true;
                EndTurn();
            }
        }

        private void Sixes_Score_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_used[6] && _rollCount > 0)
            {
                int score = 0;
                for (int i = 0; i < _dice.Length; i++)
                    if (_dice[i].Side == 6)
                        score += 6;
                _scores[6] = score;
                _used[6] = true;
                EndTurn();
            }
        }

        private void ThreeOfAKind_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_used[10] && _rollCount > 0)
            {
                int score = 0;

                // make a list of the dice and sort it
                List<int> dice = new List<int>();
                for (int i = 0; i < _dice.Length; i++)
                    dice.Add(_dice[i].Side);
                dice.Sort();

                // look for three of a kind
                if ((dice[0] == dice[1] && dice[1] == dice[2]) ||
                    (dice[1] == dice[2] && dice[2] == dice[3]) ||
                    (dice[2] == dice[3] && dice[3] == dice[4]))
                    score = dice[0] + dice[1] + dice[2] + dice[3] + dice[4];

                _scores[10] = score;
                _used[10] = true;
                EndTurn();
            }
        }

        private void FourOfAKind_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_used[11] && _rollCount > 0)
            {
                int score = 0;

                // make a list of the dice and sort it
                List<int> dice = new List<int>();
                for (int i = 0; i < _dice.Length; i++)
                    dice.Add(_dice[i].Side);
                dice.Sort();

                // look for four of a kind
                if ((dice[0] == dice[1] && dice[1] == dice[2] && dice[2] == dice[3]) ||
                    (dice[1] == dice[2] && dice[2] == dice[3] && dice[3] == dice[4]))
                    score = dice[0] + dice[1] + dice[2] + dice[3] + dice[4];

                _scores[11] = score;
                _used[11] = true;
                EndTurn();
            }
        }

        private void FullHouse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_used[12] && _rollCount > 0)
            {
                int score = 0;

                // make a list of the dice and sort it
                List<int> dice = new List<int>();
                for (int i = 0; i < _dice.Length; i++)
                    dice.Add(_dice[i].Side);
                dice.Sort();

                // look for full house (2 & 3 of a kind)
                if (((dice[0] == dice[1] && dice[1] == dice[2] && dice[3] == dice[4]) ||
                    (dice[0] == dice[1] && dice[2] == dice[3] && dice[3] == dice[4])) && 
                    (dice[0] != dice[4]))
                    score = 25;

                _scores[12] = score;
                _used[12] = true;
                EndTurn();
            }
        }

        private void SmStraight_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_used[13] && _rollCount > 0)
            {
                int score = 0;
                int i;

                // make a list of the dice and sort it
                List<int> dice = new List<int>();
                for (i = 0; i < _dice.Length; i++)
                    dice.Add(_dice[i].Side);
                dice.Sort();

                // remove duplicates
                i = 1;
                while (i < dice.Count)
                {
                    if (dice[i] == dice[i - 1])
                        dice.RemoveAt(i);
                    else
                        i++;
                }

                // look for sequence of four
                if (dice.Count == 4)
                    if (dice[0] + 1 == dice[1] && dice[1] + 1 == dice[2] && dice[2] + 1 == dice[3])
                        score = 30;
                if (dice.Count == 5)
                    if ((dice[0] + 1 == dice[1] && dice[1] + 1 == dice[2] && dice[2] + 1 == dice[3]) ||
                        (dice[1] + 1 == dice[2] && dice[2] + 1 == dice[3] && dice[3] + 1 == dice[4]))
                        score = 30;

                _scores[13] = score;
                _used[13] = true;
                EndTurn();
            }
        }

        private void LgStraight_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_used[14] && _rollCount > 0)
            {
                int score = 0;

                // make a list of the dice and sort it
                List<int> dice = new List<int>();
                for (int i = 0; i < _dice.Length; i++)
                    dice.Add(_dice[i].Side);
                dice.Sort();

                // look for sequence of five
                if (dice[0] + 1 == dice[1] && dice[1] + 1 == dice[2] && dice[2] + 1 == dice[3] && dice[3] + 1 == dice[4])
                    score = 40;

                _scores[14] = score;
                _used[14] = true;
                EndTurn();
            }
        }

        private void Yahtzee_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_used[15] && _rollCount > 0)
            {
                int score = 0;

                // look for five of a kind
                if (IsYahtzee())
                    score = 50;

                _scores[15] = score;
                _used[15] = true;
                EndTurn();
            }
        }

        private void Chance_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_used[16] && _rollCount > 0)
            {
                int score = 0;

                // sum all the dice
                for (int i = 0; i < _dice.Length; i++)
                    score += _dice[i].Side;

                _scores[16] = score;
                _used[16] = true;
                EndTurn();
            }
        }
        #endregion
        #region canvas events
        private void Table_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_rollCount == 0 || _rollCount >= 3 || _turnCount >= 13) return;
            Point p = e.GetPosition(Table);
            if (p.X >= _rollRowX && p.X <= _rollRowX + 5 * _diceSize && p.Y >= _rollRowY && p.Y <= _rollRowY + _diceSize)
            {
                for (int i = 0; i < _dice.Length; i++)
                {
                    if (p.X <= _rollRowX + (i + 1) * _diceSize)
                    {
                        _dice[i].IsToRoll = false;
                        break;
                    }
                }
            }
            if (p.X >= _keepRowX && p.X <= _keepRowX + 5 * _diceSize && p.Y >= _keepRowY && p.Y <= _keepRowY + _diceSize)
            {
                for (int i = 0; i < _dice.Length; i++)
                {
                    if (p.X <= _keepRowX + (i + 1) * _diceSize)
                    {
                        _dice[i].IsToRoll = true;
                        break;
                    }
                }
            }
            Redraw();
        }

        private void Table_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(Table);
        }

        private void Table_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(Table);
        }

        private void Table_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double tw = Table.ActualWidth;
            double th = Table.ActualHeight;
            _diceSize = tw / 7.0;
            _rollRowX = tw / 2.0 - 2.5 * _diceSize;
            _keepRowX = _rollRowX;
            _rollRowY = th / 2.0 - 1.5 * _diceSize;
            _keepRowY = th / 2.0 + 0.5 * _diceSize;
            Canvas.SetLeft(_rollLabel, _rollRowX);
            Canvas.SetTop(_rollLabel, _rollRowY - 35);
            Canvas.SetLeft(_keepLabel, _keepRowX);
            Canvas.SetTop(_keepLabel, _keepRowY - 35);
            Canvas.SetLeft(_countLabel, _keepRowX);
            Canvas.SetTop(_countLabel, _keepRowY + _diceSize + 10);
            LoadDice();
            Redraw();
        }
        #endregion
        #region not events
        private void LoadDice()
        {
            if (_diceLoaded) return;
            for (int i = 0; i < _dice.Length; i++)
            {
                _dice[i] = new Dice();
                _diceImage[i] = new Image();
                Table.Children.Add(_diceImage[i]);
            }
            _diceLoaded = true;
            _rollLabel.Text = "Roll these dice:";
            _keepLabel.Text = "Keep these dice:";
            _countLabel.Text = $"Rolls Remaining: {3-_rollCount}";
            _rollLabel.FontSize = 24;
            _keepLabel.FontSize = 24;
            _countLabel.FontSize = 24;
            Table.Children.Add(_rollLabel);
            Table.Children.Add(_keepLabel);
            Table.Children.Add(_countLabel);
        }

        private void NewGame()
        {
            for (int i = 0; i < _dice.Length; i++)
            {
                _dice[i].Side = 1;
                _dice[i].IsToRoll = true;
            }

            _turnCount = 0;
            _yahtzeeCount = 0;
            StartTurn();

            for (int i = 0; i < _scores.Length; i++)
            {
                _scores[i] = 0;
                _used[i] = false;
            }
            UpdateScorecard();
        }

        private void Redraw()
        {
            for (int i = 0; i < _dice.Length; i++)
            {
                _diceImage[i].Source = _dice[i].SideImage[_dice[i].Side];
                _diceImage[i].Width = _diceSize;
                _diceImage[i].Height = _diceSize;
                Canvas.SetLeft(_diceImage[i], _rollRowX + i * _diceSize);
                if (_dice[i].IsToRoll)
                    Canvas.SetTop(_diceImage[i], _rollRowY);
                else
                    Canvas.SetTop(_diceImage[i], _keepRowY);
            }
        }

        private void StartTurn()
        {
            _rollCount = 0;
            for (int i = 0; i < _dice.Length; i++)
                _dice[i].IsToRoll = true;
            Redraw();
            _countLabel.Text = $"Rolls Remaining: {3-_rollCount}";
            Instructions.Text = "Roll the dice.";
        }

        private void EndTurn()
        {
            if (IsYahtzee() && _used[15])
            {
                _yahtzeeCount++;
                if (_yahtzeeCount > 1)
                {
                    _scores[17] = 100 * (_yahtzeeCount - 1);
                    YahtzeeBonus.Text = _scores[17].ToString();
                }
            }
            _turnCount++;
            ClearToolTips();
            UpdateScorecard();
            if (_turnCount < 13)
                StartTurn();
            else
            {
                Instructions.Text = $"Game over. Your score is {_scores[20]}";
                _gameCompletion = DateTime.Now;
                UpdateStats();
            }
        }

        private void UpdateScorecard()
        {
            // upper section
            if (_used[1])
                Aces_Score.Text = _scores[1].ToString();
            else
                Aces_Score.Text = "";
            if (_used[2])
                Twos_Score.Text = _scores[2].ToString();
            else
                Twos_Score.Text = "";
            if (_used[3])
                Threes_Score.Text = _scores[3].ToString();
            else
                Threes_Score.Text = "";
            if (_used[4])
                Fours_Score.Text = _scores[4].ToString();
            else
                Fours_Score.Text = "";
            if (_used[5])
                Fives_Score.Text = _scores[5].ToString();
            else
                Fives_Score.Text = "";
            if (_used[6])
                Sixes_Score.Text = _scores[6].ToString();
            else
                Sixes_Score.Text = "";

            // lower section
            if (_used[10])
                ThreeOfAKind.Text = _scores[10].ToString();
            else
                ThreeOfAKind.Text = "";
            if (_used[11])
                FourOfAKind.Text = _scores[11].ToString();
            else
                FourOfAKind.Text = "";
            if (_used[12])
                FullHouse.Text = _scores[12].ToString();
            else
                FullHouse.Text = "";
            if (_used[13])
                SmStraight.Text = _scores[13].ToString();
            else
                SmStraight.Text = "";
            if (_used[14])
                LgStraight.Text = _scores[14].ToString();
            else
                LgStraight.Text = "";
            if (_used[15])
                Yahtzee.Text = _scores[15].ToString();
            else
                Yahtzee.Text = "";
            if (_used[16])
                Chance.Text = _scores[16].ToString();
            else
                Chance.Text = "";
            if (_scores[17] > 0)
                YahtzeeBonus.Text = _scores[17].ToString();
            else
                YahtzeeBonus.Text = "";

            // upper totals
            _scores[7] = _scores[1] + _scores[2] + _scores[3] + _scores[4] + _scores[5] + _scores[6];
            Upper_Subtotal.Text = _scores[7].ToString();

            if (_scores[7] >= 63)
            {
                _scores[8] = 35;
                Upper_Bonus.Text = _scores[8].ToString();
            }
            else
            {
                _scores[8] = 0;
                Upper_Bonus.Text = "";
            }

            _scores[9] = _scores[7] + _scores[8];
            Upper_Total.Text = _scores[9].ToString();

            // lower totals
            _scores[18] = _scores[10] + _scores[11] + _scores[12] + _scores[13] +
                _scores[14] + _scores[15] + _scores[16] + _scores[17];
            Lower_Total.Text = _scores[18].ToString();

            _scores[19] = _scores[9];
            Upper_Total2.Text = _scores[19].ToString();

            _scores[20] = _scores[18] + _scores[19];
            Grand_Total.Text = _scores[20].ToString();
        }

        private bool IsYahtzee()
        {
            if (_dice[0].Side == _dice[1].Side && _dice[1].Side == _dice[2].Side && _dice[2].Side == _dice[3].Side && _dice[3].Side == _dice[4].Side)
                return true;
            else
                return false;
        }

        private void UpdateStats()
        {
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

            // update stats with current game
            lastScore = _scores[20];
            lastDate = _gameCompletion;
            averageScore = (averageScore * numberOfGames + lastScore) / (numberOfGames + 1.0);
            numberOfGames++;
            // update low score
            if (lowScore == 0)
            {
                lowScore = _scores[20];
                lowDate = _gameCompletion;
            }
            else if (_scores[20] < lowScore)
            {
                lowScore = _scores[20];
                lowDate = _gameCompletion;
            }
            // update high score
            for (int i=0; i<10; i++)
            {
                if (_scores[20] > highScore[i])
                {
                    for (int j = 9; j > i; j--)
                    {
                        highScore[j] = highScore[j - 1];
                        highDate[j] = highDate[j - 1];
                    }
                    highScore[i] = _scores[20];
                    highDate[i] = _gameCompletion;
                    break;
                }
            }

            // save stats to Properties.Settings.Default
            Properties.Settings.Default.NumberOfGames = numberOfGames;
            Properties.Settings.Default.AverageScore = averageScore;
            Properties.Settings.Default.LastScore = lastScore;
            Properties.Settings.Default.LastDate = lastDate;
            Properties.Settings.Default.LowScore = lowScore;
            Properties.Settings.Default.LowDate = lowDate;
            Properties.Settings.Default.HighScore1 = highScore[0];
            Properties.Settings.Default.HighDate1 = highDate[0];
            Properties.Settings.Default.HighScore2 = highScore[1];
            Properties.Settings.Default.HighDate2 = highDate[1];
            Properties.Settings.Default.HighScore3 = highScore[2];
            Properties.Settings.Default.HighDate3 = highDate[2];
            Properties.Settings.Default.HighScore4 = highScore[3];
            Properties.Settings.Default.HighDate4 = highDate[3];
            Properties.Settings.Default.HighScore5 = highScore[4];
            Properties.Settings.Default.HighDate5 = highDate[4];
            Properties.Settings.Default.HighScore6 = highScore[5];
            Properties.Settings.Default.HighDate6 = highDate[5];
            Properties.Settings.Default.HighScore7 = highScore[6];
            Properties.Settings.Default.HighDate7 = highDate[6];
            Properties.Settings.Default.HighScore8 = highScore[7];
            Properties.Settings.Default.HighDate8 = highDate[7];
            Properties.Settings.Default.HighScore9 = highScore[8];
            Properties.Settings.Default.HighDate9 = highDate[8];
            Properties.Settings.Default.HighScore10 = highScore[9];
            Properties.Settings.Default.HighDate10 = highDate[9];
            Properties.Settings.Default.Save();
        }

        private void ShowToolTips()
        {
            if (_used[1])
                Border1.ToolTip = "used";
            else
                Border1.ToolTip = Calc_Aces().ToString();
            if (_used[2])
                Border2.ToolTip = "used";
            else
                Border2.ToolTip = Calc_Twos().ToString();
            if (_used[3])
                Border3.ToolTip = "used";
            else
                Border3.ToolTip = Calc_Threes().ToString();
            if (_used[4])
                Border4.ToolTip = "used";
            else
                Border4.ToolTip = Calc_Fours().ToString();
            if (_used[5])
                Border5.ToolTip = "used";
            else
                Border5.ToolTip = Calc_Fives().ToString();
            if (_used[6])
                Border6.ToolTip = "used";
            else
                Border6.ToolTip = Calc_Sixes().ToString();
            if (_used[10])
                Border10.ToolTip = "used";
            else
                Border10.ToolTip = Calc_ThreeOfAKind().ToString();
            if (_used[11])
                Border11.ToolTip = "used";
            else
                Border11.ToolTip = Calc_FourOfAKind().ToString();
            if (_used[12])
                Border12.ToolTip = "used";
            else
                Border12.ToolTip = Calc_FullHouse().ToString();
            if (_used[13])
                Border13.ToolTip = "used";
            else
                Border13.ToolTip = Calc_SmStraight().ToString();
            if (_used[14])
                Border14.ToolTip = "used";
            else
                Border14.ToolTip = Calc_LgStraight().ToString();
            if (_used[15])
                Border15.ToolTip = "used";
            else
                Border15.ToolTip = Calc_Yahtzee().ToString();
            if (_used[16])
                Border16.ToolTip = "used";
            else
                Border16.ToolTip = Calc_Chance().ToString();
        }

        private void ClearToolTips()
        {
            Border1.ToolTip = null;
            Border2.ToolTip = null;
            Border3.ToolTip = null;
            Border4.ToolTip = null;
            Border5.ToolTip = null;
            Border6.ToolTip = null;
            Border10.ToolTip = null;
            Border11.ToolTip = null;
            Border12.ToolTip = null;
            Border13.ToolTip = null;
            Border14.ToolTip = null;
            Border15.ToolTip = null;
            Border16.ToolTip = null;
        }
        #endregion
        #region calculate score
        private int Calc_Aces()
        {
            int score = 0;
            for (int i = 0; i < _dice.Length; i++)
                if (_dice[i].Side == 1)
                    score += 1;
            return score;
        }

        private int Calc_Twos()
        {
            int score = 0;
            for (int i = 0; i < _dice.Length; i++)
                if (_dice[i].Side == 2)
                    score += 2;
            return score;
        }

        private int Calc_Threes()
        {
            int score = 0;
            for (int i = 0; i < _dice.Length; i++)
                if (_dice[i].Side == 3)
                    score += 3;
            return score;
        }

        private int Calc_Fours()
        {
            int score = 0;
            for (int i = 0; i < _dice.Length; i++)
                if (_dice[i].Side == 4)
                    score += 4;
            return score;
        }

        private int Calc_Fives()
        {
            int score = 0;
            for (int i = 0; i < _dice.Length; i++)
                if (_dice[i].Side == 5)
                    score += 5;
            return score;
        }

        private int Calc_Sixes()
        {
            int score = 0;
            for (int i = 0; i < _dice.Length; i++)
                if (_dice[i].Side == 6)
                    score += 6;
            return score;
        }

        private int Calc_ThreeOfAKind()
        {
            int score = 0;

            // make a list of the dice and sort it
            List<int> dice = new List<int>();
            for (int i = 0; i < _dice.Length; i++)
                dice.Add(_dice[i].Side);
            dice.Sort();

            // look for three of a kind
            if ((dice[0] == dice[1] && dice[1] == dice[2]) ||
                (dice[1] == dice[2] && dice[2] == dice[3]) ||
                (dice[2] == dice[3] && dice[3] == dice[4]))
                score = dice[0] + dice[1] + dice[2] + dice[3] + dice[4];

            return score;
        }

        private int Calc_FourOfAKind()
        {
            int score = 0;

            // make a list of the dice and sort it
            List<int> dice = new List<int>();
            for (int i = 0; i < _dice.Length; i++)
                dice.Add(_dice[i].Side);
            dice.Sort();

            // look for four of a kind
            if ((dice[0] == dice[1] && dice[1] == dice[2] && dice[2] == dice[3]) ||
                (dice[1] == dice[2] && dice[2] == dice[3] && dice[3] == dice[4]))
                score = dice[0] + dice[1] + dice[2] + dice[3] + dice[4];

            return score;
        }

        private int Calc_FullHouse()
        {
            int score = 0;

            // make a list of the dice and sort it
            List<int> dice = new List<int>();
            for (int i = 0; i < _dice.Length; i++)
                dice.Add(_dice[i].Side);
            dice.Sort();

            // look for full house (2 & 3 of a kind)
            if (((dice[0] == dice[1] && dice[1] == dice[2] && dice[3] == dice[4]) ||
                (dice[0] == dice[1] && dice[2] == dice[3] && dice[3] == dice[4])) &&
                (dice[0] != dice[4]))
                score = 25;

            return score;
        }

        private int Calc_SmStraight()
        {
            int score = 0;
            int i;

            // make a list of the dice and sort it
            List<int> dice = new List<int>();
            for (i = 0; i < _dice.Length; i++)
                dice.Add(_dice[i].Side);
            dice.Sort();

            // remove duplicates
            i = 1;
            while (i < dice.Count)
            {
                if (dice[i] == dice[i - 1])
                    dice.RemoveAt(i);
                else
                    i++;
            }

            // look for sequence of four
            if (dice.Count == 4)
                if (dice[0] + 1 == dice[1] && dice[1] + 1 == dice[2] && dice[2] + 1 == dice[3])
                    score = 30;
            if (dice.Count == 5)
                if ((dice[0] + 1 == dice[1] && dice[1] + 1 == dice[2] && dice[2] + 1 == dice[3]) ||
                    (dice[1] + 1 == dice[2] && dice[2] + 1 == dice[3] && dice[3] + 1 == dice[4]))
                    score = 30;

            return score;
        }

        private int Calc_LgStraight()
        {
            int score = 0;

            // make a list of the dice and sort it
            List<int> dice = new List<int>();
            for (int i = 0; i < _dice.Length; i++)
                dice.Add(_dice[i].Side);
            dice.Sort();

            // look for sequence of five
            if (dice[0] + 1 == dice[1] && dice[1] + 1 == dice[2] && dice[2] + 1 == dice[3] && dice[3] + 1 == dice[4])
                score = 40;

            return score;
        }

        private int Calc_Yahtzee()
        {
            int score = 0;

            // look for five of a kind
            if (IsYahtzee())
                score = 50;

            return score;
        }

        private int Calc_Chance()
        {
            int score = 0;

            // sum all the dice
            for (int i = 0; i < _dice.Length; i++)
                score += _dice[i].Side;

            return score;
        }
        #endregion
    }
}