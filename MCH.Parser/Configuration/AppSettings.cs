namespace MCH.Configuration
{
    public class AppSettings
    {
        /// <summary>
        /// Строка подключения к базе данных
        /// </summary>
        public  string DbConnection { get; set; }
        
        /// <summary>
        /// Папка, в которой лежат xml файлы с правилами парсинга
        /// </summary>
        public  string XmlFilesFolder { get; set; }
    }
}