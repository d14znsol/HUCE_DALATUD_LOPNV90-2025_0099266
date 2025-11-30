using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HUCE_DALATUD_LOPNV90_2025_0099266.ViewModels;
using HUCE_DALATUD_LOPNV90_2025_0099266.Views;
using System;

namespace HUCE_DALATUD_LOPNV90_2025_0099266.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class CmdRenameFamilies : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // Lấy document từ Revit
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                Document doc = uidoc.Document;

                // Tạo ViewModel và truyền Document vào
                var vm = new CommandFamilies(doc);

                // Gọi lệnh nạp dữ liệu
                vm.LoadRevitDataCommand.Execute(null); // hoặc vm.LoadRevitData();

                // Tạo cửa sổ và gán DataContext
                var win = new ReNameFamilies
                {
                    DataContext = vm
                };

                // Hiển thị giao diện
                win.ShowDialog();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = $"Lỗi khi mở Rename Families: {ex.Message}";
                return Result.Failed;
            }
        }
    }
}