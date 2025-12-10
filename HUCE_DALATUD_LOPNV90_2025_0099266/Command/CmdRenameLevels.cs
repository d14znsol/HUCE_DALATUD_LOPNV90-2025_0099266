using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HUCE_DALATUD_LOPNV90_2025_0099266.Views;

namespace HUCE_DALATUD_LOPNV90_2025_0099266
{
    [Transaction(TransactionMode.Manual)]
    public class CmdRenameLevels : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
                          ref string message,
                          ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var winRenameLevels = new ReNameLevels();
            winRenameLevels.ShowDialog();
            return Result.Succeeded;
        }
    }
}
