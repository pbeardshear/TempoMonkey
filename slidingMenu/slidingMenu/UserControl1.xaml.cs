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
using System.IO;
using System.Windows.Media.Animation;
using System.Collections;

namespace slidingMenu
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {

        int sizeOfBox;
        double position;
        double visibleArea;
        int numberOfItems;
        int span; // the distance to move
        double theOneChosenToHighlight;
        box currentSelectedBox;
        string folderName;

        public UserControl1()
        {
            InitializeComponent();

            sizeOfBox = 240;
            position = 0.0;
            visibleArea = 720;
            numberOfItems = 0;
            span = 240;
            theOneChosenToHighlight = position - span;
        }

        public void initializeMenu(string name)
        {
            highlightBox(theOneChosenToHighlight);
            folderName = name;
            addItemToMenu();

        }

        private void addItemToMenu() // add each item to the box( a class instance )
        {
            string path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            path = path + "\\" + folderName; //folderBrowser.SelectedPath;

            double pos = 0.0;
            foreach (string filePath in Directory.GetFiles(path, "*.*"))
            {
                String fName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                addToBox(fName, filePath, pos);
                numberOfItems += 1;
                pos -= span;
            }
            this.MenuBarWindow.UpdateLayout();
        }

        private void addToBox(string name, string address, double pos) // instantiate a box instance
        {
            box littleBox = new box(sizeOfBox);
            littleBox.boxName = name;
            littleBox.address = address;
            littleBox.name = name;
            littleBox.position = pos;
            this.MenuBar.Children.Add(littleBox);
        }
 

        public void moveMenu(double position) // move the menu left or right
        {
            MenuBar.SetValue(Canvas.LeftProperty, position);
        }

        private void moveRight(object sender, RoutedEventArgs e)
        {
            if (position < sizeOfBox) // not out of bound
            {
                unHighlightBox(theOneChosenToHighlight);
                position += span;
                theOneChosenToHighlight += span;
                moveMenu(position);
                highlightBox(theOneChosenToHighlight);
               
            }
        }

        private void moveLeft(object sender, RoutedEventArgs e)
        {
            if (((numberOfItems * span) + position) >= visibleArea) // not out of bound
            {
                unHighlightBox(theOneChosenToHighlight);
                position -= span;
                theOneChosenToHighlight -= span;
                moveMenu(position);
                highlightBox(theOneChosenToHighlight);
            }
        }
        public void highlightBox(double position)
        {
            foreach (var child in this.MenuBar.Children)
            {
                if ((position <= ((box)child).position) && (position > (((box)child).position - ((box)child).size)))
                {
                    ((box)child).highlightBox();
                    currentSelectedBox = (box)child;
                }
            }
        }
        public void unHighlightBox(double position)
        {
            foreach (var child in this.MenuBar.Children)
            {
                if ((position <= ((box)child).position) && (position > (((box)child).position - ((box)child).size)))
                {
                    ((box)child).unHighlightBox();
                }
            }
        }

        public Boolean hasCurrentSelectedBox()
        {
            if (currentSelectedBox != null)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        public string getAddress()
        {
            return currentSelectedBox.address;
        }

        public string getName()
        {
            return currentSelectedBox.name;
        }
    }
}
