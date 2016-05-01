using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CSGeom
{
    static class Program
    {
        // TODO: stupid, change this
        public static string GeomDir = @"E:\[SSD User Data]\Downloads\CSGEOMSamples\CSGEOMSamples\geom";
        public static string ImageDir = @"E:\[SSD User Data]\Downloads\CSGEOMSamples\CSGEOMSamples\images (converted)";

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
