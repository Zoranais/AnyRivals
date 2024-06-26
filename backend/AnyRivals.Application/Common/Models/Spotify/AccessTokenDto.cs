﻿using System.Text.Json.Serialization;

namespace AnyRivals.Application.Common.Models.Spotify;
public class AccessTokenDto
{
    //[JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    //[JsonPropertyName("token_type")]
    public string TokenType { get; set; }
    //[JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
}
