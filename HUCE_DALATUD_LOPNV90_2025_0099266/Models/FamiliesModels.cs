using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUCE_DALATUD_LOPNV90_2025_0099266 
{
    public class FamiliesModels : INotifyPropertyChanged
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

        public FamiliesModels(Family family)
        {
            SymbolId = family.Id;
            FamilyName = family.Name;             // từ Revit API: FamilySymbol.FamilyName :contentReference[oaicite:1]{index=1}
            NewTypeName = TypeName;                      // mặc định, preview = hiện tại
            IsSelected = false;
                    
            Category = family.FamilyCategory?.Name ?? ""; // lấy category — nếu muốn lọc theo category
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}

