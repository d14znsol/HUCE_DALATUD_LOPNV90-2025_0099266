using Autodesk.Revit.DB;
using System.ComponentModel;

namespace HUCE_DALATUD_LOPNV90_2025_0099266
{
    public class LevelsModels : INotifyPropertyChanged
    {
        public ElementId LevelId { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); } }
        }

        public string Category { get; }
        public string FamilyName { get; }

        private string _typeName;
        public string TypeName // Tên Level hiện tại (Binding vào cột Level Name)
        {
            get => _typeName;
            set { if (_typeName != value) { _typeName = value; OnPropertyChanged(nameof(TypeName)); } }
        }

        private string _newTypeName;
        public string NewTypeName // Tên mới (Preview)
        {
            get => _newTypeName;
            set { if (_newTypeName != value) { _newTypeName = value; OnPropertyChanged(nameof(NewTypeName)); } }
        }

        public LevelsModels(Level level)
        {
            LevelId = level.Id;
            TypeName = level.Name;
            NewTypeName = TypeName;
            IsSelected = false;

            // Level là đối tượng đặc biệt, Category luôn là "Levels"
            Category = "Levels";

            // Family Name thường chỉ là "Level" hoặc tên của Level Type (ví dụ: "8mm Head")
            // Để đơn giản và đẹp bảng, ta lấy tên Type của nó
            try
            {
                Document doc = level.Document;
                ElementId typeId = level.GetTypeId();
                if (typeId != ElementId.InvalidElementId)
                {
                    var type = doc.GetElement(typeId) as Element;
                    FamilyName = type != null ? type.Name : "Level";
                }
                else
                {
                    FamilyName = "Level";
                }
            }
            catch { FamilyName = "Level"; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}