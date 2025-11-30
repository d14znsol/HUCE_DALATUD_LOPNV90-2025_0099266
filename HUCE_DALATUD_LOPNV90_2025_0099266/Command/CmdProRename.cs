using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HUCE_DALATUD_LOPNV90_2025_0099266.ViewModels;
using HUCE_DALATUD_LOPNV90_2025_0099266.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Revit = Autodesk.Revit;

namespace HUCE_DALATUD_LOPNV90_2025_0099266
{
    [Transaction(TransactionMode.Manual)]
    public class CmdProRename : IExternalCommand
    {
        private readonly Revit.DB.Document _doc;
        private bool _isLoading;

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            // Sau này bạn có thể lấy UIDocument, Document ở đây
            // UIDocument uidoc = commandData.Application.ActiveUIDocument;
            // Document doc = uidoc.Document;

            // Gọi UI WPF có sẵn
            var win = new MAINWindow();
            win.ShowDialog();

            return Result.Succeeded;
        }
        public string FilterCategory { get; set; }
        public string FilterText { get; set; }
        public int RemoveFirst { get; set; }
        public string AddPrefix { get; set; }
        public int RemoveLast { get; set; }
        public string AddSuffix { get; set; }
        public string FindText { get; set; }
        public string ReplaceWith { get; set; }
        public int StartIndex { get; set; }
        public int RemoveCount { get; set; }
        public string AddText { get; set; }
        public bool ToUppercase { get; set; }
        public bool RemoveDiacritics { get; set; }
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }



        public ICommand FilterCommand { get; private set; }
        public ICommand ShowAllCommand { get; private set; }
        public ICommand CheckAllCommand { get; private set; }
        public ICommand UncheckAllCommand { get; private set; }
        public ICommand RenameCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }   
}
