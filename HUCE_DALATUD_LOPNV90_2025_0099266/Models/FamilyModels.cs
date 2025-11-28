using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HUCE_DALATUD_LOPNV90_2025_0099266
{
   public class FamilyModels : INotifyPropertyChanged
    {
        
        public ElementId Id { get; }
        public string Category { get; }
        public string OldName { get; }

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

        public FamilyModels(ElementId id, string category, string name)
        {
            Id = id;
            Category = category;
            OldName = name;
            NewName = name; // Mặc định NewName = OldName
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }

}

