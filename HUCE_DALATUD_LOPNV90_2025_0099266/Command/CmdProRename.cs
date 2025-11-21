using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HUCE_DALATUD_LOPNV90_2025_0099266.Views;

namespace HUCE_DALATUD_LOPNV90_2025_0099266.Addin
{
    [Transaction(TransactionMode.Manual)]
    public class CmdProRename : IExternalCommand
    {
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
    }
}
