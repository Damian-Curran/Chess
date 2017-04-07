using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ComplexStackPanel
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        //global variables accesed by any method inside partial class MainPage
        int tapped = 0;
        string selected = " ";
        TextBlock moved;
        int turn = 2;
        int nextTurn;
        int pathClear = 0;

        //arrays used for setting accesing specific tiles
        char[] col = new char [8] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'};
        char[] rows = new char[8] { '8','7','6','5','4','3','2','1' };

        //arrays used to track all current positions and possible moves of all pieces except king
        string[] whiteCheck = new string[200];
        string[] blackCheck = new string[200];

        //arrays used to track kings current positions and possible moves
        string[] blackKing = new string[10];
        string[] whiteKing = new string[10];

        //move is called whenever a tile is touched
        private void move(object sender, RoutedEventArgs e)
        {
            TextBlock txt = (TextBlock)sender;
            if (tapped == 0 && txt.Text != ".")
            {
                //if turn % 2 then its white player turn
                if(turn % 2 == 0)
                {
                    if (((SolidColorBrush)txt.Foreground).Color == Colors.White)
                    {
                        //tapped used to focus how many tiles touched
                        tapped++;
                        //saving text tapped first
                        selected = txt.Text;
                        // saving textblock tapped first to created Textblock
                        moved = txt;
                    }
                }
                else if(turn % 2 == 1)
                {
                    //if turn remainer 1 then black player turn
                    if (((SolidColorBrush)txt.Foreground).Color == Colors.Black)
                    {
                        //tapped used to focus how many tiles touched
                        tapped++;
                        //saving text tapped first
                        selected = txt.Text;
                        // saving textblock tapped first to created Textblock
                        moved = txt;
                    }
                }
            }
            else if (tapped == 1)
            {
                //tapped == 1 then second tile tapped 
                //save grid numbers of where piece moving from and moving to
                int moveFromRow = (int)moved.GetValue(Grid.RowProperty);
                int moveToRow = (int)txt.GetValue(Grid.RowProperty);

                int moveFromCol = (int)moved.GetValue(Grid.ColumnProperty);
                int moveToCol = (int)txt.GetValue(Grid.ColumnProperty);

                //if colour moved to same tile of same colour not allowed
                if (((SolidColorBrush)txt.Foreground).Color != ((SolidColorBrush)moved.Foreground).Color || txt.Text == ".")
                {
                    //if piece moving contains Text Kn enter if
                    if (moved.Text == "Kn")
                    {
                        //variables send to different method for easier code readability
                        knight(txt,moveFromRow,moveFromCol, moveToCol, moveToRow);
                    }
                    else if (moved.Text == "P")
                    {
                        pawn(txt, moveFromRow, moveFromCol, moveToCol, moveToRow);
                    }

                    else if (moved.Text == "K")
                    {
                        king(txt, moveFromRow, moveFromCol, moveToCol, moveToRow);
                    }
                    else if (moved.Text == "B")
                    {
                        bishop(txt, moveFromRow, moveFromCol, moveToCol, moveToRow);
                    }
                    else if (moved.Text == "Q")
                    {
                        queen(txt, moveFromRow, moveFromCol, moveToCol, moveToRow);
                    }
                    else if (moved.Text == "C")
                    {
                        castle(txt, moveFromRow, moveFromCol, moveToCol, moveToRow);
                    }
                }
                
                //if move allowed and move completed then turn++ which allows next player to move
                if (nextTurn == 1)
                {
                    if (turn % 2 == 0)
                    {
                        txt.Foreground = new SolidColorBrush(Colors.White);
                    }
                    else if (turn % 2 == 1)
                    {
                        txt.Foreground = new SolidColorBrush(Colors.Black);
                    }
                    
                    turn++;
                    nextTurn= 0;
                    //plays audio sound using mediaPlayer
                    mycontrol.Play();
                    pathClear = 0;
                }

                tapped = 0;
                //not finished method to check against all positions on board for check/checkmate
                //check();
            }
        }

        #region
        private void select(String name, int pathClear, TextBlock txt, int g)
        {
            //check if next step in path is empty to continue on path
            TextBlock clear = (TextBlock)FindName(name);
            if (clear.Text == ".")
            {
                pathClear++;
                //if incrimented path is same as distance to travel then path clear
                if (pathClear == g)
                {
                    //move text from current till to targeted tile
                    txt.Text = selected;
                    //replace old tile with "."
                    moved.Text = ".";
                    //change "." color to anything but white/black so no collision issues
                    //make visibility hidden so dots are hidden from user
                    moved.Foreground = new SolidColorBrush(Colors.Navy);
                    moved.Visibility = Visibility.Collapsed;
                    nextTurn = 1;
                }
            }
            else if (clear.Text == txt.Text)
            {
                txt.Text = selected;
                moved.Text = ".";
                moved.Foreground = new SolidColorBrush(Colors.Navy);
                moved.Visibility = Visibility.Collapsed;
                nextTurn = 1;
            }
        }
        #endregion

        #region
        private void knight(TextBlock txt, int moveFromRow, int moveFromCol, int moveToCol, int moveToRow)
        {
            //if to check for all possible moves of the knight in L shapes
            //no check if path clear as knight floats over
            if ((moveToRow == (moveFromRow - 2) && moveToCol == (moveFromCol + 1)) || (moveToRow == (moveFromRow + 2) && moveToCol == (moveFromCol + 1))
                            || (moveToRow == (moveFromRow + 2) && moveToCol == (moveFromCol - 1)) || (moveToRow == (moveFromRow - 2) && moveToCol == (moveFromCol - 1))
                            || (moveToRow == (moveFromRow + 1) && moveToCol == (moveFromCol + 2)) || (moveToRow == (moveFromRow + 1) && moveToCol == (moveFromCol - 2))
                            || (moveToRow == (moveFromRow - 1) && moveToCol == (moveFromCol + 2)) || (moveToRow == (moveFromRow - 1) && moveToCol == (moveFromCol - 2)))
            {
                txt.Text = selected;
                moved.Text = ".";
                moved.Foreground = new SolidColorBrush(Colors.Navy);
                moved.Visibility = Visibility.Collapsed;
                nextTurn = 1;
            }
        }
        #endregion

        private void pawn(TextBlock txt, int moveFromRow, int moveFromCol, int moveToCol, int moveToRow)
        {
            if (turn % 2 == 0)
            {
                if ((moveToRow == (moveFromRow - 1) && moveToCol == (moveFromCol)) || (moveToRow == (moveFromRow - 1) && moveToCol == (moveFromCol - 1))
                     || (moveToRow == (moveFromRow - 1) && moveToCol == (moveFromCol + 1)))
                {
                    int k = moveToRow - moveFromRow;
                    int j = moveToCol - moveFromCol;
                    int g = k;
                    String name = "";

                    if (k < 0)
                    {
                        g = k * -1;
                    }

                    for (int i = 0; i < g; i++)
                    {
                        if (k < 0)
                        {
                            if (((SolidColorBrush)txt.Foreground).Color != Colors.Black)
                            {
                                if (j == 0)
                                {
                                    //if target path not black infront and j==0 so same column, allowed movement straight forward
                                    name = col[moveFromCol] + "" + rows[moveFromRow - i - 1];

                                    select(name, pathClear, txt, g);
                                }
                            }
                            else if (((SolidColorBrush)txt.Foreground).Color == Colors.Black)
                            {
                                if (j != 0)
                                {
                                    //if target is black and j!=0 so target tile is not in same column then allow diagnol movement
                                    txt.Text = selected;
                                    moved.Text = ".";
                                    moved.Foreground = new SolidColorBrush(Colors.Navy);
                                    moved.Visibility = Visibility.Collapsed;
                                    nextTurn = 1;
                                }
                            }
                        }
                    }
                }
            }
            else if (turn % 2 == 1)
            {
                if ((moveToRow == (moveFromRow + 1) && moveToCol == (moveFromCol)) || (moveToRow == (moveFromRow + 1) && moveToCol == (moveFromCol - 1))
                     || (moveToRow == (moveFromRow + 1) && moveToCol == (moveFromCol + 1)))
                {
                    int k = moveToRow - moveFromRow;
                    int j = moveToCol - moveFromCol;
                    int g = k;
                    String name = "";

                    if (k < 0)
                    {
                        g = k * -1;
                    }

                    for (int i = 0; i < g; i++)
                    {
                        if (k > 0)
                        {
                            if (((SolidColorBrush)txt.Foreground).Color != Colors.White)
                            {
                                if (j == 0)
                                {
                                    name = col[moveFromCol] + "" + rows[moveFromRow + i + 1];

                                    select(name, pathClear, txt, g);
                                }
                            }
                            else if (((SolidColorBrush)txt.Foreground).Color == Colors.White)
                            {
                                if (j != 0)
                                {
                                    txt.Text = selected;
                                    //txt.Foreground = new SolidColorBrush(Colors.White);
                                    moved.Text = ".";
                                    moved.Foreground = new SolidColorBrush(Colors.Navy);
                                    moved.Visibility = Visibility.Collapsed;
                                    nextTurn = 1;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void king(TextBlock txt, int moveFromRow, int moveFromCol, int moveToCol, int moveToRow)
        {
            //checks against all possible king moves
            if ((moveToRow == (moveFromRow + 1) && moveToCol == (moveFromCol)) || (moveToRow == (moveFromRow - 1) && moveToCol == (moveFromCol))
                            || (moveToRow == (moveFromRow) && moveToCol == (moveFromCol + 1)) || (moveToRow == (moveFromRow) && moveToCol == (moveFromCol - 1))
                            || (moveToRow == (moveFromRow + 1) && moveToCol == (moveFromCol + 1)) || (moveToRow == (moveFromRow - 1) && moveToCol == (moveFromCol - 1))
                            || (moveToRow == (moveFromRow - 1) && moveToCol == (moveFromCol + 1)) || (moveToRow == (moveFromRow + 1) && moveToCol == (moveFromCol - 1)))
            {
                txt.Text = selected;
                moved.Text = ".";
                moved.Foreground = new SolidColorBrush(Colors.Navy);
                moved.Visibility = Visibility.Collapsed;
                nextTurn = 1;
            }
        }

        private void queen(TextBlock txt, int moveFromRow, int moveFromCol, int moveToCol, int moveToRow)
        {
            //check all possible queen moves using for loop
            for (int q = 0; q <= 7; q++)
            {
                if ((moveToRow == (moveFromRow + q)) && (moveToCol == (moveFromCol + q)) || (moveToRow == (moveFromRow - q)) && (moveToCol == (moveFromCol + q)) ||
                    (moveToRow == (moveFromRow + q)) && (moveToCol == (moveFromCol - q)) || (moveToRow == (moveFromRow - q)) && (moveToCol == (moveFromCol - q)))
                {
                    int k = moveToRow - moveFromRow;
                    int j = moveToCol - moveFromCol;
                    int g = k;
                    String name = "";

                    if (k < 0)
                    {
                        g = k * -1;
                    }

                    ;
                    for (int i = 0; i < g; i++)
                    {
                        if (k > 0 && j > 0)
                        {
                            name = col[moveFromCol + i + 1] + "" + rows[moveFromRow + i + 1];
                        }
                        if (k > 0 && j < 0)
                        {
                            name = col[moveFromCol - i - 1] + "" + rows[moveFromRow + i + 1];
                        }
                        if (k < 0 && j > 0)
                        {
                            name = col[moveFromCol + i + 1] + "" + rows[moveFromRow - i - 1];
                        }
                        if (k < 0 && j < 0)
                        {
                            name = col[moveFromCol - i - 1] + "" + rows[moveFromRow - i - 1];
                        }

                        select(name, pathClear, txt, g);
                    }
                }
                else if ((moveToRow == (moveFromRow + q)) && (moveToCol == (moveFromCol)) || (moveToRow == (moveFromRow - q)) && (moveToCol == (moveFromCol)) ||
                    (moveToRow == (moveFromRow)) && (moveToCol == (moveFromCol + q)) || (moveToRow == (moveFromRow)) && (moveToCol == (moveFromCol - q)))
                {
                    int k = moveToRow - moveFromRow;
                    int j = moveToCol - moveFromCol;
                    int g = 0;
                    String name = "";

                    if (k < 0)
                    {
                        g = k * -1;
                    }
                    else if (j < 0)
                    {
                        g = k * -1;
                    }
                    else if (k > 0)
                    {
                        g = k;
                    }
                    else if (j > 0)
                    {
                        g = j;
                    }

                    ;
                    for (int i = 0; i < g; i++)
                    {
                        if (k > 0)
                        {
                            name = col[moveFromCol] + "" + rows[moveFromRow + i + 1];
                        }
                        if (j < 0)
                        {
                            name = col[moveFromCol - i - 1] + "" + rows[moveFromRow];
                        }
                        if (k < 0)
                        {
                            name = col[moveFromCol] + "" + rows[moveFromRow - i - 1];
                        }
                        if (j > 0)
                        {
                            name = col[moveFromCol - i - 1] + "" + rows[moveFromRow];
                        }

                        select(name, pathClear, txt, g);
                    }
                }
            }
        }

        private void bishop(TextBlock txt, int moveFromRow, int moveFromCol, int moveToCol, int moveToRow)
        {
            for (int q = 0; q <= 7; q++)
            {
                if ((moveToRow == (moveFromRow + q)) && (moveToCol == (moveFromCol + q)) || (moveToRow == (moveFromRow - q)) && (moveToCol == (moveFromCol + q)) ||
                    (moveToRow == (moveFromRow - q)) && (moveToCol == (moveFromCol - q)) || (moveToRow == (moveFromRow + q)) && (moveToCol == (moveFromCol - q)))
                {
                    int k = moveToRow - moveFromRow;
                    int j = moveToCol - moveFromCol;
                    int g = k;
                    String name = "";

                    if (k < 0)
                    {
                        g = k * -1;
                    }

                    ;
                    for (int i = 0; i < g; i++)
                    {
                        if (k > 0 && j > 0)
                        {
                            name = col[moveFromCol + i + 1] + "" + rows[moveFromRow + i + 1];
                        }
                        if (k > 0 && j < 0)
                        {
                            name = col[moveFromCol - i - 1] + "" + rows[moveFromRow + i + 1];
                        }
                        if (k < 0 && j > 0)
                        {
                            name = col[moveFromCol + i + 1] + "" + rows[moveFromRow - i - 1];
                        }
                        if (k < 0 && j < 0)
                        {
                            name = col[moveFromCol - i - 1] + "" + rows[moveFromRow - i - 1];
                        }

                        select(name, pathClear, txt, g);
                    }
                }
            }
        }

        private void castle(TextBlock txt, int moveFromRow, int moveFromCol, int moveToCol, int moveToRow)
        {
            for (int q = 0; q <= 7; q++)
            {
                if ((moveToRow == (moveFromRow + q)) && (moveToCol == (moveFromCol)) || (moveToRow == (moveFromRow - q)) && (moveToCol == (moveFromCol)) ||
                    (moveToRow == (moveFromRow)) && (moveToCol == (moveFromCol + q)) || (moveToRow == (moveFromRow)) && (moveToCol == (moveFromCol - q)))
                {
                    int k = moveToRow - moveFromRow;
                    int j = moveToCol - moveFromCol;
                    int g = k;
                    String name = "";

                    if (k < 0)
                    {
                        g = k * -1;
                    }

                    ;
                    for (int i = 0; i < g; i++)
                    {
                        if (k > 0)
                        {
                            name = col[moveFromCol] + "" + rows[moveFromRow + i + 1];
                        }
                        if (j < 0)
                        {
                            name = col[moveFromCol - i - 1] + "" + rows[moveFromRow];
                        }
                        if (k < 0)
                        {
                            name = col[moveFromCol] + "" + rows[moveFromRow - i - 1];
                        }
                        if (j > 0)
                        {
                            name = col[moveFromCol - i - 1] + "" + rows[moveFromRow];
                        }

                        select(name, pathClear, txt, g);
                    }
                }
            }
        }

        //unfinished checker to search for checkmate or checkmateable positions
        #region
        /*private void check()
        {
            int[] kn1 = new int[8] { -1, -1, 1, 1, -2, 2, -2, 2 };
            int[] kn2 = new int[8] { -2, 2, -2, 2, -1, -1, 1, 1 };

            int counterW = 0;
            int counterB = 0;

            int g = 2;
            int h = 1;
            int k = 1;
            int y = 1;

            int moves = 0;

            for(int i = 0; i <= 7; i++)
            {
                for(int j = 0; j<=7; j++)
                {
                    string search = col[j] + "" + rows[i];
                    TextBlock clear = (TextBlock)FindName(search);

                    if (clear.Text == "Kn")
                    {
                        if(((SolidColorBrush)clear.Foreground).Color == Colors.White)
                        {
                            whiteCheck[counterW] = search;
                            for (g = 0; g <= 7; g++)
                            {
                                search = col[i + kn2[g]] + "" + rows[i + kn1[g]];
                                clear = (TextBlock)FindName(search);

                                if (clear.Text == ".")
                                {
                                    counterW++;
                                    whiteCheck[counterW] = search;
                                }
                            }
                                while(clear.Text != ".")
                                {
                                    search = rows[i+g] + "" + col[j+h];
                                    clear = (TextBlock)FindName(search);

                                    if(clear.Text != ".")
                                    {
                                        counterW++;
                                        whiteCheck[counterW] = search;
                                    }

                            search = rows[i + g] + "" + col[j + h];
                            clear = (TextBlock)FindName(search); }
                        }
                        if (((SolidColorBrush)clear.Foreground).Color == Colors.Black)
                        {
                            blackCheck[counterB] = search;
                            for (g = 0; g <= 7; g++)
                            {
                                search = col[i + kn2[g]] + "" + rows[i + kn1[g]];
                                clear = (TextBlock)FindName(search);

                                if (clear.Text == ".")
                                {
                                    counterB++;
                                    blackCheck[counterB] = search;
                                }
                            }
                            /* while(clear.Text != ".")
                                {
                                    search = rows[i+g] + "" + col[j+h];
                                    clear = (TextBlock)FindName(search);

                                    if(clear.Text != ".")
                                    {
                                        counterW++;
                                        whiteCheck[counterW] = search;
                                    }
                                }
                            search = col[j + h] + "" + rows[i + g];
                            clear = (TextBlock)FindName(search);
                        }
                    }
                    else if(clear.Text == "B")
                    {
                        g = 1;
                        h = 1;
                        k = 1;
                        y = 1;

                        if (((SolidColorBrush)clear.Foreground).Color == Colors.White)
                        {
                            while(clear.Text == "B" || clear.Text == ".")
                            {                               
                                search = col[i + g] + "" + rows[i + g];
                                clear = (TextBlock)FindName(search);
                                g++;

                                if (clear.Text == ".")
                                {
                                    counterW++;
                                    whiteCheck[counterW] = search;
                                }
                            }
                            while (clear.Text == "B" || clear.Text == ".")
                            {
                                search = col[i + h] + "" + rows[i - h];
                                clear = (TextBlock)FindName(search);
                                h++;

                                if (clear.Text == ".")
                                {
                                    counterW++;
                                    whiteCheck[counterW] = search;
                                }
                            }
                            while (clear.Text == "B" || clear.Text == ".")
                            {
                                search = col[i - k] + "" + rows[i + k];
                                clear = (TextBlock)FindName(search);
                                k++;

                                if (clear.Text == ".")
                                {
                                    counterW++;
                                    whiteCheck[counterW] = search;
                                }
                            }
                            while (clear.Text == "B" || clear.Text == ".")
                            {
                                search = col[i - y] + "" + rows[i - y];
                                clear = (TextBlock)FindName(search);
                                y++;

                                if (clear.Text == ".")
                                {
                                    counterW++;
                                    whiteCheck[counterW] = search;
                                }
                            }
                        }
                        else if (((SolidColorBrush)clear.Foreground).Color == Colors.Black)
                        {
                            while (clear.Text == "B" || clear.Text == ".")
                            {
                                search = col[i + g] + "" + rows[i + g];
                                clear = (TextBlock)FindName(search);
                                g++;

                                if (clear.Text == ".")
                                {
                                    counterB++;
                                    blackCheck[counterB] = search;
                                }
                            }
                            while (clear.Text == "B" || clear.Text == ".")
                            {
                                search = col[i + h] + "" + rows[i - h];
                                clear = (TextBlock)FindName(search);
                                h++;

                                if (clear.Text == ".")
                                {
                                    counterB++;
                                    blackCheck[counterB] = search;
                                }
                            }
                            while (clear.Text == "B" || clear.Text == ".")
                            {
                                search = col[i - k] + "" + rows[i + k];
                                clear = (TextBlock)FindName(search);
                                k++;

                                if (clear.Text == ".")
                                {
                                    counterB++;
                                    blackCheck[counterB] = search;
                                }
                            }
                            while (clear.Text == "B" || clear.Text == ".")
                            {
                                search = col[i - y] + "" + rows[i - y];
                                clear = (TextBlock)FindName(search);
                                y++;

                                if (clear.Text == ".")
                                {
                                    counterB++;
                                    blackCheck[counterB] = search;
                                }
                            }
                        }
                    }
                    else if(clear.Text == "P")
                    {
                        if (((SolidColorBrush)clear.Foreground).Color == Colors.White)
                        {
                            search = col[i + 1] + "" + rows[i - 1];
                            clear = (TextBlock)FindName(search);

                            if (clear.Text == ".")
                            {
                                counterW++;
                                whiteCheck[counterW] = search;
                            }

                            search = col[i - 1] + "" + rows[i - 1];
                            clear = (TextBlock)FindName(search);

                            if (clear.Text == ".")
                            {
                                counterW++;
                                whiteCheck[counterW] = search;
                            }
                        }
                        else if (((SolidColorBrush)clear.Foreground).Color == Colors.Black)
                        {
                            search = col[i + g] + "" + rows[i + g];
                            clear = (TextBlock)FindName(search);

                            if (clear.Text == ".")
                            {
                                counterB++;
                                blackCheck[counterB] = search;
                            }

                            search = col[i - g] + "" + rows[i + g];
                            clear = (TextBlock)FindName(search);

                            if (clear.Text == ".")
                            {
                                counterB++;
                                blackCheck[counterB] = search;
                            }
                        }
                    }
                    else if (clear.Text == "Q")
                    {
                        if (((SolidColorBrush)clear.Foreground).Color == Colors.White)
                        {
                            while (clear.Text == "Q" || clear.Text == ".")
                            {
                                search = col[i + g] + "" + rows[i + g];
                                clear = (TextBlock)FindName(search);
                                g++;

                                if (clear.Text == ".")
                                {
                                    counterW++;
                                    whiteCheck[counterW] = search;
                                }
                            }
                            while (clear.Text == "Q" || clear.Text == ".")
                            {
                                search = col[i + h] + "" + rows[i - h];
                                clear = (TextBlock)FindName(search);
                                h++;

                                if (clear.Text == ".")
                                {
                                    counterW++;
                                    whiteCheck[counterW] = search;
                                }
                            }
                            while (clear.Text == "Q" || clear.Text == ".")
                            {
                                search = col[i - k] + "" +rows[i + k];
                                clear = (TextBlock)FindName(search);
                                k++;

                                if (clear.Text == ".")
                                {
                                    counterW++;
                                    whiteCheck[counterW] = search;
                                }
                            }
                            while (clear.Text == "Q" || clear.Text == ".")
                            {
                                search = col[i - y] + "" + rows[i - y];
                                clear = (TextBlock)FindName(search);
                                y++;

                                if (clear.Text == ".")
                                {
                                    counterW++;
                                    whiteCheck[counterW] = search;
                                }
                            }

                            g = 1;
                            h = 1;
                            k = 1;
                            y = 1;

                            while (clear.Text == "Q" || clear.Text == ".")
                            {
                                search = col[i] + "" + rows[i + g];
                                clear = (TextBlock)FindName(search);
                                g++;

                                if (clear.Text == ".")
                                {
                                    counterW++;
                                    whiteCheck[counterW] = search;
                                }
                            }
                            while (clear.Text == "Q" || clear.Text == ".")
                            {
                                search = col[i] + "" + rows[i + h];
                                clear = (TextBlock)FindName(search);
                                h++;

                                if (clear.Text == ".")
                                {
                                    counterW++;
                                    whiteCheck[counterW] = search;
                                }
                            }
                            while (clear.Text == "Q" || clear.Text == ".")
                            {
                                search = col[i + k] + "" + rows[i];
                                clear = (TextBlock)FindName(search);
                                k++;

                                if (clear.Text == ".")
                                {
                                    counterW++;
                                    whiteCheck[counterW] = search;
                                }
                            }
                            while (clear.Text == "Q" || clear.Text == ".")
                            {
                                search = col[i + y] + "" + rows[i];
                                clear = (TextBlock)FindName(search);
                                y++;

                                if (clear.Text == ".")
                                {
                                    counterW++;
                                    whiteCheck[counterW] = search;
                                }
                            }
                        }
                        else if (((SolidColorBrush)clear.Foreground).Color == Colors.Black)
                        {
                            while (clear.Text == "Q" || clear.Text == ".")
                            {
                                search = col[i + g] + "" + rows[i + g];
                                clear = (TextBlock)FindName(search);
                                g++;

                                if (clear.Text == ".")
                                {
                                    counterB++;
                                    blackCheck[counterB] = search;
                                }
                            }
                            while (clear.Text == "Q" || clear.Text == ".")
                            {
                                search = col[i + h] + "" + rows[i - h];
                                clear = (TextBlock)FindName(search);
                                h++;

                                if (clear.Text == ".")
                                {
                                    counterB++;
                                    blackCheck[counterB] = search;
                                }
                            }
                            while (clear.Text == "Q" || clear.Text == ".")
                            {
                                search = col[i - k] + "" + rows[i + k];
                                clear = (TextBlock)FindName(search);
                                k++;

                                if (clear.Text == ".")
                                {
                                    counterB++;
                                    blackCheck[counterB] = search;
                                }
                            }
                            while (clear.Text == "Q" || clear.Text == ".")
                            {
                                search = col[i - y] + "" + rows[i - y];
                                clear = (TextBlock)FindName(search);
                                y++;

                                if (clear.Text == ".")
                                {
                                    counterB++;
                                    blackCheck[counterB] = search;
                                }
                            }

                            g = 1;
                            h = 1;
                            k = 1;
                            y = 1;

                            while (clear.Text == "Q" || clear.Text == ".")
                            {
                                search = col[i] + "" + rows[i + g];
                                clear = (TextBlock)FindName(search);
                                g++;

                                if (clear.Text == ".")
                                {
                                    counterB++;
                                    blackCheck[counterB] = search;
                                }
                            }
                            while (clear.Text == "Q" || clear.Text == ".")
                            {
                                search = col[i] + "" + rows[i + h];
                                clear = (TextBlock)FindName(search);
                                h++;

                                if (clear.Text == ".")
                                {
                                    counterB++;
                                    blackCheck[counterB] = search;
                                }
                            }
                            while (clear.Text == "Q" || clear.Text == ".")
                            {
                                search = col[i + k] + "" + rows[i];
                                clear = (TextBlock)FindName(search);
                                k++;

                                if (clear.Text == ".")
                                {
                                    counterB++;
                                    blackCheck[counterB] = search;
                                }
                            }
                            while (clear.Text == "Q" || clear.Text == ".")
                            {
                                search = col[i + y] + "" + rows[i];
                                clear = (TextBlock)FindName(search);
                                y++;

                                if (clear.Text == ".")
                                {
                                    counterB++;
                                    blackCheck[counterB] = search;
                                }
                            }
                        }
                    }
                    else if (clear.Text == "C")
                    {
                        if (((SolidColorBrush)clear.Foreground).Color == Colors.White)
                        {

                        }
                        else if (((SolidColorBrush)clear.Foreground).Color == Colors.Black)
                        {
                            while (clear.Text == "C" || clear.Text == ".")
                            {
                                search = col[i] + "" + rows[i + 1];
                                clear = (TextBlock)FindName(search);
                                y++;

                                if (clear.Text == ".")
                                {
                                    counterB++;
                                    blackCheck[counterB] = search;
                                }
                            }
                            while (clear.Text == "C" || clear.Text == ".")
                            {
                                search = col[i] + "" + rows[i + 1];
                                clear = (TextBlock)FindName(search);
                                y++;

                                if (clear.Text == ".")
                                {
                                    counterB++;
                                    blackCheck[counterB] = search;
                                }
                            }
                            while (clear.Text == "C" || clear.Text == ".")
                            {
                                search = col[i + 1] + "" + rows[i];
                                clear = (TextBlock)FindName(search);
                                y++;

                                if (clear.Text == ".")
                                {
                                    counterB++;
                                    blackCheck[counterB] = search;
                                }
                            }
                            while (clear.Text == "C" || clear.Text == ".")
                            {
                                search = col[i - 1] + "" + rows[i];
                                clear = (TextBlock)FindName(search);
                                y++;

                                if (clear.Text == ".")
                                {
                                    counterB++;
                                    blackCheck[counterB] = search;
                                }
                            }
                        }
                    }
                    else if (clear.Text == "K")
                    {
                        if (((SolidColorBrush)clear.Foreground).Color == Colors.White)
                        {

                        }
                        else if (((SolidColorBrush)clear.Foreground).Color == Colors.Black)
                        {

                        }
                    }
                }
            }
        }*/
        #endregion
    }
}