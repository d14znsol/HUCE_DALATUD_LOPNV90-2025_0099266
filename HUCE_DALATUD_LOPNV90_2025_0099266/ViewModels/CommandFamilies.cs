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
    public class CommandFamilies : INotifyPropertyChanged

    {
        private readonly Document _doc;
                
       // Data collections
        public ObservableCollection<FamilyModels> ReFamilies { get; } = new ObservableCollection<FamilyModels>();

        // Selection
        private FamilyModels _selectedFamily;
        public FamilyModels SelectedFamily
        {
            get => _selectedFamily;
            set { _selectedFamily = value; OnPropertyChanged(); }
        }

        // Filter text (used by your XAML)
        private string _filterText;
        public string FilterText
        {
            get => _filterText;
            set { _filterText = value; OnPropertyChanged(); }
        }

        // Status
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        // Checked count for UI
        private int _checkedCount;
        private Autodesk.Revit.Creation.Document doc;

        public int CheckedCount
        {
            get => _checkedCount;
            set { _checkedCount = value; OnPropertyChanged(); }
        }

        // Commands
        public ICommand LoadRevitDataCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand ShowAllCommand { get; }
        public ICommand CheckAllCommand { get; }
        public ICommand UncheckAllCommand { get; }
        public ICommand RenameCommand { get; }

        public CommandFamilies(Document doc)
        {
            _doc = doc ?? throw new ArgumentNullException(nameof(doc));

            LoadRevitDataCommand  = new RelayCommand(LoadRevitData);
            FilterCommand         = new RelayCommand(ApplyFilter);
            ShowAllCommand        = new RelayCommand(ShowAll);
            CheckAllCommand       = new RelayCommand(CheckAll);
            UncheckAllCommand     = new RelayCommand(UncheckAll);
            RenameCommand         = new RelayCommand(RenameSelected);
        }

        public CommandFamilies(Autodesk.Revit.Creation.Document doc)
        {
            this.doc = doc;
        }

        private void LoadRevitData()
        {
            try
            {
                IsLoading = true;

                // Families
                var families = new FilteredElementCollector(_doc)
                    .OfClass(typeof(Family))
                    .Cast<Family>()
                    .ToList();

                ReFamilies.Clear();
                foreach (var family in families)
                {
                    var category = family.FamilyCategory?.Name ?? "Unknown";
                    ReFamilies.Add(new FamilyModels(family.Id, category, family.Name));
                }

                // Types (if you need counts)
                var types = new FilteredElementCollector(_doc)
                    .OfClass(typeof(ElementType))
                    .Cast<ElementType>()
                    .ToList();

                // Levels (if you need counts)
                var levels = new FilteredElementCollector(_doc)
                    .OfClass(typeof(Level))
                    .Cast<Level>()  
                    .ToList();

                MessageBox.Show($"Có {families.Count} Family, {types.Count} Type, {levels.Count} Level", "Dữ liệu Revit");
                UpdateCheckedCount();
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Filtering by FilterText (applies on NewName and Category)
        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(FilterText))
                return;

            // Simple in-memory filter toggle: mark IsSelected for match
            foreach (var item in ReFamilies)
            {
                bool match = (item._OldName?.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                          || (item.Category?.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0)
                          || (item.NewName?.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0);

                item.IsSelected = match;
            }
            UpdateCheckedCount();
        }

        private void ShowAll()
        {
            foreach (var item in ReFamilies)
                item.IsSelected = true;

            UpdateCheckedCount();
        }

        private void CheckAll()
        {
            foreach (var item in ReFamilies)
                item.IsSelected = true;

            UpdateCheckedCount();
        }

        private void UncheckAll()
        {
            foreach (var item in ReFamilies)
                item.IsSelected = false;

            UpdateCheckedCount();
        }

        private void UpdateCheckedCount()
        {
            CheckedCount = ReFamilies.Count(f => f.IsSelected);
        }

        private void RenameSelected()
        {
            // Example: rename selected families to their NewName
            using (var tx = new Transaction(_doc, "Rename Families"))
            {
                tx.Start();

                foreach (var fm in ReFamilies.Where(f => f.IsSelected))
                {
                    var famElement = _doc.GetElement(fm.Id) as Family;
                    if (famElement == null) continue;

                    // Avoid null/empty; also skip if same name
                    var newName = fm.NewName;
                    if (string.IsNullOrWhiteSpace(newName) || newName == fm._OldName)
                        continue;

                    // Revit API family rename
                    famElement.Name = newName;
                }

                tx.Commit();
            }

            MessageBox.Show("Đã đổi tên các Family được chọn.", "Thành công");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


    }
}
   
