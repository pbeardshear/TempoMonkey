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
using System.Collections;
using System.IO;
using System.Windows.Media.Animation;
using slidingMenu;

namespace TempoMonkey
{
    /// <summary>
    /// Interaction logic for BrowseMusic.xaml
    /// </summary>
    public partial class BrowseMusic : Page
    {

        private string _type;
        int span = 240;
        int numberOfItems = 0;
        int sizeOfBox = 240;
        double position = 0;
        List<box> Boxes = new List<box>();
        List<box> mySelections = new List<box>();
        int boxIndex = 1;

        public BrowseMusic(string type)
        {
            InitializeComponent();
            _type = type;
            addItemsToMenu();
            Boxes[boxIndex].highlightBox();
        }

        private void addItemsToMenu()
        {
            string path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Music";
            double pos = 0.0;
            foreach (string filepath in Directory.GetFiles(path, "*.mp3"))
            {
                string filename = System.IO.Path.GetFileNameWithoutExtension(filepath);
                addToBox(filename, filepath, pos);
                numberOfItems += 1;
                pos -= span;
            }
        }

        private void addToBox(string name, string address, double pos) // instantiate a box instance
        {
            box littleBox = new box(sizeOfBox);
            littleBox.boxName = name;
            littleBox.address = address;
            littleBox.name = name;
            littleBox.setImage(name);
            littleBox.position = pos;
            Boxes.Add(littleBox);
            MenuBar.Children.Add(littleBox);
        }

        private void addCurrToSelection()
        {
            if (!mySelections.Contains(Boxes[boxIndex]))
            {
                Button myButton = new Button();
                myButton.Content = Boxes[boxIndex].name;
                myButton.IsEnabled = false;
                myButton.Width = 214;
                myButton.Height = 46;
                myButton.FontSize = 30;
                myButton.MouseEnter += Mouse_Enter;
                myButton.MouseLeave += Mouse_Leave;
                myButton.Click += Delete_Song;
                selectedMusicList.Children.Add(myButton);
                mySelections.Add(Boxes[boxIndex]);
            }
        }

        #region Mouse Events
        private void Mouse_Enter(object sender, MouseEventArgs e)
        {
            MainWindow.Mouse_Enter(sender, e);
        }

        private void Mouse_Leave(object sender, MouseEventArgs e)
        {
            MainWindow.Mouse_Leave(sender, e);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.currentPage = new HomePage();
            NavigationService.Navigate(MainWindow.currentPage);
        }

        private void Left_Click(object sender, RoutedEventArgs e)
        {
            if (boxIndex > 0)
            {
                ((box)Boxes[boxIndex]).unHighlightBox();
                position += span;
                moveMenu(position);
                boxIndex--;
                ((box)Boxes[boxIndex]).highlightBox();
            }
        }

        private void Right_Click(object sender, RoutedEventArgs e)
        {
            if (boxIndex < Boxes.Count() - 1)
            {
                ((box)Boxes[boxIndex]).unHighlightBox();
                position -= span;
                moveMenu(position);
                boxIndex++;
                ((box)Boxes[boxIndex]).highlightBox();
            }
        }

        bool deleting = false;
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (!deleting)
            {
                deleting = true;
                delete.Content = "Done Deleting";
                foreach (var child in selectedMusicList.Children)
                {
                    BrushConverter converter = new BrushConverter();
                    ((Button)child).IsEnabled = true;
                    ((Button)child).BorderBrush = converter.ConvertFromString("#FFE81515") as Brush;
                    ((Button)child).Background = converter.ConvertFromString("Yellow") as Brush;
                    ((Button)child).FontSize = 25.0;
                    ((Button)child).Foreground = converter.ConvertFromString("#FF9264DE") as Brush;
                }
            }
            else
            {
                deleting = false;
                delete.Content = "Delete";
                foreach (var child in selectedMusicList.Children)
                {
                    BrushConverter converter = new BrushConverter();
                    ((Button)child).IsEnabled = false;
                    ((Button)child).BorderBrush = converter.ConvertFromString("Black") as Brush;
                    ((Button)child).Background = converter.ConvertFromString("White") as Brush;
                    ((Button)child).FontSize = 25.0;
                    ((Button)child).Foreground = converter.ConvertFromString("Black") as Brush;
                }
            }
        }

        public void moveMenu(double position) // move the menu left or right
        {
            Canvas.SetLeft(MenuBar, position);
        }

        private void Choose_Click(object sender, RoutedEventArgs e)
        {
            addCurrToSelection();
        }

        private void Delete_Song(object sender, RoutedEventArgs e)
        {
            if (deleting)
            {
                int i = 0;
                string theItemToDelete = (string)((Button)sender).Content;
                if (selectedMusicList.Children.Count != 0)
                {
                    foreach (var child in selectedMusicList.Children)
                    {

                        if (((Button)child).Content.Equals(theItemToDelete))
                        {
                            selectedMusicList.Children.RemoveAt(i);
                            mySelections.RemoveAt(i);
                            MainWindow.timeOnCurrentButton = 0;
                            return;
                        }
                        i++;
                    }
                }
            }
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            if (mySelections.Count == 0)
            {
                // Maybe give feedback about this?
                return;
            }

            ArrayList musicAddrList = new ArrayList();
            ArrayList musicList = new ArrayList();

            foreach (box selection in mySelections)
            {
                musicAddrList.Add(selection.address);
                musicList.Add(selection.name);
            }

            if (_type == "Interactive")
            {
                MainWindow.currentPage = new InteractiveMode(musicAddrList, musicList);
                MainWindow.isManipulating = true;
            }
            else if (_type == "Free")
            {
                MainWindow.currentPage = new FreeFormMode(musicAddrList, musicList);
                MainWindow.isManipulating = true;
            }
            else
            {
                throw new Exception();
            }
            NavigationService.Navigate(MainWindow.currentPage);
        }

        #endregion
    }
}
