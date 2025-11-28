using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HUCE_DALATUD_LOPNV90_2025_0099266.ViewModels
{
    public class RenameFamilyViewModel : INotifyPropertyChanged
    {
        private readonly Document _doc;

        // ObservableCollection chứa các đối tượng Family cần đổi tên
        public ObservableCollection<FamilyModels> Families { get; } = new ObservableCollection<FamilyModels>();

        // Các thuộc tính binding cho các TextBox, CheckBox trong giao diện WPF
        public string FilterText { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public string FindText { get; set; }
        public string ReplaceWith { get; set; }
        public bool ToUppercase { get; set; }
        public bool ToLowercase { get; set; }
        public bool RemoveDiacritics { get; set; }

        // Các Command sẽ được sử dụng trong giao diện (lọc, hiển thị tất cả, áp dụng đổi tên)
        public ICommand FilterCommand { get; }
        public ICommand ShowAllCommand { get; }
        public ICommand ApplyRenameCommand { get; }

        public RenameFamilyViewModel(Document doc)
        {
            _doc = doc;

            // Khởi tạo các command
            FilterCommand = new RelayCommand(FilterFamilies);
            ShowAllCommand = new RelayCommand(ShowAllFamilies);
            ApplyRenameCommand = new RelayCommand(ApplyRename, CanApplyRename);

            // Gọi phương thức LoadFamilyNames để lấy danh sách Family
            LoadFamilyNames();
        }

        // Lọc các Family theo tên
        private void FilterFamilies()
        {
            Families.Clear();

            var collector = new FilteredElementCollector(_doc).OfClass(typeof(Family)); // Lấy tất cả các Family
            foreach (var family in collector)
            {
                if (family.Name.Contains(FilterText)) // Lọc các Family theo tên
                {
                    Families.Add(new FamilyModels(family.Id, "Family", family.Name)); // Thêm vào ObservableCollection
                }
            }
        }

        // Hiển thị tất cả các Family (không có bộ lọc)
        private void ShowAllFamilies()
        {
            Families.Clear();

            var collector = new FilteredElementCollector(_doc).OfClass(typeof(Family));
            foreach (var family in collector)
            {
                Families.Add(new FamilyModels(family.Id, "Family", family.Name)); // Thêm tất cả Family vào ObservableCollection
            }
        }

        // Phương thức kiểm tra điều kiện trước khi áp dụng đổi tên
        private bool CanApplyRename()
        {
            return Families.Any(f => f.IsSelected);  // Kiểm tra xem có chọn Family nào không
        }

        // Phương thức thực hiện đổi tên
        private void ApplyRename()
        {
            using (var t = new Transaction(_doc, "Rename Families"))
            {
                t.Start();

                foreach (var item in Families.Where(i => i.IsSelected))  // Chỉ áp dụng cho các Family đã chọn
                {
                    string newName = item.NewName;

                    // Áp dụng Prefix và Suffix
                    if (!string.IsNullOrEmpty(Prefix)) newName = Prefix + newName;
                    if (!string.IsNullOrEmpty(Suffix)) newName = newName + Suffix;

                    // Áp dụng Find and Replace
                    if (!string.IsNullOrEmpty(FindText)) newName = newName.Replace(FindText, ReplaceWith);

                    // Áp dụng Uppercase, Lowercase
                    if (ToUppercase) newName = newName.ToUpper();
                    if (ToLowercase) newName = newName.ToLower();

                    // Loại bỏ dấu tiếng Việt
                    if (RemoveDiacritics) newName = RemoveAccents(newName);

                    // Đổi tên Family trong Revit
                    var family = _doc.GetElement(item.Id) as Family;
                    if (family != null)
                    {
                        family.Name = newName;
                    }
                }

                t.Commit();
            }
        }

        // Hàm loại bỏ dấu tiếng Việt
        private string RemoveAccents(string input)
        {
            return new string(input.Normalize(NormalizationForm.FormD)
                             .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                             .ToArray());
        }

        // Lấy tên tất cả các Family trong dự án
        private void LoadFamilyNames()
        {
            Families.Clear();

            var collector = new FilteredElementCollector(_doc).OfClass(typeof(Family)); // Lấy tất cả các Family
            foreach (var family in collector)
            {
                if (!string.IsNullOrEmpty(family.Name))  // Kiểm tra tên Family có hợp lệ không
                {
                    Families.Add(new FamilyModels(family.Id, "Family", family.Name)); // Thêm vào ObservableCollection
                }
            }
        }

        // PropertyChanged event implementation for INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}