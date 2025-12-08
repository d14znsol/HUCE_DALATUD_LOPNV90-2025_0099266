using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HUCE_DALATUD_LOPNV90_2025_0099266.Views;
using HUCE_DALATUD_LOPNV90_2025_0099266.ViewModels;

namespace HUCE_DALATUD_LOPNV90_2025_0099266
{
    [Transaction(TransactionMode.Manual)]
    public class CmdRenameFamilyTypes : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // ViewModel bạn đã có
            var renameFamilyTypesViewModel = new RenameFamilyTypesViewModel(doc);

            // Window XAML bạn đã gửi là ReNameFamilyTypes
            var win = new ReNameFamilyTypes
            {
                DataContext = renameFamilyTypesViewModel
            };

            win.ShowDialog();
            return Result.Succeeded;
        }
    }
}
