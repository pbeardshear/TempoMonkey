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


namespace TempoMonkey
{
    /// <summary>
    /// Interaction logic for BrowseMusic.xaml
    /// </summary>
    public partial class BrowseMusic : Page
    {
        ArrayList musicAddrList = new ArrayList();
        ArrayList musicList = new ArrayList();
        Boolean selectionDone;
        int direction;
        Boolean isReady;
        string theItemToDelete;

        public BrowseMusic()
        {
            InitializeComponent();
            this.slidingMenu.initializeMenu("Music");
            selectionDone = false;
            direction = 999;
            isReady = false;
            DisableConfirmationCanvas();
        }

        public int getSelectedMenu()
        {
            return direction;
        }

        private void setSelectionStatus(Boolean value)
        {
            isReady = value;
            if (!isReady)
            {
                RaiseEvent(new RoutedEventArgs(MainWindow.resetTimer));
            }
        }

        bool musicChooser = false;
        public bool getMusicChoose(){
            return musicChooser;
        }
        //Tell the MainWindow if the cursor is on the button.
        public Boolean isSelectionReady()
        {
            musicChooser = isMenuSelectionValid();
            return isReady;
        }

        public bool isMenuSelectionValid() //function used when push gesture is performed
        {
            if (slidingMenu.hasCurrentSelectedBox())
            {
                foreach (var name in musicList)
                {
                    if (((string)name).Equals(slidingMenu.getName()))
                    {

                        return false;
                    }
                }
                if (musicAddrList.Count == 3)
                {
                    return false;
                }
                else
                {
                    direction = 5;
                    isReady = true;
                    return true;
                }
            }
            return false;
        }
            
        public void addingToMusicAddrList() 
        {
            musicAddrList.Add(this.slidingMenu.getAddress());
        }

        public void addingToMusicList()
        {
            Button myButton = new Button();
            myButton.Content = this.slidingMenu.getName();
            myButton.IsEnabled = false;
            myButton.Width = 214;
            myButton.Height = 46;
            myButton.FontSize = 30;
            myButton.MouseEnter += new MouseEventHandler(itemDeletionMouseEnter);
            myButton.MouseLeave += new MouseEventHandler(itemDelectionMouseLeave);
            selectedMusicList.Children.Add(myButton);
            musicList.Add(this.slidingMenu.getName());
            /*
            Label myLabel = new Label();
            myLabel.Content = this.slidingMenu.getName();
            selectedMusicList.Children.Add(myLabel);
            musicList.Add(this.slidingMenu.getName());
             * */
        }

        public void deletingMusic()
        {
            int i=0;
            if (selectedMusicList.Children.Count != 0)
            {
                foreach (var child in selectedMusicList.Children)
                {
                    
                    if (((Button)child).Content.Equals(theItemToDelete))
                    {
                        selectedMusicList.Children.RemoveAt(i);
                        musicAddrList.RemoveAt(i);
                        musicList.RemoveAt(i);
                        return;
                    }
                    i++;
                }

            }

            /*
            int i = musicList.Count - 1;            
            selectedMusicList.Children.RemoveAt(i);
            musicAddrList.RemoveAt(i);
            musicList.RemoveAt(i);
             * */
        }



        public void EnalbeConfirmationCanvas()
        {
            confirmationCanvas.Visibility = Visibility.Visible;
            back.IsEnabled = false;
            done.IsEnabled = false;
            //delele.IsEnabled = false;
            itemName.Content = theItemToDelete;
        }
        public void DisableConfirmationCanvas()
        {
            confirmationCanvas.Visibility = Visibility.Collapsed;
            back.IsEnabled = true;
            done.IsEnabled = true;
            delele.IsEnabled = true;
        }

        public void EnableButtons()
        {
            delele.IsEnabled = false;
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
        public void DisableButtons()
        {
            foreach (var child in selectedMusicList.Children)
            {
                BrushConverter converter = new BrushConverter();
                ((Button)child).IsEnabled = false;
                ((Button)child).Foreground = converter.ConvertFromString("Black") as Brush; 
            }

        }


        public Boolean isSelectionDone()
        {
            return selectionDone;
        }

        public ArrayList getMusicAddrList()
        {
            return musicAddrList;
        }

        public ArrayList getMusicList()
        {
            return musicList;
        }

        private void done_MouseEnter(object sender, MouseEventArgs e)
        {
            setSelectionStatus(true);
            selectionDone = true;
            direction = 4;
        }
        private void done_MouseLeave(object sender, MouseEventArgs e)
        {
            setSelectionStatus(false);
            selectionDone = false;
        }

        private void back_MouseEnter(object sender, MouseEventArgs e)
        {
            setSelectionStatus(true);
            direction = 3;
        }

        private void back_MouseLeave(object sender, MouseEventArgs e)
        {
            setSelectionStatus(false);           
        }

        private void delete_MouseEnter(object sender, MouseEventArgs e)
        {
            setSelectionStatus(true);
            direction = 9;
        }

        private void delete_MouseLeave(object sender, MouseEventArgs e)
        {
            setSelectionStatus(false);
        }

        private void No_MouseEnter(object sender, MouseEventArgs e)
        {
            setSelectionStatus(true);
            direction = 10;
        }

        private void No_MouseLeave(object sender, MouseEventArgs e)
        {
            setSelectionStatus(false);
        }

        private void Yes_MouseLeave(object sender, MouseEventArgs e)
        {
            setSelectionStatus(false);
        }

        private void Yes_MouseEnter(object sender, MouseEventArgs e)
        {
            setSelectionStatus(true);
            direction = 11;
        }

        private void itemDeletionMouseEnter(object sender, MouseEventArgs e)
        {
            
            foreach (var child in selectedMusicList.Children)
            {
                string name = (string)((Button)child).Content;
                if(((Button)sender).Content.Equals(name))
                {
                    theItemToDelete = name;
                }
            }
            setSelectionStatus(true);
            direction = 12;
        }

        private void itemDelectionMouseLeave(object sender, MouseEventArgs e)
        {
            setSelectionStatus(false);
        }

    }
}
