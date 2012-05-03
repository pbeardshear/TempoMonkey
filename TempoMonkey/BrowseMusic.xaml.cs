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
    public partial class BrowseMusic : Page, SelectionPage
    {

        string _type;
        int sizeOfBox = 160;
		int numBoxes = 5;
        List<box> mySelections = new List<box>();
        int gridRows, gridCols;

		NavigationButton backButton, doneButton;

        public void initBrowseMusic(string type)
        {
            _type = type;
        }

        public BrowseMusic()
        {
            InitializeComponent();
            addGrid();
            addItemsToGrid(@"..\..\Resources\Music", "*.mp3");
			// Create navigation buttons
			backButton = new NavigationButton(BackButton, delegate()
			{
                foreach (box selection in mySelections)
                {
                    selection.unHighlightBox();
                }
                mySelections = new List<box>();
                Message.Content = "";
                if (_type == "Buddy")
                    return MainWindow.homePage;
                else//if (_type == "Solo")
                    return MainWindow.soloPage;
			});
            
            doneButton = new NavigationButton(DoneButtonBackground, delegate(){
                if (mySelections.Count == 0)
                {
                    Message.Content = "You have to pick at least one song!";
                    return null;
                } else 
                {
                    ArrayList musicAddrList = new ArrayList();
                    ArrayList musicList = new ArrayList();

                    foreach (box selection in mySelections)
                    {
                        musicAddrList.Add(selection.address);
                        musicList.Add(selection.name);
                    }

                    if (_type == "Buddy")
                    {
                        ((FreeFormMode)MainWindow.freeFormPage).initBuddyForm( 
                            ((string)musicAddrList[0]), 
                            ((string)musicList[0]));
                        MainWindow.setManipulating(true);
                    }

                    else if (_type == "Solo")
                    {
                        ((FreeFormMode)MainWindow.freeFormPage).initSoloForm(musicAddrList, musicList);
                        MainWindow.setManipulating(true);
                    }

                    else
                    {
                        throw new Exception();
                    }
                    tearDown();
                    return MainWindow.freeFormPage;
                }
            });
        }

        public void tearDown()
        {
            foreach (box selection in mySelections)
            {
                selection.unHighlightBox();
            }
            mySelections = new List<box>();
        }

        #region Grid stuff
        private void addGrid()
        {

			int sizeOfCell = (int)selectionGallary.Width / numBoxes;
            gridRows = (int)selectionGallary.Height / sizeOfCell;
            gridCols = (int)selectionGallary.Width / sizeOfCell;

            for (int i = 0; i < gridCols; i += 1)
            {
                ColumnDefinition row = new ColumnDefinition();
                row.Width = new System.Windows.GridLength(200);
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
                addToBox(filename, filepath, rowspot, colspot);
                index += 1;
            }
            selectionGallary.UpdateLayout();
        }

        private void addToBox(string name, string address, int rowspot, int colspot) // instantiate a box instance
        {
            box littleBox = new box(sizeOfBox);

            littleBox.MouseEnter += Mouse_Enter;
            littleBox.MouseLeave += Mouse_Leave;

            littleBox.boxName = name;
            littleBox.address = address;
            littleBox.name = name;
            string path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Images\\Album_Art\\" + name + ".jpg";
            littleBox.setImage(path);

            Grid.SetRow(littleBox, rowspot);
            Grid.SetColumn(littleBox, colspot);
            selectionGallary.Children.Add(littleBox);
        }
        #endregion

        public void unSelectBox(box Box)
        {
            Box.unHighlightBox();
            mySelections.Remove(Box);
        }

        public void SelectBox(box Box)
        {
            Box.highlightBox();
            bool tooMuch = (_type == "Solo" && mySelections.Count >= 3) || (_type == "Buddy" && mySelections.Count >= 1);
            if (tooMuch)
            {
                unSelectBox(mySelections[0]);
                mySelections.Add(Box);
            }
            else
            {
                mySelections.Add(Box);
            }
        }

        public void Click()
        {
            box currentlySelectedBox = (box)MainWindow.currentlySelectedObject;
            if (!mySelections.Contains(currentlySelectedBox))
            {
                SelectBox(currentlySelectedBox);
            }
            else
            {
                unSelectBox(currentlySelectedBox);
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

        private void DoneButton_MouseLeave(object sender, MouseEventArgs e)
        {
            DoneButtonBackground.Visibility = System.Windows.Visibility.Hidden;
        }

        private void DoneButton_MouseEnter(object sender, MouseEventArgs e)
        {
            DoneButtonBackground.Visibility = System.Windows.Visibility.Visible;
        }

        #endregion

    }
}
