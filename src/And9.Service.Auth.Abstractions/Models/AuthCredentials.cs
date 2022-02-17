using System.ComponentModel.DataAnnotations;
using MessagePack;

namespace And9.Service.Auth.Abstractions.Models;

[MessagePackObject]
public struct AuthCredentials
{
    [Required]
    [MessagePack.Key(0)]
    public string Nickname { get; set; }
    [Required]
    [MessagePack.Key(1)]
    public string Password { get; set; }
}