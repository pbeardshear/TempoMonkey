using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace TempoMonkey
{
	/// <summary>
	/// Wrapper object for UI elements that don't natively support clicking
	/// Provides a consistent interface for .xaml files to hook into for page navigation
	/// </summary>
	public class NavigationButton
	{
		private UIElement Element { get; set; }
		private DestinationDelegate Destination { get; set; }

		public delegate Page DestinationDelegate();

		public NavigationButton(UIElement element, DestinationDelegate destination)
		{
			Element = element;
			Destination = destination;
		}

		public void Click()
		{
            Page nextPage = Destination();
            if (nextPage != null)
            {
                MainWindow.currentPage = nextPage;
                NavigationService.GetNavigationService(Element).Navigate(MainWindow.currentPage);
            }
		}
	}
}
