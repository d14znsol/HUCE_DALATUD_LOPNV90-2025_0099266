using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUCE_DALATUD_LOPNV90_2025_0099266
{
    public class ViewsModels : INotifyPropertyChanged
    {
        // Lưu ID của View để sau này gọi lệnh Rename
        public ElementId ViewId { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        // Tương ứng cột "Category" trong XAML (Ví dụ: Floor Plan, Ceiling Plan...)
        public string Category { get; }

        // Tương ứng cột "Family Name" trong XAML (Ví dụ: Tên loại View Type)
        public string FamilyName { get; }

        private string _typeName;
        // Tương ứng cột "View Name" trong XAML (Binding Path=TypeName)
        public string TypeName
        {
            get => _typeName;
            set
            {
                if (_typeName != value)
                {
                    _typeName = value;
                    OnPropertyChanged(nameof(TypeName));
                }
            }
        }

        private string _newTypeName;
        // Tương ứng cột "New View Name Preview" trong XAML
        public string NewTypeName
        {
            get => _newTypeName;
            set
            {
                if (_newTypeName != value)
                {
                    _newTypeName = value;
                    OnPropertyChanged(nameof(NewTypeName));
                }
            }
        }

        // Constructor nhận vào một View
        public ViewsModels(View view)
        {
            ViewId = view.Id;

            // 1. Lấy tên View hiện tại
            TypeName = view.Name;
            NewTypeName = TypeName; // Mặc định tên mới = tên cũ
            IsSelected = false;

            // 2. Xử lý Category (Loại View: FloorPlan, Section, 3D...)
            Category = view.ViewType.ToString();

            // 3. Xử lý FamilyName (Lấy tên của ViewFamilyType, ví dụ: "Building Elevation")
            // Để cột Family Name trong bảng không bị trống
            FamilyName = "";
            try
            {
                Document doc = view.Document;
                ElementId typeId = view.GetTypeId();
                if (typeId != ElementId.InvalidElementId)
                {
                    var viewType = doc.GetElement(typeId) as ViewFamilyType;
                    if (viewType != null)
                    {
                        FamilyName = viewType.Name;
                    }
                }
            }
            catch { /* Bỏ qua nếu không lấy được */ }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}

