using System;
using System.Collections.Generic;
using System.Collections;
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
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;
using System.Windows.Media.Animation;
using System.Threading;
using System.Windows.Threading;

namespace TempoMonkey
{
    /// <summary>
    /// Interaction logic for TutorMode.xaml
    /// </summary>
    public partial class SoloPage : Page
    {
        NavigationButton freeButton, tutorButton, backButton;

        public SoloPage()
        {
            InitializeComponent();
            freeButton = new NavigationButton(solo_remixBackground, delegate()
            {
                ((BrowseMusic)MainWindow.browseMusicPage).initBrowseMusic("Solo");
                return MainWindow.browseMusicPage;
            });

            tutorButton = new NavigationButton(solo_learningBackground, delegate()
            {
                return MainWindow.browseTutorialsPage;
            });

            backButton = new NavigationButton(BackButton, delegate()
            {
                return MainWindow.homePage;
            });
        }
        private void TutorButton_MouseEnter(object sender, MouseEventArgs e)
        {
            solo_learningBackground.Visibility = Visibility.Visible;
        }

        private void TutorButton_MouseLeave(object sender, MouseEventArgs e)
        {
            solo_learningBackground.Visibility = Visibility.Hidden;
        }

        private void FreeButton_MouseEnter(object sender, MouseEventArgs e)
        {
            solo_remixBackground.Visibility = Visibility.Visible;
        }

        private void FreeButton_MouseLeave(object sender, MouseEventArgs e)
        {
            solo_remixBackground.Visibility = Visibility.Hidden;
        }
    }
}
