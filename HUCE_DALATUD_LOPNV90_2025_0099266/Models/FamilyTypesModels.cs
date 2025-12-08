using System.ComponentModel;
using Autodesk.Revit.DB;

namespace HUCE_DALATUD_LOPNV90_2025_0099266
{
    public class FamilyTypesModels : INotifyPropertyChanged
    {
        // ID của FamilySymbol (type) — dùng để apply rename / tìm lại trong Document
        public ElementId SymbolId { get; }

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

        public string Category { get; }
         
        public string FamilyName { get; }

        private string _typeName;
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

        public FamilyTypesModels(FamilySymbol symbol)
        {
            SymbolId = symbol.Id;
            FamilyName = symbol.FamilyName;             // từ Revit API: FamilySymbol.FamilyName :contentReference[oaicite:1]{index=1}
            TypeName = symbol.Name;                      // tên type hiện tại
            NewTypeName = TypeName;                      // mặc định, preview = hiện tại
            IsSelected = false;

            Category = symbol.Family.FamilyCategory?.Name ?? ""; // lấy category — nếu muốn lọc theo category
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
