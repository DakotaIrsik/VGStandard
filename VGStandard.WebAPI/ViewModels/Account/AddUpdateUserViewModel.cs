using System.ComponentModel.DataAnnotations;

namespace VGStandard.WebAPI.ViewModels.Account;

public class AddUserViewModel : AddUpdateBaseViewModel
{
    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }
}

public class UpdateUserViewModel : AddUpdateBaseViewModel
{
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }
}

public class AddUpdateBaseViewModel
{
    [Display(Name = "UserId")]
    public string Id { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Display(Name = "Phone")]
    [RegularExpression("^\\+[1-9]\\d{10,14}$|^$", ErrorMessage = "Phone Number is not in valid(E.164) format")]
    public string PhoneNumber { get; set; }

    [Required]
    [Display(Name = "Clients")]
    public List<string> Clients { get; set; }

    [Required]
    [Display(Name = "Roles")]
    public List<string> Roles { get; set; }
}
