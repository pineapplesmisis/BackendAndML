using System;

namespace MCH.API.Configuration
{
    public class AuthSettings
    {
        public string Issuer { get; set; }
        public string Secret { get; set; }
        public TimeSpan TokenLifeTime { get; set; }
    }
}