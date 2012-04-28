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
using Microsoft.Kinect;
//using System.Windows.Forms;

namespace TempoMonkey
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
		NavigationButton soloButton;
		NavigationButton buddyButton;

        public HomePage()
        {
            InitializeComponent();
			// Create some navigation buttons to wrap our images
			if (MainWindow.browseMusicPage != null)
			{
				MainWindow.browseMusicPage = new BrowseMusic("Free");
			}
			soloButton = new NavigationButton(SoloButton, delegate()
			{
				return MainWindow.browseMusicPage;
			});
			buddyButton = new NavigationButton(BuddyButton, delegate()
			{
				return null;
			});

        }

        #region Button Handlers
        private void Mouse_Enter(object sender, MouseEventArgs e)
        {
            MainWindow.Mouse_Enter(sender, e);
        }

        private void Mouse_Leave(object sender, MouseEventArgs e)
        {
            MainWindow.Mouse_Leave(sender, e);
        }

        private void learningStudioButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.currentPage = new LearningStudio();
            NavigationService.Navigate(MainWindow.currentPage);
        }

        private void browseMusic_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.currentPage = new BrowseMusic("Free");
            NavigationService.Navigate(MainWindow.currentPage);
        #endregion
        }

		private void SoloButton_MouseEnter(object sender, MouseEventArgs e)
		{
			SoloBackground.Visibility = Visibility.Visible;
			SoloButton.Opacity = 0.0;
			MainWindow.MouseEnter(soloButton);
		}

		private void SoloButton_MouseLeave(object sender, MouseEventArgs e)
		{
			SoloBackground.Visibility = Visibility.Hidden;
			SoloButton.Opacity = 1.0;
			MainWindow.Mouse_Leave(sender, e);
		}

		private void BuddyButton_MouseEnter(object sender, MouseEventArgs e)
		{
			BuddyBackground.Visibility = Visibility.Visible;
			BuddyButton.Opacity = 0.0;
			MainWindow.MouseEnter(buddyButton);
		}

		private void BuddyButton_MouseLeave(object sender, MouseEventArgs e)
		{
			BuddyBackground.Visibility = Visibility.Hidden;
			BuddyButton.Opacity = 1.0;
			MainWindow.Mouse_Leave(sender, e);
		}
    }
}
