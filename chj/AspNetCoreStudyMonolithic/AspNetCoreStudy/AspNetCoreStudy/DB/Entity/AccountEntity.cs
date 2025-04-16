namespace AspNetCoreStudy.DB.Entity;

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

public class AccountEntity
{
    [Key, Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public long CreatedTime { get; set; }
}
