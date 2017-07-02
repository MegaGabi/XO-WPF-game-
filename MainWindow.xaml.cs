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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool Xwin = false, Owin = false;
        int first_X_pos = -1;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var sender_text = ((TextBlock)sender);
            if(sender_text.Text != "O" && sender_text.Text != "X")
            {
                //player press
                sender_text.Text = "X";

                var All = new List<string>();
                foreach(TextBlock item in grid.Children)
                    All.Add(item.Text);

                var XO = new string[3, 3];
                int i = 0;
                foreach (var str in All)
                {
                    XO[(int)(i / 3), i % 3] = str;
                    i++;
                }

                if (All.Count(x => (x == "X")) == 1)
                {
                    for (int x = 0; x < XO.GetLength(0); x++)
                    {
                        for (int y = 0; y < XO.GetLength(1); y++)
                        {
                            if (XO[x, y] == "X")
                            {
                                first_X_pos = x * XO.GetLength(0) + y;
                            }
                        }
                    }
                }

                Xwin = CheckWin(XO, "X");

                if (All.Count(x => (x == "X" || x == "O")) < All.Count - 1 && !Xwin)
                {
                    int answer = AI(XO);

                    if (answer != -1)
                    {
                        int x = (int)(answer / 3), y = answer % 3;
                        XO[x, y] = "O";
                        ((TextBlock)grid.Children[answer]).Text = "O";
                    }

                    Xwin = CheckWin(XO, "X");
                    Owin = CheckWin(XO, "O");
                }
                else if(!Xwin)
                    Xwin = Owin = true;

                if (Xwin == Owin && (Xwin || Owin))
                {
                    MessageBox.Show("Ничья!");
                    Reset(grid.Children);
                }
                else if (Xwin)
                {
                    MessageBox.Show("Вы выйграли!");
                    Reset(grid.Children);
                }
                else if (Owin)
                {
                    MessageBox.Show("Вы проиграли!");
                    Reset(grid.Children);
                }
            }
        }

        int AI(string[,] XO)
        {
            int pos = 0;

            //O
            for (int i = 0; i < 3; i++)
            {
                if(CheckForTwoSimilar(XO[i, 0], XO[i, 1], XO[i, 2], "O", ref pos))
                {
                    return i * 3 + pos;
                }

                if (CheckForTwoSimilar(XO[0, i], XO[1, i], XO[2, i], "O", ref pos))
                {
                    return pos * 3 + i;
                }
            }

            if (CheckForTwoSimilar(XO[0, 0], XO[1, 1], XO[2, 2], "O", ref pos))
            {
                return pos * 4;
            }

            if (CheckForTwoSimilar(XO[0, 2], XO[1, 1], XO[2, 0], "O", ref pos))
            {
                return pos * 2 + 2;
            }

            //X
            for (int i = 0; i < 3; i++)
            {
                if (CheckForTwoSimilar(XO[i, 0], XO[i, 1], XO[i, 2], "X", ref pos))
                {
                    return i * 3 + pos;
                }

                if (CheckForTwoSimilar(XO[0, i], XO[1, i], XO[2, i], "X", ref pos))
                {
                    return pos * 3 + i;
                }
            }

            if (CheckForTwoSimilar(XO[0, 0], XO[1, 1], XO[2, 2], "X", ref pos))
            {
                return pos * 4;
            }

            if (CheckForTwoSimilar(XO[0, 2], XO[1, 1], XO[2, 0], "X", ref pos))
            {
                return pos * 2 + 2;
            }

            List<int> free_positions = new List<int>();
            int fpos_idx = 0;

            //diagonal
            if (first_X_pos % 2 == 0 && first_X_pos != 4)
            {
                if (XO[1, 1] == "-")
                    return 4;

                for (int i = 0; i < XO.GetLength(0) * XO.GetLength(1); i += 2)
                {
                    if (i != 4 && CheckForSimilar(XO, "O", i, ref pos))
                        return pos;
                }

                for (int i = 1; i < XO.GetLength(0) * XO.GetLength(1); i += 2)
                    if (XO[i / XO.GetLength(0), (int)i % XO.GetLength(0)] == "-")
                        free_positions.Add(i);
            }
            //horizontal
            else if (first_X_pos % 2 == 1 || first_X_pos == 4)
            {
                for (int i = 2; i < XO.GetLength(0) * XO.GetLength(1); i += 2)
                    if (XO[i / XO.GetLength(0), (int)i % XO.GetLength(0)] == "-")
                        free_positions.Add(i);
            }
            

            if (free_positions.Count > 0)
            {
                fpos_idx = new Random().Next(free_positions.Count);
                return free_positions[fpos_idx];
            }

            //Random Pos
            free_positions = new List<int>();

            for(int i = 0; i < XO.GetLength(0); i++)
            {
                for(int j = 0; j < XO.GetLength(1); j++)
                    if(XO[i, j] == "-")
                    {
                        free_positions.Add(i * XO.GetLength(0) + j);
                    }
            }

            //Random
            fpos_idx = new Random().Next(free_positions.Count);

            return free_positions[fpos_idx];
            
        }

        private static bool CheckForTwoSimilar(string f, string s, string t, string toCheck, ref int pos)
        {
            if (s == toCheck && t == toCheck)
            {
                if (f == "-")
                {
                    pos = 0;
                    return true;
                }
            }

            if (f == toCheck && t == toCheck)
            {
                if (s == "-")
                {
                    pos = 1;
                    return true;
                }
            }

            if (f == toCheck && s == toCheck)
            {
                if (t == "-")
                {
                    pos = 2;
                    return true;
                }
            }

            return false;
        }

        private static bool CheckForSimilar(string[,] XO, string toCheck, int mpos, ref int pos)
        {
            string[] bufXO = new string[XO.GetLength(0) * XO.GetLength(1)];

            for(int i = 0; i < XO.GetLength(0); i++)
            {
                for (int j = 0; j < XO.GetLength(1); j++)
                {
                    bufXO[i * 3 + j] = XO[i, j];
                }
            }

            if (bufXO[mpos] == toCheck)
            {
                int x = mpos == 0 || mpos == 8 ? 2 : 0,
                    y = mpos == 0 || mpos == 8 ? 6 : 8;

                int prom_x = mpos == 0 || mpos == 2 ? 1 : mpos == 6 ? 3 : 5,
                    prom_y = mpos == 0              ? 3 : mpos == 2 ? 5 : mpos == 6? 7 : 7;

                if (bufXO[prom_x] == "-" && bufXO[x] == "-")
                {
                    pos = x;
                    return true;
                }
                else if (bufXO[prom_y] == "-" && bufXO[y] == "-") 
                {
                    pos = y;
                    return true;
                }
            }

            return false;
        }

        public bool CheckWin(string[,] XO, string plr)
        {

            for (int i = 0; i < 3; i++)
            {
                if (XO[i, 0] == plr && XO[i, 1] == plr && XO[i, 2] == plr)
                {
                    return true;
                }

                if (XO[0, i] == plr && XO[1, i] == plr && XO[2, i] == plr)
                {
                    return true;
                }
            }

            if (XO[0, 0] == plr && XO[1, 1] == plr && XO[2, 2] == plr)
            {
                return true;
            }

            if (XO[0, 2] == plr && XO[1, 1] == plr && XO[2, 0] == plr)
            {
                return true;
            }

            return false;
        }

        private void Reset(UIElementCollection GridChildren)
        {
            for (int i = 0; i < GridChildren.Count; i++)
            {
                ((TextBlock)GridChildren[i]).Text = "-";
            }

            first_X_pos = -1;
        }
    }
}

