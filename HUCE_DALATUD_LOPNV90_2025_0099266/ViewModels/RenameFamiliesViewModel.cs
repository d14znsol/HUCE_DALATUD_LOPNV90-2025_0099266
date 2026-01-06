using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HUCE_DALATUD_LOPNV90_2025_0099266
{
    public class RenameFamiliesViewModel : INotifyPropertyChanged
    {
        private readonly Document _doc;

        public ObservableCollection<FamiliesModels> ReFamilies { get; }
            = new ObservableCollection<FamiliesModels>();

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
        public bool ISO19650 { get; set; }

        public RenameFamiliesViewModel(Document doc)
        {
            _doc = doc ?? throw new ArgumentNullException(nameof(doc));

            LoadFamilies();

            FilterCommand = new RelayCommand(ApplyFilter);
            ShowAllCommand = new RelayCommand(ShowAll);
            CheckAllCommand = new RelayCommand(CheckAll);
            UncheckAllCommand = new RelayCommand(UncheckAll);
            RenameCommand = new RelayCommand(ExecuteRename, CanExecuteRename);

            CollectCategories();
        }

        private void LoadFamilies()
        {
            // Lấy tất cả Family trong doc
            var collector = new FilteredElementCollector(_doc)
                .OfClass(typeof(Family))
                .Cast<Family>();

            foreach (var symbol in collector)
            {
                var model = new FamiliesModels(symbol);
                ReFamilies.Add(model);
            }
        }
        

        private void CollectCategories()
        {
            var cats = ReFamilies
                .Select(f => f.Category)
                .Distinct()
                .OrderBy(s => s);
            foreach (var c in cats)
                Categories.Add(c);
        }

        private void ApplyFilter()
        {
            foreach (var item in ReFamilies)
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
            foreach (var item in ReFamilies)
                item.IsSelected = true;
        }

        private void CheckAll()
        {
            foreach (var item in ReFamilies)
                item.IsSelected = true;
        }

        private void UncheckAll()
        {
            foreach (var item in ReFamilies)
                item.IsSelected = false;
        }

        private bool CanExecuteRename()
        {
            return ReFamilies.Any(f => f.IsSelected);
        }


        private void ExecuteRename()
        {
            // Bắt transaction
            using (var tx = new Transaction(_doc, "Batch Rename Families"))
            {
                tx.Start();
                foreach (var item in ReFamilies.Where(f => f.IsSelected))
                {
                    try
                    {
                        var symbol = _doc.GetElement(item.SymbolId) as FamilySymbol;
                        if (symbol == null) continue;

                        string newName = ComputeNewName(item.TypeName);

                        // Đổi tên type
                        symbol.Name = newName;

                        // Update lại UI
                        item.NewTypeName = newName;
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}

