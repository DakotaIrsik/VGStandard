using System.ComponentModel.DataAnnotations;

namespace VGStandard.Core.ViewModels.Account;

public class LoginWithRecoveryCodeViewModel
{
    #region Properties

    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Recovery Code")]
    public string RecoveryCode { get; set; }

    #endregion
}