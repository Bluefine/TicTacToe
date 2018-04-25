using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TicTacToe
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum Places
        {
            Empty = 0,
            X = 1,
            O = 2
        }

        private readonly Places[] board = new Places[9];
        private readonly List<Button> buttons = new List<Button>();

        private readonly List<int[]> values = new List<int[]>();

        private int Choice;

        private Places computer;
        private Places current = Places.X;
        private Places player;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < 9; i++)
            {
                var btn = new Button
                {
                    Margin = new Thickness(5),
                    Width = 100,
                    Height = 100,
                    FontSize = 30
                };
                btn.Click += MakeMoveHuman;
                buttons.Add(btn);
                WrapPanelBoard.Children.Add(btn);
            }

            player = Places.X;
            computer = Places.O;
        }

        public void MakeMoveHuman(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var btnindex = buttons.IndexOf(btn);
            if (current == Places.X)
            {
                btn.Content = Places.X;
                board[btnindex] = Places.X;
            }
            else if (current == Places.O)
            {
                btn.Content = Places.O;
                board[btnindex] = Places.O;
            }

            btn.IsEnabled = false;
            current = changePlayer(current);
            AI();
            if (player == Places.X)
                if (GameOver(board, player))
                    MessageBox.Show("Player win");
            if (computer == Places.O)
                if (GameOver(board, computer))
                    MessageBox.Show("AI win");

            if (computer == Places.X)
                if (GameOver(board, computer))
                    MessageBox.Show("AI win");
            if (player == Places.O)
                if (GameOver(board, player))
                    MessageBox.Show("Player win");

            if (board.All(x => x != Places.Empty)) MessageBox.Show("TIE");
        }

        private Places changePlayer(Places _player)
        {
            if (_player == Places.X)
                return Places.O;
            return Places.X;
        }

        private void MenuItemReset_OnClick(object sender, RoutedEventArgs e)
        {
            for (var index = 0; index < buttons.Count; index++)
            {
                var btn = buttons[index];
                btn.Content = string.Empty;
                board[index] = Places.Empty;
                btn.IsEnabled = true;
            }

            player = player == Places.X ? Places.O : Places.X;
            computer = computer == Places.O ? Places.X : Places.O;
            current = changePlayer(current);
            values.Clear();
        }

        private bool GameOver(Places[] _board, Places _player)
        {
            if (_board[0] == _player && _board[1] == _player && _board[2] == _player ||
                _board[3] == _player && _board[4] == _player && _board[5] == _player ||
                _board[6] == _player && _board[7] == _player && _board[8] == _player ||
                _board[0] == _player && _board[3] == _player && _board[6] == _player ||
                _board[1] == _player && _board[4] == _player && _board[7] == _player ||
                _board[2] == _player && _board[5] == _player && _board[8] == _player ||
                _board[0] == _player && _board[4] == _player && _board[8] == _player ||
                _board[2] == _player && _board[4] == _player && _board[6] == _player)
                return true;

            return false;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            current = changePlayer(current);
            AI();
        }

        private void AI()
        {
            minimax(board, computer, 0);
            var btn = buttons.ElementAt(Choice);
            if (current == Places.X)
            {
                btn.Content = Places.X;
                board[Choice] = Places.X;
            }
            else if (current == Places.O)
            {
                btn.Content = Places.O;
                board[Choice] = Places.O;
            }

            btn.IsEnabled = false;
            current = changePlayer(current);
            values.Clear();
        }

        private Places[] makeMoveAI(Places[] _board, Places _player, int pos)
        {
            var _boardClone = (Places[]) _board.Clone();
            _boardClone[pos] = _player;
            return _boardClone;
        }

        private int score(Places[] _boardClone, int depth)
        {
            if (GameOver(_boardClone, computer))
                return 10 - depth;
            if (GameOver(_boardClone, player))
                return depth - 10;
            return 0;
        }

        private int minimax(Places[] _board, Places _player, int depth)
        {
            var _boardClone = (Places[]) _board.Clone();
            if (score(_boardClone, depth) != 0) return score(_boardClone, depth);
            if (_boardClone.All(x => x != Places.Empty)) return score(_boardClone, depth);
            depth++;
            var scores = new List<int>();
            var moves = new List<int>();

            for (var i = 0; i < 9; i++)
                if (_boardClone[i] == Places.Empty)
                {
                    scores.Add(minimax(makeMoveAI(_boardClone, _player, i), changePlayer(_player), depth));
                    moves.Add(i);
                }

            if (_player == computer)
            {
                var MaxScoreIndex = scores.IndexOf(scores.Max());
                Choice = moves[MaxScoreIndex];
                values.Add(new[] {scores.Max(), Choice});
                return scores.Max();
            }

            var MinScoreIndex = scores.IndexOf(scores.Min());
            Choice = moves[MinScoreIndex];
            values.Add(new[] {scores.Min(), Choice});
            return scores.Min();
        }
    }
}