using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Session6Desktop.Base
{
    /// <summary>
    /// Класс для получения контекста данных
    /// </summary>
    public class AppData
    {
        private static KazanNeftSession6DBEntities context;

        /// <summary>
        /// Получение контекста данных
        /// </summary>
        /// <returns>Контекст данных</returns>
        public static KazanNeftSession6DBEntities GetContext()
        {
            if (context == null)
                context = new KazanNeftSession6DBEntities();
            return context;
        }
    }
}
