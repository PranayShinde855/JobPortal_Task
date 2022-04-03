using System;

namespace Models.DTOs
{
    public class TokenDetailsTO
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
