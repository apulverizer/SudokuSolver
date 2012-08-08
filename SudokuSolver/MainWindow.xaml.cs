using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Diagnostics;
using System.Windows.Forms;

namespace SudokuSolver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Board myboard = new Board();
        Board Solved = new Board();
        bool solved = false;
        public MainWindow()
        {
            InitializeComponent();
            myboard.SetSpace(1, 0, 0);
            MyBoard.ItemsSource = myboard.StringSpace;
        }

        private void MyBoard_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            myboard.StringToInt();
            bool test =myboard.IsValid();
            Solve(myboard);
            if (solved)
            {
                MyBoard.ItemsSource = Solved.spaces;
                MyBoard.Items.Refresh();
            }
        }
        bool Solve(Board board)
        {
            List<Board> sucessors = new System.Collections.Generic.List<Board>();
            for (int i = 0; i < 9; i++)
            {
                if (!solved)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (board.spaces[i][j] == 0)
                        {
                            if (!solved)
                            {
                                for (int r = 1; r < 10; r++)
                                {
                                    if (!solved)
                                    {
                                        board.SetSpace(r, i, j);
                                        if (board.IsSolved())
                                        {
                                            Solved = board;
                                            solved = true;
                                            return true;
                                        }
                                        if (board.IsValid())
                                        {
                                            sucessors.Add(board.DeepClone());
                                        }
                                        foreach (Board brd in sucessors)
                                        {
                                            if (!solved)
                                            {
                                                Solve(brd);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
}
