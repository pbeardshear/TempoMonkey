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
        int sizeOfBox = 100;
        List<box> Boxes = new List<box>();
        List<box> mySelections = new List<box>();

        Grid myGrid;
        int gridRows, gridCols;

        public BrowseMusic(string type)
        {
            InitializeComponent();
            _type = type;
            MainWindow.changeFonts(mainCanvas);
            addGrid((int)MainWindow.height, (int)MainWindow.width);
            addItemsToGrid();
        }

        /* Creates a grid dyanmically with demensions equal to (height/100) by (width/100) */
        private void addGrid(int height, int width)
        {
            myGrid = new Grid();

            int sizeofCell = sizeOfBox + sizeOfBox / 5;
            int heightOffSet = 120;
            int widthOffSet = 70;
            gridRows = (height - heightOffSet) / sizeofCell;
            gridCols = (width - widthOffSet) / sizeofCell;

            for (int i = 0; i < gridCols; i += 1)
            {
                ColumnDefinition row = new ColumnDefinition();
                row.Width = new System.Windows.GridLength(sizeofCell + sizeofCell / 3);
                myGrid.ColumnDefinitions.Add(row);
            }

            for (int j = 0; j < gridRows; j += 1)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new System.Windows.GridLength(sizeofCell + sizeofCell / 3);
                myGrid.RowDefinitions.Add(row);
            }

            mainCanvas.Children.Add(myGrid);
            Canvas.SetLeft(myGrid, widthOffSet);
            Canvas.SetTop(myGrid, heightOffSet);
        }

        private void addItemsToGrid()
        {
            string path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Music";
            int index = 0;
            foreach (string filepath in Directory.GetFiles(path, "*.mp3"))
            {
                int colspot = index % gridRows;
                int rowspot = index / gridRows;
                string filename = System.IO.Path.GetFileNameWithoutExtension(filepath);

                addToBox(filename, filepath, rowspot, colspot);
                index += 1;
            }
            myGrid.UpdateLayout();
        }

        private void addToBox(string name, string address, int rowspot, int colspot) // instantiate a box instance
        {
            box littleBox = new box(sizeOfBox, this);

            littleBox.MouseEnter += Mouse_Enter;
            littleBox.MouseLeave += Mouse_Leave;

            littleBox.boxName = name;
            littleBox.address = address;
            littleBox.name = name;
            littleBox.setImage(name);

            Grid.SetRow(littleBox, rowspot);
            Grid.SetColumn(littleBox, colspot);
            myGrid.Children.Add(littleBox);
            Boxes.Add(littleBox);
        }

        public void Click()
        {
            box currentlySelectedBox = (box)MainWindow.currentlySelectedObject;
            if (!mySelections.Contains(currentlySelectedBox))
            {
                currentlySelectedBox.highlightBox();
                mySelections.Add(currentlySelectedBox);
            }
            else
            {
                currentlySelectedBox.unHighlightBox();
                mySelections.Remove(currentlySelectedBox);
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
                MainWindow.currentPage = new FreeFormMode(musicAddrList, musicList, "Interactive");
                MainWindow.isManipulating = true;
            }
            else if (_type == "Free")
            {
                MainWindow.currentPage = new FreeFormMode(musicAddrList, musicList, "FreeForm");
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
