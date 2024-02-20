using System;
using System.Windows.Forms;

namespace ProjectComvision
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form2()); // เปลี่ยน Form2 เป็น Form ที่คุณต้องการเริ่มต้น
        }
    }
}
