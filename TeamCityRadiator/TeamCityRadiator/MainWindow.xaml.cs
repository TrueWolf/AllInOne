using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace TeamCityRadiator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void ChangedEventHandler(object sender, EventArgs e);

        private List<ProjectInfo> _projectsData;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var projectsRestPath = AddressBox.Text + @"/guestAuth/app/rest/projects";
            GetPOSTResponse(new Uri(projectsRestPath));
        }

       public class ProjectInfo
        {
            public ProjectInfo()
            {

            }

            public ProjectInfo(string name,string webUrl,bool isSelected) : this()
            {
                Name = name;
                WebUrl = webUrl;
                IsSelected = isSelected;
            }

            public string Name {get;set;}
            public string WebUrl { get; set; }
            public bool IsSelected { get; set; }
        }

        private void GetPOSTResponse(Uri uri)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);

            request.Method = "GET";

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            XDocument xmlDoc = new XDocument();
                using (WebResponse response = request.GetResponse())
                {
                    xmlDoc = XDocument.Load(response.GetResponseStream());             
                }
                _projectsData = xmlDoc.Descendants("project").Select(p => new ProjectInfo(p.Attribute("id").Value, p.Attribute("webUrl").Value, false)).ToList();
                ProjectsListBox.ItemsSource = _projectsData;
                
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {            
            
            ApplyChangesButton.IsEnabled = false;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Handle(sender as CheckBox);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Handle(sender as CheckBox);
        }

        void Handle(CheckBox checkBox)
        {
            ApplyChangesButton.IsEnabled = true;
        }

        public void ListChanged(object sender, EventArgs e)
        {
            ApplyChangesButton.IsEnabled = true;
        }
    }
}
