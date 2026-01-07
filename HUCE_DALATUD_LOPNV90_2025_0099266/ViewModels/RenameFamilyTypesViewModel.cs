using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace HUCE_DALATUD_LOPNV90_2025_0099266.ViewModels
{
    public class RenameFamilyTypesViewModel : INotifyPropertyChanged
    {
        private readonly Document _doc;

        public ObservableCollection<FamilyTypesModels> ReFamilyTypes { get; }
            = new ObservableCollection<FamilyTypesModels>();

        private string _filterText;
        public string FilterText
        {
            get => _filterText;
            set
            {
                if (_filterText != value)
                {
                    _filterText = value;
                    OnPropertyChanged(nameof(FilterText));
                    ApplyFilter();
                }
            }
        }

        private string _filterCategory;
        public string FilterCategory
        {
            get => _filterCategory;
            set
            {
                if (_filterCategory != value)
                {
                    _filterCategory = value;
                    OnPropertyChanged(nameof(FilterCategory));
                    ApplyFilter();
                }
            }
        }

        public ObservableCollection<string> Categories { get; }
            = new ObservableCollection<string>();

        public ICommand FilterCommand { get; }
        public ICommand ShowAllCommand { get; }
        public ICommand CheckAllCommand { get; }
        public ICommand UncheckAllCommand { get; }
        public ICommand RenameCommand { get; }

        // Các thuộc tính rename rule — tương ứng với UI của bạn
        public int RemoveFirst { get; set; }
        public int RemoveLast { get; set; }
        public string AddPrefix { get; set; }
        public string AddSuffix { get; set; }
        public string FindText { get; set; }
        public string ReplaceWith { get; set; }
        public int StartIndex { get; set; }
        public int RemoveCount { get; set; }
        public string AddText { get; set; }
        public bool ToUppercase { get; set; }
        public bool Lowercase { get; set; }
        public bool RemoveDiacritics { get; set; }
        private bool _iso19650;
        public bool ISO19650
        {
            get => _iso19650;
            set
            {
                if (_iso19650 != value)
                {
                    _iso19650 = value;
                    OnPropertyChanged(nameof(ISO19650)); // báo cho UI biết đã thay đổi
                }
            }
        }

        public RenameFamilyTypesViewModel(Document doc)
        {
            _doc = doc ?? throw new ArgumentNullException(nameof(doc));

            LoadFamilyTypes();
            UpdateRenameStats(); // cập nhật biểu đồ sau khi preview tên mới

            FilterCommand = new RelayCommand(ApplyFilter);
            ShowAllCommand = new RelayCommand(ShowAll);
            CheckAllCommand = new RelayCommand(CheckAll);
            UncheckAllCommand = new RelayCommand(UncheckAll);
            RenameCommand = new RelayCommand(ExecuteRename, CanExecuteRename);

            CollectCategories();
        }

        private void LoadFamilyTypes()
        {
            // Lấy tất cả FamilySymbol trong doc
            var collector = new FilteredElementCollector(_doc)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>();

            foreach (var symbol in collector)
            {
                var model = new FamilyTypesModels(symbol);
                ReFamilyTypes.Add(model);
            }
        }

        private void CollectCategories()
        {
            var cats = ReFamilyTypes
                .Select(f => f.Category)
                .Distinct()
                .OrderBy(s => s);
            foreach (var c in cats)
                Categories.Add(c);
        }

        private void ApplyFilter()
        {
            foreach (var item in ReFamilyTypes)
            {
                bool match = true;
                if (!string.IsNullOrEmpty(FilterCategory)
                    && item.Category != FilterCategory)
                    match = false;
                if (!string.IsNullOrEmpty(FilterText)
                && (item.TypeName == null
         || item.TypeName.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) < 0))
                {
                    match = false;
                }
                // Ẩn/hiện — bạn có thể mở rộng, hoặc dùng CollectionView để filter
                // Ở ví dụ đơn giản: nếu không match → unselect
                item.IsSelected = match;
            }
        }

        private void ShowAll()
        {
            foreach (var item in ReFamilyTypes)
                item.IsSelected = true;
        }

        private void CheckAll()
        {
            foreach (var item in ReFamilyTypes)
                item.IsSelected = true;
        }

        private void UncheckAll()
        {
            foreach (var item in ReFamilyTypes)
                item.IsSelected = false;
        }

        private bool CanExecuteRename()
        {
            return ReFamilyTypes.Any(f => f.IsSelected);
        }

        private void ExecuteRename()
        {
            // Bắt transaction
            using (var tx = new Transaction(_doc, "Batch Rename Family Types"))
            {
                tx.Start();
                foreach (var item in ReFamilyTypes.Where(f => f.IsSelected))
                {
                    try
                    {
                        var type = _doc.GetElement(item.SymbolId) as FamilySymbol;
                        if (type == null) continue;

                        string newName = ComputeNewName(item.TypeName);
                        // Gán tên mới
                        type.Name = newName;
                        // Update preview
                        item.NewTypeName = newName;
                        UpdateRenameStats(); // cập nhật biểu đồ sau khi preview tên mới
                    }
                    catch (Exception ex)
                    {
                        // Có thể log lỗi, notify user...
                    }
                }
                tx.Commit();
            }
        }

        private string ComputeNewName(string oldName)
        {
            string name = oldName;

            // Remove first N chars
            if (RemoveFirst > 0 && name.Length > RemoveFirst)
                name = name.Substring(RemoveFirst);

            // Remove last N chars
            if (RemoveLast > 0 && name.Length > RemoveLast)
                name = name.Substring(0, name.Length - RemoveLast);

            // Find & replace
            if (!string.IsNullOrEmpty(FindText))
                name = name.Replace(FindText, ReplaceWith ?? "");

            // Add prefix/suffix
            if (!string.IsNullOrEmpty(AddPrefix))
                name = AddPrefix + name;
            if (!string.IsNullOrEmpty(AddSuffix))
                name = name + AddSuffix;

            // Additional add at index (simplified)
            if (StartIndex >= 0 && StartIndex <= name.Length && !string.IsNullOrEmpty(AddText))
                name = name.Insert(StartIndex, AddText);

            // To upper / lower
            if (ToUppercase)
                name = name.ToUpperInvariant();
            else if (Lowercase)
                name = name.ToLowerInvariant();

            // TODO: Remove diacritics, ISO19650, other rules nếu cần

            return name;
        }
        public void UpdateRenameStats()
        {
            int success = ReFamilyTypes.Count(v => v.NewTypeName != v.TypeName);
            int error = 0; // nếu bạn có danh sách lỗi riêng thì gán vào đây
            int pending = ReFamilyTypes.Count(v => v.NewTypeName == v.TypeName);

            RenameStats = new SeriesCollection
            {
                new PieSeries { Title = "Rename Successful", Values = new ChartValues<int> { success }, Fill = Brushes.Blue },
                new PieSeries { Title = "Error", Values = new ChartValues<int> { error }, Fill = Brushes.Red },
                new PieSeries { Title = "Pending", Values = new ChartValues<int> { pending }, Fill = Brushes.Gold }
            };

            OnPropertyChanged(nameof(RenameStats));
        }

        public SeriesCollection RenameStats { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
