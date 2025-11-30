using Autodesk.Revit.Creation;
using HUCE_DALATUD_LOPNV90_2025_0099266;
using HUCE_DALATUD_LOPNV90_2025_0099266.ViewModels;
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

namespace HUCE_DALATUD_LOPNV90_2025_0099266.Views
{
    /// <summary>
    /// Interaction logic for MAINWindow.xaml
    /// </summary>
    public partial class MAINWindow : Window
    {
        public MAINWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModels();

        }
        private Document _doc;
        public MAINWindow(Document doc)
        {
            InitializeComponent();
            _doc = doc;
        }
        private void RenameFamilies_Click(object sender, RoutedEventArgs e)
        {
            var vm = new CommandFamilies(_doc);
            vm.LoadRevitDataCommand.Execute(null);

            var win = new ReNameFamilies
            {
                DataContext = vm
            };
            win.ShowDialog();
        }


    }

}
