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
		private Page Destination { get; set; }

		public NavigationButton(UIElement element, Page destination)
		{
			Element = element;
			Destination = destination;
		}

		public void Click()
		{
			MainWindow.currentPage = Destination;
			NavigationService.GetNavigationService(Element).Navigate(Destination);
		}
	}
}
