using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using ExtensionAdapter;
using Microsoft.Win32;
using Path = System.IO.Path;

namespace ModularDesktopApplication {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            this.Closed += MainWindow_Closed;
            this.Loaded += MainWindow_Loaded;

            var oneDrive = Registry.CurrentUser
                .OpenSubKey("Software\\Malomed\\MyApp");
            if (oneDrive != null) {
                var coord = ((string)oneDrive.GetValue("Coord")).Split(' ');
                double left = double.Parse(coord[0]);
                double top = double.Parse(coord[1]);

                this.Left = left;
                this.Top = top;

                var size = ((string)oneDrive.GetValue("Size")).Split(' ');
                double width = double.Parse(size[0]);
                double height = double.Parse(size[1]);

                this.Width = width;
                this.Height = height;
            }

        }

        private void MainWindow_Closed(object sender, EventArgs e) {
            var software = Registry.CurrentUser.OpenSubKey("Software", true);
            var myApp = software.CreateSubKey("Malomed\\MyApp");
            myApp.SetValue("Coord", $"{this.Left} {this.Top}");
            myApp.SetValue("Size", $"{this.Width} {this.Height}");
            myApp.Flush();
            myApp.Close();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            IEnumerable<string> dlls = null;
            var path = Path.Combine(Environment.CurrentDirectory, "ext");
            if (!Directory.Exists(path)) {
                return;
            }
            var filePath = Path.Combine(Environment.CurrentDirectory, "ext.ini");
            if (File.Exists(filePath)) {
                dlls = File.ReadAllLines(filePath).Select(file => Path.Combine(path, file));
            } else {
                dlls = Directory.EnumerateFiles(path, "*.dll");
            }

            foreach (var dll in dlls) {
                try {
                    var fileName = Path.Combine(Environment.CurrentDirectory, dll);
                    var asm = Assembly.Load(AssemblyName.GetAssemblyName(fileName));
                    var types = asm.GetTypes();
                    // multiple modules also possible
                    var moduleType = types.FirstOrDefault(type => typeof(IFunctionModule).IsAssignableFrom(type));
                    if (moduleType == null) {
                        continue;
                    }

                    Task.Run(() => {
                        var moduleObject = Activator.CreateInstance(moduleType) as IFunctionModule;
                        var info = moduleObject.GetModuleFunctionInfo();
                        Dispatcher.Invoke(() => {
                            var button = new Button {
                                Content = info.ButtonText,
                                Height = 50,
                                Width = 100,
                                Margin = new Thickness(5)
                            };
                            button.Click += (s, ee) => {
                                var text = InputTextBox.Text;
                                var result = info.Function.Invoke(text);
                                OutputTextBox.Text = result;
                            };
                            ButtonsPanel.Children.Add(button);
                        });
                    });
                } catch (Exception ex) {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e) {
            var prefs = new Preferences();
            prefs.ShowDialog();
        }
    }
}
