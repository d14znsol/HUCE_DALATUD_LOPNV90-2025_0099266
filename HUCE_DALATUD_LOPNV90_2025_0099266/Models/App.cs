using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace HUCE_DALATUD_LOPNV90_2025_0099266
{
    public class App : IExternalApplication
    {
        static App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var requested = new AssemblyName(args.Name);
            if (!string.Equals(requested.Name, "Xceed.Wpf.Toolkit", StringComparison.OrdinalIgnoreCase))
                return null;

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
                // Tạo tab nếu chưa có
                try
                {
                    application.CreateRibbonTab(tabName);
                }
                catch (Autodesk.Revit.Exceptions.ArgumentException)
                {
                    // tab đã tồn tại -> bỏ qua
                }

                // Tạo / lấy panel
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
                string asmDir = Path.GetDirectoryName(assemblyPath) ?? "";

                // ===== 1. Rename Families =====
                var familiesBtnData = new PushButtonData(
                    "CmdRenameFamilies",
                    "Rename\nFamilies",
                    assemblyPath,
                    "HUCE_DALATUD_LOPNV90_2025_0099266.CmdRenameFamilies"
                );
                var familiesBtn = panel.AddItem(familiesBtnData) as PushButton;
                if (familiesBtn != null)
                {
                    familiesBtn.ToolTip = "Đổi tên Families";
                    string iconFamilies = Path.Combine(asmDir, "Resource", "Icons", "Families.png");
                    if (File.Exists(iconFamilies))
                        familiesBtn.LargeImage = LoadBitmap(iconFamilies);
                }

                // ===== 2. Rename Family Types =====
                var typesBtnData = new PushButtonData(
                    "CmdRenameFamilyTypes",
                    "Rename\nTypes",
                    assemblyPath,
                    "HUCE_DALATUD_LOPNV90_2025_0099266.CmdRenameFamilyTypes"
                );
                var typesBtn = panel.AddItem(typesBtnData) as PushButton;
                if (typesBtn != null)
                {
                    typesBtn.ToolTip = "Đổi tên Family Types";
                    string iconTypes = Path.Combine(asmDir, "Resource", "Icons", "Types.png");
                    if (File.Exists(iconTypes))
                        typesBtn.LargeImage = LoadBitmap(iconTypes);
                }

                // ===== 3. Rename Views =====
                var viewsBtnData = new PushButtonData(
                    "CmdRenameViews",
                    "Rename\nViews",
                    assemblyPath,
                    "HUCE_DALATUD_LOPNV90_2025_0099266.CmdRenameViews"
                );
                var viewsBtn = panel.AddItem(viewsBtnData) as PushButton;
                if (viewsBtn != null)
                {
                    viewsBtn.ToolTip = "Đổi tên Views";
                    string iconViews = Path.Combine(asmDir, "Resource", "Icons", "Views.png");
                    if (File.Exists(iconViews))
                        viewsBtn.LargeImage = LoadBitmap(iconViews);
                }

                // ===== 4. Rename Levels =====
                var levelsBtnData = new PushButtonData(
                    "CmdRenameLevels",
                    "Rename\nLevels",
                    assemblyPath,
                    "HUCE_DALATUD_LOPNV90_2025_0099266.CmdRenameLevels"
                );
                var levelsBtn = panel.AddItem(levelsBtnData) as PushButton;
                if (levelsBtn != null)
                {
                    levelsBtn.ToolTip = "Đổi tên Levels";
                    string iconLevels = Path.Combine(asmDir, "Resource", "Icons", "Levels.png");
                    if (File.Exists(iconLevels))
                        levelsBtn.LargeImage = LoadBitmap(iconLevels);
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("HUCE Ribbon", "Lỗi OnStartup:\n" + ex);
                return Result.Failed;
            }
        }

        private static BitmapImage LoadBitmap(string path)
        {
            var img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(path, UriKind.Absolute);
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.EndInit();
            return img;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
