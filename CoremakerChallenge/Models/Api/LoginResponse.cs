﻿namespace CoremakerChallenge.Models.Api;

public class LoginResponse
{
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}
