using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace HUCE_DALATUD_LOPNV90_2025_0099266
{
    public class App : IExternalApplication
    {
        // static constructor – chạy 1 lần khi App được load
        static App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // Tên assembly đang được yêu cầu
            var requested = new AssemblyName(args.Name);
            if (!string.Equals(requested.Name, "Xceed.Wpf.Toolkit", StringComparison.OrdinalIgnoreCase))
                return null;    // C# 7.3 vẫn cho phép trả về null vì Assembly là reference type

            // Thư mục chứa dll plugin hiện tại
            string asmDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(asmDir))
                return null;

            string dllPath = Path.Combine(asmDir, "Xceed.Wpf.Toolkit.dll");

            if (File.Exists(dllPath))
            {
                try
                {
                    return Assembly.LoadFrom(dllPath);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            const string tabName = "HUCE Tools";
            const string panelName = "Pro Rename";

            try
            {
                try
                {
                    application.CreateRibbonTab(tabName);
                }
                catch (Autodesk.Revit.Exceptions.ArgumentException)
                {
                    // tab đã tồn tại
                }

                RibbonPanel panel = null;
                foreach (var p in application.GetRibbonPanels(tabName))
                {
                    if (p.Name == panelName)
                    {
                        panel = p;
                        break;
                    }
                }
                if (panel == null)
                    panel = application.CreateRibbonPanel(tabName, panelName);

                string assemblyPath = Assembly.GetExecutingAssembly().Location;

                var btnData = new PushButtonData(
                    "CmdProRename",
                    "Pro Rename",
                    assemblyPath,
                    "HUCE_DALATUD_LOPNV90_2025_0099266.CmdProRename"
                );

                var btn = panel.AddItem(btnData) as PushButton;
                if (btn != null)
                {
                    btn.ToolTip = "Đổi tên Family / Type / Level / View";

                    string iconPath = Path.Combine(
                    Path.GetDirectoryName(assemblyPath) ?? "",
                     "Resource",
                     "Icons",
                     "Rename.png"
                );

                    if (File.Exists(iconPath))
                    {
                        var img = new BitmapImage();
                        img.BeginInit();
                        img.UriSource = new Uri(iconPath, UriKind.Absolute);
                        img.EndInit();
                        btn.LargeImage = img;   // icon 32x32 cho ribbon
                    }

                }


                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("HUCE Ribbon", "Lỗi OnStartup:\n" + ex);
                return Result.Failed;
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
