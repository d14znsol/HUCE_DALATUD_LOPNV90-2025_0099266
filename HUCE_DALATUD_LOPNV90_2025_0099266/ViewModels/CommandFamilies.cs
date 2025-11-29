using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace HUCE_DALATUD_LOPNV90_2025_0099266.ViewModels
{
    internal class CommandFamilies : INotifyPropertyChanged

    {
        private Document _doc;

        public ICommand LoadRevitDataCommand { get; }
      
        public CommandFamilies(Document doc)
        {
            _doc = doc ?? throw new ArgumentNullException(nameof(doc));

            LoadRevitDataCommand = new RelayCommand(LoadRevitData);
        }


        private void LoadRevitData()
        {
           

            // Lấy Family
            var families = new FilteredElementCollector(_doc).OfClass(typeof(Family)).Cast<Family>().ToList();

            // Lấy Family Type
            var types = new FilteredElementCollector(_doc).OfClass(typeof(ElementType)).Cast<ElementType>().ToList();

            // Lấy Level
            var levels = new FilteredElementCollector(_doc).OfClass(typeof(Level)).Cast<Level>().ToList();

            // Ví dụ: hiển thị số lượng
            MessageBox.Show($"Có {families.Count} Family, {types.Count} Type, {levels.Count} Level", "Dữ liệu Revit");
        }
        public ObservableCollection<Family> FamilyList { get; } = new ObservableCollection<Family>();
        private Family _selectedFamily;
        public Family SelectedFamily
        {
            get => _selectedFamily;
            set
            {
                _selectedFamily = value;
                OnPropertyChanged();
                // Viết thêm  Family, viết thêm ở đây
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
   
