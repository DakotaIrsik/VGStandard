using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VGStandard.Core.ViewModels.Account;

public class LoginViewModel
{
    #region Properties

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Remember me?")] public bool RememberMe { get; set; }

    public string PlayerId { get; set; }

    #endregion
}

[Obsolete("This class is being brought over for interim SharedCookie auth")]
public class LoginApiResponse
{
    public string AuthToken { get; set; }
    public string Release { get; set; }
    public string ManagerHostname { get; set; }
}