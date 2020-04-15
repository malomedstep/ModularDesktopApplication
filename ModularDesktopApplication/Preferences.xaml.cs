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
using System.Windows.Shapes;

namespace ModularDesktopApplication {
    /// <summary>
    /// Interaction logic for Preferences.xaml
    /// </summary>
    public partial class Preferences : Window {
        public Preferences() {
            InitializeComponent();

            this.ModulesList.ItemsSource = new List<ModuleInfo> {
                new ModuleInfo { Name = "ToLower.dll", Active = true},
                new ModuleInfo { Name = "ToUpper.dll", Active = false}
            };
        }
    }
}
