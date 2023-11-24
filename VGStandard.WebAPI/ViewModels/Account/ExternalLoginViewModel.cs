using System.ComponentModel.DataAnnotations;

namespace VGStandard.WebAPI.ViewModels.Account;

public class ExternalLoginViewModel
{
    #region Properties

    [Required][EmailAddress] public string Email { get; set; }

    #endregion
}