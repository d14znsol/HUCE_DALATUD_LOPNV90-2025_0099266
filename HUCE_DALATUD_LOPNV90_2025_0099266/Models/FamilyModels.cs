using Autodesk.Revit.DB;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HUCE_DALATUD_LOPNV90_2025_0099266
{
    public class FamilyModels : INotifyPropertyChanged
    {
        public ElementId Id { get; }

        private string _category;
        public string Category
        {
            get => _category;
            set
            {
                if (_category != value)
                {
                    _category = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _oldName;
        public string OldName
        {
            get => _oldName;
            set
            {
                if (_oldName != value)
                {
                    _oldName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _newName;
        public string NewName
        {
            get => _newName;
            set
            {
                if (_newName != value)
                {
                    _newName = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        // Constructor
        public FamilyModels(ElementId id, string category, string name)
        {
            Id = id;
            Category = category;
            OldName = name;
            NewName = name; // mặc định NewName = OldName
            IsSelected = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}