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

namespace SudokuSolver
{
    enum MessageType{Message,Warning,Failure}
    /// <summary>
    /// Interaction logic for Sudoku.xaml
    /// </summary>
    public partial class Sudoku : UserControl
    {
        Board myboard = new Board();
        Board original = new Board();
        Stopwatch watch = new Stopwatch();
        int Solved=0;
        int Removed = 0;
        Board SolvedBoard = new Board();
        public Sudoku()
        {
            InitializeComponent();
            MyBoard.ItemsSource = myboard.StringSpace;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ShowMessage("Solving...",MessageType.Message);
            try
            {
                myboard.StringToInt();
            }
            catch(Exception ex)
            {
                ShowMessage("Invalid Board, Please Check.", MessageType.Failure);
                return;
            }
            original = myboard.DeepClone();
            try
            {
                bool test = myboard.IsValid();
                watch.Start();
                if (!Solve(myboard))
                {
                    watch.Stop();
                    myboard = original;
                    MyBoard.ItemsSource = myboard.StringSpace;
                    ShowMessage("Failed to find solution, time: " + watch.Elapsed.TotalSeconds, MessageType.Failure);
                }
                else
                {
                    watch.Stop();
                    ShowMessage("Solved Already Silly!", MessageType.Warning);
                }
                watch.Reset();
            }
            catch (System.ArgumentException ex)
            {
                MyBoard.ToString();
                MyBoard.ItemsSource = myboard.StringSpace;
                MyBoard.Items.Refresh();
                ShowMessage("Solved in: "+watch.Elapsed.TotalSeconds+ " seconds.",MessageType.Message);
                watch.Stop();
                watch.Reset();
                return;
            }
        }
        bool Solve(Board board)
        {
            List<Board> sucessors = new System.Collections.Generic.List<Board>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (board.spaces[i][j] == 0)
                    {
                        for (int r = 1; r < 10; r++)
                        {
                            board.SetSpace(r, i, j);
                            if (board.IsSolved(i, j))
                            {
                                watch.Stop();
                                myboard = board.DeepClone();
                                throw new System.ArgumentException("Solved");
                            }
                            if (board.IsValid(i, j))
                            {
                                sucessors.Add(board.DeepClone());
                            }
                        }
                        foreach (Board brd in sucessors)
                        {
                            Solve(brd);
                        }
                        return false;
                    }
                }
            }
            if (myboard.IsValid())
            {
                return true;
            }
            return false;
        }
        bool SolveComplete(Board board)
        {
            List<Board> sucessors = new System.Collections.Generic.List<Board>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (board.spaces[i][j] == 0)
                    {
                        for (int r = 1; r < 10; r++)
                        {
                            board.SetSpace(r, i, j);
                            if (board.IsSolved(i, j))
                            {
                                watch.Stop();
                                Solved++;
                                return true;
                            }
                            if (board.IsValid(i,j))
                            {
                                sucessors.Add(board.DeepClone());
                            }
                        }
                        foreach (Board brd in sucessors)
                        {
                            SolveComplete(brd);
                        }
                        return false;
                    }
                }
            }
            if (myboard.IsValid())
            {
                return true;
            }
            return false;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            myboard = new Board();
            MyBoard.ItemsSource = myboard.StringSpace;
            ShowMessage("Board Reset",MessageType.Message);
        }
        private void ShowMessage(String message, MessageType type)
        {
            TextBox_Message.Text = message;
            switch (type)
            {
                case MessageType.Message:
                    TextBox_Message.Background = Brushes.LightGreen;
                    break;
                case MessageType.Warning:
                    TextBox_Message.Background = Brushes.Yellow;
                    break;
                case MessageType.Failure:
                    TextBox_Message.Background = Brushes.Red;
                    break;
                default:
                    break;
            }
        }
        private bool Generate(Board board)
        {
            List<Board> sucessors = new System.Collections.Generic.List<Board>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (board.spaces[i][j] == 0)
                    {
                        Random rand = new Random();
                        List<int> nums = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                        while(nums.Count!=0)
                        {
                            int r = nums[rand.Next(0,nums.Count)];
                            nums.Remove(r);
                            board.SetSpace(r, i, j);
                            if (board.IsValid())
                            {
                                sucessors.Add(board.DeepClone());
                                if (board.IsSolved(i, j))
                                {
                                    watch.Stop();
                                    myboard = board.DeepClone();
                                    throw new System.ArgumentException("Solved");
                                }
                            }
                        }
                        foreach (Board brd in sucessors)
                        {
                            Generate(brd);
                        }
                    }
                }
            }
            if (myboard.IsValid())
            {
                return true;
            }
            return false;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Removed = 0;
            Solved = 0;
            myboard = new Board();
            try
            {
                Generate(myboard);
            }
            catch (System.ArgumentException ex)
            {
                try
                {
                    RemoveSpaces(myboard);
                }
                catch (System.ArgumentException ex2)
                {
                    myboard.IntToString();
                    MyBoard.ItemsSource = myboard.StringSpace;
                }
            }
        }   
        private void RemoveSpaces(Board board)
        {
            List<Tuple<int, int>> Spaces = new List<Tuple<int, int>>();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Spaces.Add(new Tuple<int, int>(i, j));
                }
            }
            Random rand = new Random();
            for (int i = 0; i < 81; i++)
            {
                int index = rand.Next(0, Spaces.Count);
                if (board.spaces[Spaces[index].Item1][Spaces[index].Item2] != 0)
                {
                    int temp = board.spaces[Spaces[index].Item1][Spaces[index].Item2];
                    board.spaces[Spaces[index].Item1][Spaces[index].Item2] = 0;
                    Removed++;
                    Solved = 0;
                    SolveComplete(board.DeepClone());
                    if (Solved == 1)
                    {
                    }
                    if (Solved == 1 && Removed >= 50)
                    {
                        myboard = board.DeepClone();
                        throw new System.ArgumentException("Generated");
                    }
                    else if (Solved == 1 && Removed < 50)
                    {
                        RemoveSpaces(board.DeepClone());
                    }
                    board.spaces[Spaces[index].Item1][Spaces[index].Item2] = temp;
                    Removed--;
                }
                Spaces.RemoveAt(index);
            }
        }
    }
}

