
using Autodesk.Revit.DB;
using HUCE_DALATUD_LOPNV90_2025_0099266;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace HUCE_DALATUD_LOPNV90_2025_0099266.ViewModels
{

    public class RenameViewsViewModel : INotifyPropertyChanged
    {
        private readonly Document _doc;

        // Collection bind vào DataGrid trong ReNameViews.xaml
        public ObservableCollection<ViewsModels> ReViews { get; }
            = new ObservableCollection<ViewsModels>();

        // --- Các biến Filter và Logic giống hệt RenameFamilies ---
        private string _filterText;
        public string FilterText
        {
            get => _filterText;
            set
            {
                if (_filterText != value) { _filterText = value; OnPropertyChanged(nameof(FilterText)); }
                ApplyFilter();
            }
        }

        private string _filterCategory;
        public string FilterCategory
        {
            get => _filterCategory;
            set
            {
                if (_filterCategory != value) { _filterCategory = value; OnPropertyChanged(nameof(FilterCategory)); ApplyFilter(); }
            }
        }

        public ObservableCollection<string> Categories { get; } = new ObservableCollection<string>();

        // Commands
        public ICommand FilterCommand { get; }
        public ICommand ShowAllCommand { get; }
        public ICommand CheckAllCommand { get; }
        public ICommand UncheckAllCommand { get; }
        public ICommand RenameCommand { get; }

        // Rename Rules Properties (Copy từ bên Families sang)
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
        public string Category { get; set; }


        public RenameViewsViewModel(Document doc)
        {
            _doc = doc;
            LoadViews(); // <--- Hàm quan trọng nhất
            UpdateRenameStats(); // đảm bảo có dữ liệu ngay từ đầu

            // Khởi tạo Command
            FilterCommand = new RelayCommand(ApplyFilter);
            ShowAllCommand = new RelayCommand(ShowAll);
            CheckAllCommand = new RelayCommand(CheckAll);
            UncheckAllCommand = new RelayCommand(UncheckAll);
            RenameCommand = new RelayCommand(ExecuteRename, CanExecuteRename);

            CollectCategories();
        }

        private void LoadViews()
        {
            ReViews.Clear();
            // Lọc lấy các View trong dự án
            var collector = new FilteredElementCollector(_doc)
                .OfClass(typeof(View))
                .Cast<View>()
                .Where(v => !v.IsTemplate) // Quan trọng: Không lấy View Template
                .Where(v => CanRenameView(v)); // Lọc bỏ các view hệ thống không đổi tên được

            foreach (var v in collector)
            {
                ReViews.Add(new ViewsModels(v));
            }
        }

        private void ApplyFilter()
        {
            // Xóa hết trước
            foreach (var item in ReViews)
            {
                bool match = true;

                // Lọc theo Category
                if (!string.IsNullOrEmpty(FilterCategory)
                    && item.Category != FilterCategory)
                {
                    match = false;
                }

                // Lọc theo Text (trong TypeName hoặc NewTypeName)
                if (!string.IsNullOrEmpty(FilterText)
                    && (item.TypeName == null
                        || item.TypeName.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) < 0)
                    && (item.NewTypeName == null
                        || item.NewTypeName.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) < 0))
                {
                    match = false;
                }
                // Nếu không match thì bỏ chọn
                item.IsSelected = match;

            }

        }

        // Hàm phụ trợ để kiểm tra View có đổi tên được không
        private bool CanRenameView(View v)
        {
            // Loại bỏ các view hệ thống như Project Browser, System Browser...
            if (v.ViewType == ViewType.Internal ||
                v.ViewType == ViewType.Undefined ||
                v.ViewType == ViewType.ProjectBrowser ||
                v.ViewType == ViewType.SystemBrowser)
                return false;

            return true;
        }

        private void ExecuteRename()
        {
            using (var tx = new Transaction(_doc, "Rename Views"))
            {
                tx.Start();
                foreach (var item in ReViews.Where(x => x.IsSelected))
                {
                    try
                    {
                        var view = _doc.GetElement(item.ViewId) as View;
                        if (view == null) continue;

                        string newName = ComputeNewName(item.TypeName);

                        // Revit không cho phép trùng tên View, cần try-catch
                        view.Name = newName;

                        // Update UI
                        
                        item.NewTypeName = newName;
                        Category = GetViewCategory(view); // ← phải có dòng này
                        ;
                        UpdateRenameStats(); // cập nhật biểu đồ sau khi rename
                    }
                    catch
                    {
                        // Thường lỗi do trùng tên view
                    }
                }
                tx.Commit();
            }
        }
       

        public static string RemoveDiacritics(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            string normalized = input.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();
            foreach (char c in normalized)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(c);
                }
            }
            return builder.ToString().Normalize(NormalizationForm.FormC);
        }



        // --- Các hàm Logic xử lý chuỗi (ComputeNewName, ApplyFilter...) ---
        // Bạn hãy Copy y hệt từ RenameFamiliesViewModel sang đây vì logic chuỗi là giống nhau
        private string ComputeNewName(string oldName)
        {
            string name = oldName;
            // ... ( nội dung xử lý chuỗi: Substring, Replace, Insert...)
            // Để code chạy được ngay, mình để logic đơn giản
            if (!string.IsNullOrEmpty(AddPrefix)) name = AddPrefix + name;
            if (!string.IsNullOrEmpty(AddSuffix)) name = name + AddSuffix;
           
            if (ToUppercase)
                name = name.ToUpper();

            if (Lowercase)
                name = name.ToLower();


            // ...
            UpdateRenameStats(); // cập nhật biểu đồ sau khi preview tên mới
            

            return name;

        }

        public void ApplyRenameRules()
        {
            foreach (var item in ReViews)
            {
                if (item.IsSelected)
                {
                    string newName = ComputeNewName(item.TypeName);
                    item.NewTypeName = newName;
                }
            }

            UpdateRenameStats(); // cập nhật biểu đồ sau khi đổi tên
        }

        


        private void CollectCategories()
        {
            Categories.Clear();
            Categories.Add("Views"); // Level chỉ có 1 category duy nhất
        }

        // Các hàm Filter/CheckAll copy từ RenameFamiliesViewModel sang...

        private string GetViewCategory(View view)
        {
            return view.ViewType switch
            {
                ViewType.FloorPlan => "FloorPlan",
                ViewType.CeilingPlan => "CeilingPlan",
                ViewType.Elevation => "Elevation",
                ViewType.ThreeD => "ThreeD",
                ViewType.EngineeringPlan => "EngineeringPlan",
                _=> "Other"
            };
        }

        private void ShowAll() { foreach (var i in ReViews) i.IsSelected = true; }
        private void CheckAll() { foreach (var i in ReViews) i.IsSelected = true; }
        private void UncheckAll() { foreach (var i in ReViews) i.IsSelected = false; }
        private bool CanExecuteRename() { return ReViews.Any(x => x.IsSelected); }

        

        public void UpdateRenameStats()
        {
            int success = ReViews.Count(v => v.NewTypeName != v.TypeName);
            int error = 0; // nếu bạn có danh sách lỗi riêng thì gán vào đây
            int pending = ReViews.Count(v => v.NewTypeName == v.TypeName);

            RenameStats= new SeriesCollection
            {
                new PieSeries { Title = "Rename Successful", Values = new ChartValues<int> { success }, Fill = Brushes.Blue },
                new PieSeries { Title = "Error", Values = new ChartValues<int> { error }, Fill = Brushes.Red },
                new PieSeries { Title = "Pending", Values = new ChartValues<int> { pending }, Fill = Brushes.Gold }
            };

            OnPropertyChanged(nameof(RenameStats));
        }
        public SeriesCollection RenameStats { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        
    }

}
