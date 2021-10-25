using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Session6Desktop
{
    /// <summary>
    /// Класс для преобразования в ico
    /// </summary>
    public class IcoConverter
    {
        /// <summary>
        ///  Получение ico из png
        /// </summary>
        public static void ConvertIco()
        {
            using (FileStream stream = File.OpenWrite(@"C:\C#\Kazan-Neft\Session6Desktop\Session6Desktop\Images\logo.ico"))
            {
                Bitmap bitmap = (Bitmap)Image.FromFile(@"C:\C#\Kazan-Neft\Session6Desktop\Session6Desktop\Images\KN En Colors.png");
                Icon.FromHandle(bitmap.GetHicon()).Save(stream);
            }
        }
    }
}
