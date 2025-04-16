using System.ComponentModel.DataAnnotations;
namespace AspNetCoreStudy.DTO;

public class AccountDTO
{
    [Required, Length(5, 20)]
    public string Id { get; set; } = string.Empty;

    [Required, Length(5, 30)]
    public string Password { get; set; } = string.Empty;
}
