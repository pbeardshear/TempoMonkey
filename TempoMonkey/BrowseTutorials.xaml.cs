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
        int sizeOfBox = 150;
        List<box> Boxes = new List<box>();
        int gridRows, gridCols;
        NavigationButton backButton;

        public BrowseTutorials()
        {
            InitializeComponent();
            addGrid();
            addItemsToGrid(@"Tutorials", "*.m4v");

            backButton = new NavigationButton(BackButton, delegate(){
                return MainWindow.soloPage;
            });
        }

        #region Grid stuff
        /* Creates a grid dyanmically with demensions equal to (height/100) by (width/100) */
        private void addGrid()
        {
            int sizeOfCell = sizeOfBox + sizeOfBox * 1 / 5;
            gridRows = (int)selectionGallary.Height / sizeOfCell;
            gridCols = (int)selectionGallary.Width / sizeOfCell;

            for (int i = 0; i < gridCols; i += 1)
            {
                ColumnDefinition row = new ColumnDefinition();
                row.Width = new System.Windows.GridLength(sizeOfCell);
                selectionGallary.ColumnDefinitions.Add(row);
            }

            for (int j = 0; j < gridRows; j += 1)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new System.Windows.GridLength(sizeOfCell);
                selectionGallary.RowDefinitions.Add(row);
            }
        }

        private void addItemsToGrid(string path, string extenstion)
        {
            int index = 0;
            foreach (string filepath in Directory.GetFiles(path, extenstion))
            {
                int colspot = index % gridCols;
                int rowspot = index / gridCols;
                string filename = System.IO.Path.GetFileNameWithoutExtension(filepath);
                addToBox(filename, filepath, rowspot, colspot, index);
                index += 1;
            }
            selectionGallary.UpdateLayout();
        }

        private void addToBox(string name, string address, int rowspot, int colspot, int index) // instantiate a box instance
        {
            box littleBox = new box(sizeOfBox);

            littleBox.index = index;
            littleBox.MouseEnter += Mouse_Enter;
            littleBox.MouseLeave += Mouse_Leave;

            littleBox.boxName = name;
            littleBox.address = address;
            littleBox.name = name;
            string path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Images\\Tutorial_Art\\" + name + "_tutorial.png";
            littleBox.setImage(path);

            Grid.SetRow(littleBox, rowspot);
            Grid.SetColumn(littleBox, colspot);
            selectionGallary.Children.Add(littleBox);
        }
        #endregion

        public void Click()
        {
            box currentlySelectedBox = (box)MainWindow.currentlySelectedObject;
            (MainWindow.tutorPage as TutorMode).initTutor(currentlySelectedBox.index);
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
        #endregion

    }
}
