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

namespace tempoMonkey
{
    /// <summary>
    /// Interaction logic for FreeFormMode.xaml
    /// </summary>
    public partial class FreeFormMode : Page
    {
        ArrayList musicAddrList;
        ArrayList musicNameList;
        string mediaAddress;
        int direction;
        Boolean isReady;

        public FreeFormMode(ArrayList addrList, ArrayList nameList)
        {
            InitializeComponent();
            musicAddrList = addrList;
            musicNameList = nameList;
            addingToMusicList(musicNameList);
            mediaAddress = (string)musicAddrList[0];
            direction = 999;
            isReady = false;
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

        //Tell the MainWindow if the cursor is on the button.
        public Boolean isSelectionReady()
        {
            return isReady;
        }

        private void addingToMusicList(ArrayList list)
        {
            foreach (string name in list)
            {
                Label myLabel = new Label();
                myLabel.Content = name;
                musicList.Children.Add(myLabel);
            }
        }


        private void Stop_MouseEnter(object sender, MouseEventArgs e)
        {
            musicPlayer.Stop();
            Play.Content = "Play";
            Play.IsEnabled = true;
            Stop.IsEnabled = false;

        }

        private void Play_MouseEnter(object sender, MouseEventArgs e)
        {
            Stop.IsEnabled = true;

            musicPlayer.Source = new Uri(mediaAddress);
            Play.IsEnabled = true;
            if (Play.Content.ToString() == "Play")
            {
                musicPlayer.Play();
                Play.Content = "Pause";
            }
            else
            {
                musicPlayer.Pause();
                Play.Content = "Play";
            }
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


    }
}
