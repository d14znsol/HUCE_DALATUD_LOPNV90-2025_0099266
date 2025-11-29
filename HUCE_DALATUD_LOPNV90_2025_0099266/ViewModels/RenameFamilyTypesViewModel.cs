using HUCE_DALATUD_LOPNV90_2025_0099266;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Revit = Autodesk.Revit;

namespace HUCE_DALATUD_LOPNV90_2025_0099266
{
    public class RenameFamilyTypesViewModel : INotifyPropertyChanged
    {
        private readonly Revit.DB.Document _doc;
        private bool _isLoading;

        public ObservableCollection<FamilyTypeModels> ReFamilyTypes { get; set; }
        public ObservableCollection<string> Categories { get; set; }
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

        public RenameFamilyTypesViewModel(Revit.DB.Document doc)
        {
            _doc = doc ?? throw new ArgumentNullException(nameof(doc));
            ReFamilyTypes = new ObservableCollection<FamilyTypeModels>();
            Categories = new ObservableCollection<string>();
            LoadData();
            InitializeCommands();
        }

        private void LoadData()
        {
            IsLoading = true;
            try
            {
                var collector = new Revit.DB.FilteredElementCollector(_doc).OfClass(typeof(Revit.DB.ElementType));
                var familyTypes = collector.Cast<Revit.DB.ElementType>()
                    .Where(e => e.FamilyName != null)
                    .ToList();

                Categories.Clear();
                var uniqueCategories = familyTypes.Select(e => e.Category?.Name).Distinct().Where(c => c != null);
                foreach (var category in uniqueCategories)
                    Categories.Add(category);

                ReFamilyTypes.Clear();
                foreach (var type in familyTypes)
                {
                    var info = new FamilyTypeModels
                    {
                        Category = type.Category?.Name ?? "Unknown",
                        FamilyName = type.FamilyName,
                        TypeName = type.Name,
                        NewTypeName = ApplyRenameRules(type.Name),
                        ViewModel = this
                    };
                    ReFamilyTypes.Add(info);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void InitializeCommands()
        {
            FilterCommand = new RelayCommand(() => _ = Task.Run(DebounceFilter));
            ShowAllCommand = new RelayCommand(ShowAll);
            CheckAllCommand = new RelayCommand(() => ReFamilyTypes.ToList().ForEach(t => t.IsSelected = true));
            UncheckAllCommand = new RelayCommand(() => ReFamilyTypes.ToList().ForEach(t => t.IsSelected = false));
            RenameCommand = new RelayCommand(ExecuteRename, CanExecuteRename);
        }

        private async void DebounceFilter()
        {
            await Task.Delay(300); // Debounce 300ms
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (ReFamilyTypes == null) return;
            IsLoading = true;
            try
            {
                var filtered = ReFamilyTypes.Where(t =>
                    (string.IsNullOrEmpty(FilterCategory) || t.Category == FilterCategory) &&
                    (string.IsNullOrEmpty(FilterText) || t.TypeName.Contains(FilterText)))
                    .ToList();
                ReFamilyTypes.Clear();
                foreach (var item in filtered) ReFamilyTypes.Add(item);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ShowAll()
        {
            FilterCategory = null;
            FilterText = null;
            LoadData();
        }

        public string ApplyRenameRules(string originalName)
        {
            if (string.IsNullOrEmpty(originalName)) return originalName;
            string result = originalName;

            if (RemoveFirst > 0 && RemoveFirst < result.Length) result = result.Substring(RemoveFirst);
            if (!string.IsNullOrEmpty(AddPrefix)) result = AddPrefix + result;
            if (RemoveLast > 0 && RemoveLast < result.Length) result = result.Substring(0, result.Length - RemoveLast);
            if (!string.IsNullOrEmpty(AddSuffix)) result = result + AddSuffix;
            if (!string.IsNullOrEmpty(FindText) && !string.IsNullOrEmpty(ReplaceWith))
                result = result.Replace(FindText, ReplaceWith);
            if (StartIndex >= 0 && RemoveCount > 0 && StartIndex < result.Length)
                result = result.Remove(StartIndex, Math.Min(RemoveCount, result.Length - StartIndex));
            if (!string.IsNullOrEmpty(AddText) && StartIndex >= 0)
                result = result.Insert(StartIndex, AddText);
            if (ToUppercase) result = result.ToUpper();

            return result;
        }

        private bool CanExecuteRename()
        {
            return ReFamilyTypes?.Any(t => t.IsSelected) == true;
        }

        private void ExecuteRename()
        {
            if (_doc == null || ReFamilyTypes == null) return;
            IsLoading = true;
            using (var tx = new Revit.DB.Transaction(_doc, "Rename Family Types"))
            {
                tx.Start();
                try
                {
                    foreach (var type in ReFamilyTypes.Where(t => t.IsSelected))
                    {
                        var element = new Revit.DB.FilteredElementCollector(_doc)
                            .OfClass(typeof(Revit.DB.ElementType))
                            .Cast<Revit.DB.ElementType>()
                            .FirstOrDefault(e => e.FamilyName == type.FamilyName && e.Name == type.TypeName);
                        if (element != null)
                            element.Name = type.NewTypeName;
                    }
                    tx.Commit();
                    MessageBox.Show("Renaming completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                }
                catch (Exception ex)
                {
                    tx.RollBack();
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}