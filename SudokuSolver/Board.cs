using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SudokuSolver
{
    [Serializable]
    class Board :INotifyPropertyChanged
    {
        public Board()
        {
            spaces = new ObservableCollection<ObservableCollection<int>>();
            StringSpace = new ObservableCollection<ObservableCollection<string>>();
            StringSpace.CollectionChanged += new NotifyCollectionChangedEventHandler(spaces_CollectionChanged);
            
            for (int i = 0; i < 9; i++)
            {
                spaces.Add(new ObservableCollection<int>());
                StringSpace.Add(new ObservableCollection<string>());
                for (int j = 0; j < 9; j++)
                {
                    spaces[i].Add(new int());
                    StringSpace[i].Add("");
                }
            }
        }
        void spaces_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Collection");
        }
        public ObservableCollection<ObservableCollection<int>> spaces { get; set; }
        public ObservableCollection<ObservableCollection<String>> StringSpace { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        public bool StringToInt()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if ((StringSpace[i][j] == "") || (StringSpace[i][j].Contains(' ')) || (StringSpace[i][j]=="\b")|| (StringSpace[i][j]=="\t"))
                    {
                        spaces[i][j] = 0;
                    }
                    else
                    {
                        spaces[i][j] = Convert.ToInt32(StringSpace[i][j]);
                        if (spaces[i][j] <= 0 || spaces[i][j] > 9)
                        {
                            throw new System.ArgumentException("Invalid character, please check.");
                        }
                    }
                }
            }
            return true;
        }
        public bool IntToString()
        {

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (spaces[i][j] == 0)
                    {
                        StringSpace[i][j] = "";
                    }
                    else
                    {
                        StringSpace[i][j] = spaces[i][j].ToString();
                    }
                }
            }
            return true;
        }

        public bool SetSpace(int num, int row, int col)
        {
            try
            {
                spaces[row][col] = num;
                if (num <= 9 && num > 0)
                    StringSpace[row][col]=num.ToString();
                else
                    StringSpace[row][col] = "";
                return true;
            }
            catch
            {
                Debug.Assert(false);
                return false;
            }

        }
        public int GetSpace(int row, int col)
        {
            try
            {
                return spaces[row][col];
            }
            catch
            {
                Debug.Assert(false);
                return -1;
            }
        }
        public bool IsSolved(int row,int col)
        {
            if (this.IsValid(row,col))
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (spaces[i][j] > 10 || spaces[i][j] <= 0)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsValid(int i, int j)
        {
            List<int> RowIndices;
            List<int> ColIndices;

            for (int r = 0; r < 9; r++)
            {
                if (spaces[i][j] == spaces[i][r] && r != j && spaces[i][j] != 0)
                {
                    return false;
                }
                if (spaces[r][j] == spaces[i][j] && i != r && spaces[i][j] != 0)
                {
                    return false;
                }
            }
            if (i <= 2)
            {
                RowIndices = new List<int>() { 0, 1, 2 };
            }
            else if (i <= 5)
            {
                RowIndices = new List<int>() { 3, 4, 5 };
            }
            else
            {
                RowIndices = new List<int>() { 6, 7, 8 };
            }
            if (j <= 2)
            {
                ColIndices = new List<int>() { 0, 1, 2 };
            }
            else if (j <= 5)
            {
                ColIndices = new List<int>() { 3, 4, 5 };
            }
            else
            {
                ColIndices = new List<int>() { 6, 7, 8 };
            }
            foreach (int row in RowIndices)
            {
                foreach (int col in ColIndices)
                {
                    if (spaces[i][j] == spaces[row][col] && (i != row && j != col) && spaces[i][j] != 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool IsValid()
        {
            List<int> RowIndices;
            List<int> ColIndices;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    for (int r = 0; r < 9; r++)
                    {
                        if (spaces[i][j] == spaces[i][r] && r != j && spaces[i][j]!=0)
                        {
                            return false;
                        }
                        if (spaces[r][j] == spaces[i][j] && i != r && spaces[i][j] != 0)
                        {
                            return false;
                        }
                    }
                    if (i <= 2)
                    {
                        RowIndices = new List<int>() { 0, 1, 2 };
                    }
                    else if (i <= 5)
                    {
                        RowIndices = new List<int>() { 3, 4, 5 };
                    }
                    else
                    {
                        RowIndices = new List<int>() { 6, 7, 8 };
                    }
                    if (j <= 2)
                    {
                        ColIndices = new List<int>() { 0, 1, 2 };
                    }
                    else if (j <= 5)
                    {
                        ColIndices = new List<int>() { 3, 4, 5 };
                    }
                    else
                    {
                        ColIndices = new List<int>() { 6, 7, 8 };
                    }
                    foreach (int row in RowIndices)
                    {
                        foreach (int col in ColIndices)
                        {
                            if (spaces[i][j] == spaces[row][col] && (i != row && j != col)&& spaces[i][j]!=0)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
    public static class ExtensionMethods
    {
        // Deep clone
        public static T DeepClone<T>(this T a)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}