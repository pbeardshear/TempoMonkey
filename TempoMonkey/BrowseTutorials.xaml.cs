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

namespace TempoMonkey
{
    /// <summary>
    /// Interaction logic for BrowseTutorials.xaml
    /// </summary>
    public partial class BrowseTutorials : Page
    {
        Boolean isReady;
        Boolean selectionDone;
        int direction;

        public BrowseTutorials()
        {
            InitializeComponent();
            this.slidingMenu.initializeMenu("TutorialVideos");
            isReady = false;
            direction = 999;
            selectionDone = false;
        }
        //Tell the MainWindow which menu button has been selected
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

        bool chooseReady = false;
        //Tell the MainWindow if the cursor is on the button.
        public Boolean isSelectionReady()
        {
            chooseReady = isSelectionValid();
            return isReady;
        }

        public bool isSelectionValid()
        {
            return false;
        }

        public string getAddr()
        {
            return slidingMenu.getAddress();
        }

        private void Back_MouseEnter(object sender, MouseEventArgs e)
        {
            setSelectionStatus(true);
            direction = 3;
        }

        private void Back_MouseLeave(object sender, MouseEventArgs e)
        {
            setSelectionStatus(false);
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

    }
}
