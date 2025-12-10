using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HUCE_DALATUD_LOPNV90_2025_0099266.ViewModels;
using HUCE_DALATUD_LOPNV90_2025_0099266.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUCE_DALATUD_LOPNV90_2025_0099266
{
    [Transaction(TransactionMode.Manual)]
    public class CmdRenameFamilies : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            // Sau này bạn có thể lấy UIDocument, Document ở đây
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Gọi UI WPF có sẵn
            var renameFamiliesViewModel = new RenameFamiliesViewModel(doc);
            var winRenameFamilies = new ReNameFamilies()
            {
                DataContext = renameFamiliesViewModel   
            };

            winRenameFamilies.ShowDialog();
            return Result.Succeeded;
        }
    }
}
