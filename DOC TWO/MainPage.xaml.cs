using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DOC_TWO
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private int _tabCount = 1;
        private const string YourName = "MICHEAL"; // Replace with your actual name

        public MainPage()
        {
            this.InitializeComponent();
            WebView1.Navigate(new Uri("https://www.bing.com"));
        }

        private void AddNewTab_Click(object sender, RoutedEventArgs e)
        {
            _tabCount++;
            PivotItem newTab = new PivotItem
            {
                Header = $"Tab {_tabCount} - {YourName}"
            };

            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            TextBlock nameBlock = new TextBlock
            {
                Text = YourName,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 10)
            };
            Grid.SetRow(nameBlock, 0);

            WebView newWebView = new WebView();
            newWebView.Navigate(new Uri("https://www.bing.com"));
            Grid.SetRow(newWebView, 1);

            grid.Children.Add(nameBlock);
            grid.Children.Add(newWebView);

            newTab.Content = grid;
            TabPivot.Items.Add(newTab);
            TabPivot.SelectedItem = newTab;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (TabPivot.SelectedItem is PivotItem selectedTab)
            {
                WebView webView = FindWebViewInTab(selectedTab);
                if (webView != null && webView.CanGoBack)  // Ensure CanGoBack is true
                {
                    webView.GoBack();  // Navigate back if possible
                }
            }
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (TabPivot.SelectedItem is PivotItem selectedTab)
            {
                WebView webView = FindWebViewInTab(selectedTab);
                if (webView != null && webView.CanGoForward)
                {
                    webView.GoForward();
                }
            }
        }

        private WebView FindWebViewInTab(PivotItem tab)
        {
            if (tab.Content is Grid grid)
            {
                foreach (var child in grid.Children)
                {
                    if (child is WebView webView)
                    {
                        return webView;
                    }
                }
            }
            return null;
        }

        private async void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            List<TabPreview> previews = new List<TabPreview>();

            foreach (PivotItem tab in TabPivot.Items)
            {
                WebView webView = FindWebViewInTab(tab);
                if (webView != null)
                {
                    // Create an in-memory random access stream
                    var thumbnailStream = new Windows.Storage.Streams.InMemoryRandomAccessStream();

                    // Capture the preview to the stream
                    await webView.CapturePreviewToStreamAsync(thumbnailStream);

                    // Create a BitmapImage from the stream
                    BitmapImage bitmapImage = new BitmapImage();
                    thumbnailStream.Seek(0);  // Reset stream position before reading
                    await bitmapImage.SetSourceAsync(thumbnailStream);

                    previews.Add(new TabPreview
                    {
                        Header = tab.Header.ToString(),
                        ThumbnailSource = bitmapImage
                    });
                }
            }

            PreviewList.ItemsSource = previews;
            PreviewPopup.IsOpen = true;
        }


        private void PreviewList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PreviewList.SelectedIndex != -1)
            {
                TabPivot.SelectedIndex = PreviewList.SelectedIndex;
                PreviewPopup.IsOpen = false;
            }
        }

        private void ClosePreview_Click(object sender, RoutedEventArgs e)
        {
            PreviewPopup.IsOpen = true;
        }
    }

    public class TabPreview
    {
        public string Header { get; set; }
        public BitmapImage ThumbnailSource { get; set; }
    }



}
