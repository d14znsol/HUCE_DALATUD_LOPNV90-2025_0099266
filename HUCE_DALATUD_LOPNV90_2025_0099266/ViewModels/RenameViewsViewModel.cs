
using Autodesk.Revit.DB;
using HUCE_DALATUD_LOPNV90_2025_0099266;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
        public bool RemoveDiacritics { get; set; }
        public bool ISO19650 { get; set; }

        public RenameViewsViewModel(Document doc)
        {
            _doc = doc;
            LoadViews(); // <--- Hàm quan trọng nhất

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
                        item.TypeName = newName;
                        item.NewTypeName = newName;
                    }
                    catch
                    {
                        // Thường lỗi do trùng tên view
                    }
                }
                tx.Commit();
            }
        }

        // --- Các hàm Logic xử lý chuỗi (ComputeNewName, ApplyFilter...) ---
        // Bạn hãy Copy y hệt từ RenameFamiliesViewModel sang đây vì logic chuỗi là giống nhau
        private string ComputeNewName(string oldName)
        {
            // ... (Copy nội dung hàm ComputeNewName từ RenameFamiliesViewModel)
            // Để ngắn gọn tôi không viết lại ở đây, nhưng bạn bắt buộc phải có để code chạy
            return oldName; // Placeholder
        }

        // Các hàm Filter/CheckAll copy từ RenameFamiliesViewModel sang...
        private void CollectCategories() { /* Copy logic */ }
        private void ApplyFilter() { /* Copy logic */ }
        private void ShowAll() { foreach (var i in ReViews) i.IsSelected = true; }
        private void CheckAll() { foreach (var i in ReViews) i.IsSelected = true; }
        private void UncheckAll() { foreach (var i in ReViews) i.IsSelected = false; }
        private bool CanExecuteRename() { return ReViews.Any(x => x.IsSelected); }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
