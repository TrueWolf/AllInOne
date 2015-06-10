using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace TeamCityRadiator
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void ChangedEventHandler(object sender, EventArgs e);

        private List<ProjectInfo> _projectsData;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectToServer(object sender, RoutedEventArgs e)
        {
            string projectsRestPath = AddressBox.Text + @"/guestAuth/app/rest/projects";
            string buildTypesRestPath = AddressBox.Text + @"/guestAuth/app/rest/buildTypes";
            FillControlItemsViaRestCall(projectsRestPath, ProjectsListBox, "project");
            FillControlItemsViaRestCall(buildTypesRestPath, BuildTypesListBox, "buildType");
        }

        private void FillControlItemsViaRestCall(string uri, ItemsControl control, string descendandsToFetch)
        {
            XDocument xmlDoc = HttpGetRequest(uri);
            _projectsData =
                xmlDoc.Descendants(descendandsToFetch)
                    .Select(bt => new ProjectInfo(bt.Attribute("id").Value, bt.Attribute("href").Value, false))
                    .ToList();
            control.ItemsSource = _projectsData;
        }

        private static XDocument HttpGetRequest(string uri)
        {
            var request = (HttpWebRequest) WebRequest.Create(new Uri(uri));

            request.Method = "GET";
            var xmlDoc = new XDocument();
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    xmlDoc = XDocument.Load(response.GetResponseStream());
                }
            }
            catch (WebException e)
            {
                MessageBox.Show(e.Message);
            }
            return xmlDoc;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var button = (sender as Button);
            button.IsEnabled = false;
            if (button.Name.Equals("ApplyChangesProjects"))
            {
                foreach (ProjectInfo item in ProjectsListBox.Items)
                {
                    if (item.IsSelected == true)
                    {

                    }
                }
            }
            else if (button.Name.Equals("ApplyChangesBTypes"))
            {
                SelectedBuildTypesListBox.Items.Clear();
                foreach (ProjectInfo item in BuildTypesListBox.Items)
                {
                    if (item.IsSelected)
                    {
                        string buildsUrl = AddressBox.Text + item.WebUrl + "/builds";
                        XDocument responseXml = HttpGetRequest(buildsUrl);
                        XElement lastBuild = responseXml.Descendants("build").FirstOrDefault();
                        var label = new Label();
                        label.Margin = new Thickness(10);
                        label.Foreground = Brushes.AliceBlue;
                        if (lastBuild == null)
                        {
                            label.Content = item.Name+": No builds yet runned for this configuration.";
                            label.Background = Brushes.Gray;
                        }
                        else
                        {
                            label.Content = item.Name + " " + lastBuild.Attribute("number") + ": " + lastBuild.Attribute("status");
                            label.Background = lastBuild.Attribute("status").Value == "SUCCESS" ? Brushes.Green : Brushes.Red;
                        }
                        SelectedBuildTypesListBox.Items.Add(label);
                    }
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Handle(sender as CheckBox);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Handle(sender as CheckBox);
        }

        private void Handle(CheckBox checkBox)
        {
            if (ProjectsListBox.Items.Contains(checkBox.DataContext))
                ApplyChangesProjects.IsEnabled = true;
            else ApplyChangesBTypes.IsEnabled = true;
        }

        public class ProjectInfo
        {
            public ProjectInfo()
            {
            }

            public ProjectInfo(string name, string webUrl, bool isSelected) : this()
            {
                Name = name;
                WebUrl = webUrl;
                IsSelected = isSelected;
            }

            public string Name { get; set; }
            public string WebUrl { get; set; }
            public bool IsSelected { get; set; }
        }
    }
}