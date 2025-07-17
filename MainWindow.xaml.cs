using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.Windows.Media; 

namespace MiNavegadorWeb
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AddInitialTab(); 
        }

        private void AddInitialTab()
        {
           
            TabItem newTab = CreateNewTab("https://www.bing.com");
            MainTabControl.Items.Add(newTab);
            MainTabControl.SelectedItem = newTab; 
        }

        private TabItem CreateNewTab(string url = "about:blank")
        {
            TabItem tab = new TabItem();
            tab.Header = "Nueva Pestaña"; 

         
            Grid tabContentGrid = new Grid();
            tabContentGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            tabContentGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

         
            StackPanel tabNavBar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
            Button backButton = new Button { Content = "←", Width = 30, Margin = new Thickness(0, 0, 5, 0) };
            Button forwardButton = new Button { Content = "→", Width = 30, Margin = new Thickness(0, 0, 5, 0) };
            TextBox tabUrlTextBox = new TextBox { Width = 300, VerticalContentAlignment = VerticalAlignment.Center };
            Button tabGoButton = new Button { Content = "Ir", Margin = new Thickness(5, 0, 0, 0) };
            Button closeTabButton = new Button { Content = "X", Margin = new Thickness(5,0,0,0), ToolTip = "Cerrar Pestaña" };

            tabNavBar.Children.Add(backButton);
            tabNavBar.Children.Add(forwardButton);
            tabNavBar.Children.Add(tabUrlTextBox);
            tabNavBar.Children.Add(tabGoButton);
            tabNavBar.Children.Add(closeTabButton);

            Grid.SetRow(tabNavBar, 0);
            tabContentGrid.Children.Add(tabNavBar);

            WebBrowser webBrowser = new WebBrowser();
            Grid.SetRow(webBrowser, 1);
            tabContentGrid.Children.Add(webBrowser);

            tab.Content = tabContentGrid;

            
            webBrowser.Navigated += (s, e) =>
            {
                tabUrlTextBox.Text = e.Uri?.AbsoluteUri ?? "about:blank"; 

                
                string? uriString = e.Uri?.AbsoluteUri;
                if (!string.IsNullOrEmpty(uriString))
                {
                  
                    string[] parts = uriString.Split('/');
                    string potentialHeader = parts[^1]; 
                    
                    if (potentialHeader.Length > 0 && !potentialHeader.Contains(".") && potentialHeader.Length < 30)
                    {
                        tab.Header = potentialHeader;
                    }
                    else if (uriString.Contains("google.com"))
                    {
                        tab.Header = "Google"; 
                    }
                    else if (uriString.Contains("bing.com"))
                    {
                        tab.Header = "Bing";
                    }
                    else if (uriString.Equals("about:blank", StringComparison.OrdinalIgnoreCase))
                    {
                        tab.Header = "Nueva Pestaña";
                    }
                    else if (uriString.Length > 25) 
                    {
                        tab.Header = uriString.Substring(0, 25) + "...";
                    }
                    else
                    {
                        tab.Header = uriString;
                    }
                }
                else
                {
                    tab.Header = "Nueva Pestaña";
                }


                
                backButton.IsEnabled = webBrowser.CanGoBack;
                forwardButton.IsEnabled = webBrowser.CanGoForward;
            };

           

            backButton.Click += (s, e) =>
            {
                if (webBrowser.CanGoBack) webBrowser.GoBack();
            };

            forwardButton.Click += (s, e) =>
            {
                if (webBrowser.CanGoForward) webBrowser.GoForward();
            };

            tabGoButton.Click += (s, e) =>
            {
                NavigateWebBrowser(webBrowser, tabUrlTextBox.Text);
            };

            tabUrlTextBox.KeyDown += (s, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    NavigateWebBrowser(webBrowser, tabUrlTextBox.Text);
                }
            };

            closeTabButton.Click += (s, e) =>
            {
                MainTabControl.Items.Remove(tab);
            };

            
            NavigateWebBrowser(webBrowser, url);

           
            backButton.IsEnabled = webBrowser.CanGoBack;
            forwardButton.IsEnabled = webBrowser.CanGoForward;

            return tab;
        }

        private void NavigateWebBrowser(WebBrowser browser, string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                try
                {
                    Uri uri = new Uri(url.Contains("://") ? url : "http://" + url);
                    browser.Navigate(uri);
                }
                catch (UriFormatException)
                {
                    MessageBox.Show("URL inválida. Por favor, introduce una URL válida.");
                }
            }
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainTabControl.SelectedItem is TabItem currentTab)
            {
                if (currentTab.Content is Grid tabContentGrid)
                {
                    WebBrowser currentWebBrowser = (WebBrowser)tabContentGrid.Children[1];
                    StackPanel tabNavBar = tabContentGrid.Children[0] as StackPanel;
                    if (tabNavBar != null && tabNavBar.Children.Count > 2)
                    {
                        TextBox tabUrlTextBox = tabNavBar.Children[2] as TextBox;
                        if (tabUrlTextBox != null)
                        {
                            NavigateWebBrowser(currentWebBrowser, UrlTextBox.Text);
                            tabUrlTextBox.Text = UrlTextBox.Text; 
                        }
                    }
                }
            }
        }

        private void UrlTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                GoButton_Click(sender, e);
            }
        }

        private void NuevaPestana_Click(object sender, RoutedEventArgs e)
        {
            TabItem newTab = CreateNewTab("https://www.bing.com");
            MainTabControl.Items.Add(newTab);
            MainTabControl.SelectedItem = newTab;
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (MainTabControl.SelectedItem is TabItem currentTab)
            {
                if (currentTab.Content is Grid tabContentGrid)
                {
                    WebBrowser currentWebBrowser = (WebBrowser)tabContentGrid.Children[1];
                    if (currentWebBrowser.Source != null)
                    {
                        UrlTextBox.Text = currentWebBrowser.Source.AbsoluteUri;
                    }
                    else
                    {
                        UrlTextBox.Text = "";
                    }
                }
            }
        }

      
        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedColor = ((ComboBoxItem)ColorComboBox.SelectedItem!).Content.ToString()!;

            switch (selectedColor)
            {
                case "Default":
                    this.Background = SystemColors.WindowBrush;
                    break;
                case "Azul Claro":
                    this.Background = new SolidColorBrush(Colors.LightBlue);
                    break;
                case "Gris":
                    this.Background = new SolidColorBrush(Colors.LightGray);
                    break;
                case "Verde":
                    this.Background = new SolidColorBrush(Colors.LightGreen);
                    break;
                case "Naranja":
                    this.Background = new SolidColorBrush(Colors.Orange);
                    break;
                case "Morado": 
                    this.Background = new SolidColorBrush(Color.FromRgb(98, 0, 238)); 
                    break;
                default:
                    this.Background = SystemColors.WindowBrush;
                    break;
            }
        }
    }
}


