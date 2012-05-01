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
			soloButton = new NavigationButton(SoloButton, delegate()
			{
                return MainWindow.soloPage;
			});
			buddyButton = new NavigationButton(BuddyButton, delegate()
			{
                ((BrowseMusic)MainWindow.browseMusicPage).initBrowseMusic("Buddy");
                return MainWindow.browseMusicPage;
  			});
        }

        #region Button Handlers

        #endregion
		private void SoloButton_MouseEnter(object sender, MouseEventArgs e)
		{
			SoloBackground.Visibility = Visibility.Visible;
		}

		private void SoloButton_MouseLeave(object sender, MouseEventArgs e)
		{
			SoloBackground.Visibility = Visibility.Hidden;
		}

		private void BuddyButton_MouseEnter(object sender, MouseEventArgs e)
		{
			BuddyBackground.Visibility = Visibility.Visible;
		}

		private void BuddyButton_MouseLeave(object sender, MouseEventArgs e)
		{
			BuddyBackground.Visibility = Visibility.Hidden;
		}
    }
}
