using HUCE_DALATUD_LOPNV90_2025_0099266.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HUCE_DALATUD_LOPNV90_2025_0099266.ViewModels
{
    internal class MainWindowViewModels
    {
        public ICommand OpenRenameFamiliesCommand { get; set; }
        public ICommand OpenRenameTypesFamilyCommand { get; set; }
        public ICommand OpenRenameViewsCommand { get; set; }
        public ICommand OpenRenameLevelsCommand { get; set; }

        public MainWindowViewModels()
        {
            OpenRenameFamiliesCommand = new RelayCommand(OpenRenameFamilies);
            OpenRenameTypesFamilyCommand = new RelayCommand(OpenRenameTypesFamily);
            OpenRenameViewsCommand = new RelayCommand(OpenRenameViews);
            OpenRenameLevelsCommand = new RelayCommand(OpenRenameLevels);
        }

        private void OpenRenameFamilies()
        {
            var renameFamiliesWindow = new ReNameFamilies();
            renameFamiliesWindow.Show();
        }

        private void OpenRenameTypesFamily()
        {
            var renameTypesFamilyWindow = new ReNameFamilyTypes();
            renameTypesFamilyWindow.Show();
        }

        private void OpenRenameViews()
        {
           var renameViewsWindow = new ReNameViews();
           renameViewsWindow.Show();
       }

        private void OpenRenameLevels()
        {
            var renameLevelsWindow = new ReNameLevels();
            renameLevelsWindow.Show();
        }
    }
}

