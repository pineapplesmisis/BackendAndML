namespace MCH.API.Configuration
{
    public class AppSettings
    {
        /// <summary>
        /// Строка подключения к базе данных
        /// </summary>
        public  string DbConnection { get; set; }
        
        /// <summary>
        /// Адрес апи с машинным обучением
        /// </summary>
        public string MlApiUrl { get; set; }
    }
}