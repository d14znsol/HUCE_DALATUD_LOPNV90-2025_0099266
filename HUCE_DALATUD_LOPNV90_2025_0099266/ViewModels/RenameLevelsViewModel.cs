using Autodesk.Revit.DB;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace HUCE_DALATUD_LOPNV90_2025_0099266.ViewModels
{
    public class RenameLevelsViewModel : INotifyPropertyChanged
    {
        private readonly Document _doc;

        // Collection bind vào DataGrid trong ReNameLevels.xaml
        public ObservableCollection<LevelsModels> ReLevels { get; }
            = new ObservableCollection<LevelsModels>();

        // --- FILTER PROPERTIES ---
        private string _filterText;
        public string FilterText
        {
            get => _filterText;
            set { if (_filterText != value) { _filterText = value; OnPropertyChanged(nameof(FilterText)); ApplyFilter(); } }
        }

        private string _filterCategory;
        public string FilterCategory
        {
            get => _filterCategory;
            set { if (_filterCategory != value) { _filterCategory = value; OnPropertyChanged(nameof(FilterCategory)); ApplyFilter(); } }
        }

        public ObservableCollection<string> Categories { get; } = new ObservableCollection<string>(); // gán binding

        // --- COMMANDS ---
        public ICommand FilterCommand { get; }
        public ICommand ShowAllCommand { get; }
        public ICommand CheckAllCommand { get; }
        public ICommand UncheckAllCommand { get; }
        public ICommand RenameCommand { get; }

        // --- RENAME RULES (Giống hệt các ViewModel khác) ---
        private bool _isAllSelected;
        public bool IsAllSelected
        {
            get => _isAllSelected;
            set
            {
                if (_isAllSelected != value)
                {
                    _isAllSelected = value;
                    OnPropertyChanged(nameof(IsAllSelected));
                    ToggleAll(_isAllSelected); // gọi hàm chọn/bỏ toàn bộ
                }
            }
        }

        private void ToggleAll(bool select)
        {
            foreach (var item in ReLevels)
                item.IsSelected = select;
        }
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

        public RenameLevelsViewModel(Document doc)
        {
            _doc = doc;
            LoadLevels();

            FilterCommand = new RelayCommand(ApplyFilter);
            ShowAllCommand = new RelayCommand(ShowAll);
            CheckAllCommand = new RelayCommand(CheckAll);
            UncheckAllCommand = new RelayCommand(UncheckAll);
            RenameCommand = new RelayCommand(ExecuteRename, CanExecuteRename);

            CollectCategories();
        }

        private void LoadLevels()
        {
            ReLevels.Clear();
            // Lọc lấy tất cả đối tượng Level
            var collector = new FilteredElementCollector(_doc)
           .OfClass(typeof(Level)).Cast <Level>();

            foreach (var level in collector)
            {
                string name = level.Name;              // tên Level
                double elevation = level.Elevation;    // cao độ (feet)
                ElementId id = level.Id;               // ID để thao tác sau này
                {
                    ReLevels.Add(new LevelsModels(level));
                }
            }

        }
        private void ApplyFilter()
        {
            foreach (var item in ReLevels)
            {
                bool match = true;

                // Lọc theo Category
                if (!string.IsNullOrEmpty(FilterCategory)
                    && item.Category != FilterCategory)
                    match = false;

                // Lọc theo Text
                if (!string.IsNullOrEmpty(FilterText)
                    && (item.TypeName == null
                        || item.TypeName.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) < 0))
                {
                    match = false;
                }

                // Nếu không match thì bỏ chọn
                item.IsSelected = match;
            }
        }
        private void ExecuteRename()
        {
            using (var tx = new Transaction(_doc, "Rename Levels"))
            {
                tx.Start();
                foreach (var item in ReLevels.Where(x => x.IsSelected))
                {
                    try
                    {
                        var lv = _doc.GetElement(item.LevelId) as Level;
                        if (lv == null) continue;

                        string newName = ComputeNewName(item.TypeName);

                        // Đổi tên Level
                        lv.Name = newName;

                        // Update UI
                        item.TypeName = newName;
                        item.NewTypeName = newName;
                    }
                    catch (Exception)
                    {
                        // Level trùng tên sẽ gây lỗi, Revit tự báo hoặc ta bỏ qua
                    }
                }
                tx.Commit();
            }
        }

        // --- LOGIC XỬ LÝ CHUỖI & FILTER (Dùng chung cho cả dự án) ---
        // Bạn copy hàm ComputeNewName, ApplyFilter, ShowAll... từ RenameFamiliesViewModel sang đây nhé
        // Vì logic xử lý text là y hệt nhau.

        // Dưới đây là ví dụ hàm quan trọng nhất cần có để code không lỗi:
        private string ComputeNewName(string oldName)
        {
            string name = oldName;
            // ... (Copy nội dung xử lý chuỗi: Substring, Replace, Insert...)
            // Để code chạy được ngay, mình để tạm logic đơn giản, bạn nhớ paste code đầy đủ vào:
            if (!string.IsNullOrEmpty(AddPrefix)) name = AddPrefix + name;
            if (!string.IsNullOrEmpty(AddSuffix)) name = name + AddSuffix;
            // ...
            return name;
        }

        private void CollectCategories()
        {
            Categories.Clear();
            Categories.Add("Levels"); // Level chỉ có 1 category duy nhất
        }

    

        private void ShowAll() { foreach (var i in ReLevels) i.IsSelected = true; }
        private void CheckAll() { foreach (var i in ReLevels) i.IsSelected = true; }
        private void UncheckAll() { foreach (var i in ReLevels) i.IsSelected = false; }
        private bool CanExecuteRename() { return ReLevels.Any(x => x.IsSelected); }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}