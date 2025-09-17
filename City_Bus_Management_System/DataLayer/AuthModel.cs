﻿using System.Text.Json.Serialization;

namespace City_Bus_Management_System.DataLayer
{
    public class AuthModel
    {
        public string? Id { get; set; }
        public string? Message { get; set; }
        public bool IsAuthenticated { get; set; } = false;
        public string? Username { get; set; }
        public string? Email { get; set; }
        public List<string>? Roles { get; set; }
        public string? Token { get; set; }
        [JsonIgnore]
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
        //public DateTime? ExpiresOn { get; set; }

    }
}
