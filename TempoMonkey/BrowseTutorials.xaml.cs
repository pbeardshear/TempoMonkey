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
using System.Windows.Navigation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using slidingMenu;
using System.IO;
using System.Windows.Media.Animation;
using System.Collections;

namespace TempoMonkey
{
    /// <summary>
    /// Interaction logic for BrowseTutorials.xaml
    /// </summary>
    public partial class BrowseTutorials : Page, SelectionPage
    {
        int sizeOfBox = 100;
        List<box> Boxes = new List<box>();
        string mySelection;
        Grid myGrid;
        int gridRows, gridCols;
        NavigationButton backButton;

        public BrowseTutorials()
        {
            InitializeComponent();
            addGrid((int)MainWindow.height, (int)MainWindow.width);
            addItemsToGrid();

            backButton = new NavigationButton(BackButton, delegate(){
                return MainWindow.soloPage;
            });


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
            int index = 0;
            foreach (string filepath in Directory.GetFiles(@"Tutorials", "*.m4v"))
            {
                int colspot = index % gridRows;
                int rowspot = index / gridRows;
                string filename = System.IO.Path.GetFileNameWithoutExtension(filepath);
                addToBox(filename, filepath, rowspot, colspot, index);
                index += 1;
            }
            myGrid.UpdateLayout();
        }

        private void addToBox(string name, string address, int rowspot, int colspot, int index) // instantiate a box instance
        {
            box littleBox = new box(sizeOfBox, this);

            littleBox.index = index;
            littleBox.MouseEnter += Mouse_Enter;
            littleBox.MouseLeave += Mouse_Leave;

            littleBox.boxName = name;
            littleBox.address = address;
            littleBox.name = name;

            string path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Images\\Tutorial_Art\\" + name + ".jpg";
            littleBox.setImage(path);

            Grid.SetRow(littleBox, rowspot);
            Grid.SetColumn(littleBox, colspot);
            myGrid.Children.Add(littleBox);
            Boxes.Add(littleBox);
        }

        public void Click()
        {
            box currentlySelectedBox = (box)MainWindow.currentlySelectedObject;
            (MainWindow.tutorPage as TutorMode).initTutorials(currentlySelectedBox.index);
            MainWindow.currentPage = MainWindow.tutorPage;
            NavigationService.Navigate(MainWindow.currentPage);
        }

        #region Button Handlers
        void Mouse_Enter(object sender, MouseEventArgs e)
        {
            MainWindow.Mouse_Enter(sender, e);
        }

        void Mouse_Leave(object sender, MouseEventArgs e)
        {
            MainWindow.Mouse_Leave(sender, e);
        }

        private void Back_Enter(object sender, MouseEventArgs e)
        {
            MainWindow.MouseEnter(backButton);
        }

        #endregion



    }
}
