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

    public class CmdRenameViews : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
                          ref string message,
                          ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            var renameViewsViewModel = new RenameViewsViewModel(doc);
            var winRenameViews = new ReNameViews
            {
                DataContext = renameViewsViewModel
            };

            winRenameViews.ShowDialog();
            return Result.Succeeded;
        }
    }
}
