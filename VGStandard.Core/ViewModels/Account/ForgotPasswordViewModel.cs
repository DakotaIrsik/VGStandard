using System.ComponentModel.DataAnnotations;

namespace VGStandard.Core.ViewModels.Account;

public class ForgotPasswordViewModel
{
    #region Properties

    [Required] 
    [EmailAddress]
    [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
    public string Email { get; set; }

    #endregion
}