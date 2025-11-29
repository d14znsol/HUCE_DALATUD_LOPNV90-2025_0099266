using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUCE_DALATUD_LOPNV90_2025_0099266
{
    public class FamilyTypeModels : INotifyPropertyChanged
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public string Category { get; set; }
        public string FamilyName { get; set; }
        public string TypeName { get; set; }

        private string _newTypeName;
        public string NewTypeName
        {
            get => _newTypeName;
            set
            {
                _newTypeName = value;
                OnPropertyChanged(nameof(NewTypeName));
                if (ViewModel != null) NewTypeName = ViewModel.ApplyRenameRules(TypeName);
            }
        }

        public RenameFamilyTypesViewModel ViewModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


