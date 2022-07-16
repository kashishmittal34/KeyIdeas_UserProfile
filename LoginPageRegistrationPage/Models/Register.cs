using System.ComponentModel.DataAnnotations;

namespace LoginPageRegistrationPage.Models
{
    public class Register
    {

        [Display(Name = "ID")]
        public int ID { get; set; }

        [Required(ErrorMessage = "Please enter FirstName"),MaxLength(40, ErrorMessage = "Maximum Length is 40")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter LastName"), MaxLength(40, ErrorMessage = "Maximum Length is 40")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Password \"{0}\" must have {2} character", MinimumLength = 8)]
        [RegularExpression(@"^([a-zA-Z0-9@*#]{8,15})$", ErrorMessage = "Password must contain: Minimum 8 characters atleast 1 UpperCase Alphabet, 1 LowerCase      Alphabet, 1 Number and 1 Special Character")]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Please enter confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        [DataType(DataType.Password)]
        public string Confirmpwd { get; set; }
        public Nullable<bool> Is_Deleted { get; set; }

        [Display(Name = "Gender")]
        [Required(ErrorMessage = "Please Select the gender")]
        public string Gender { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail id is not valid")]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        [MaxLength(10,ErrorMessage = "Phone Number is 10 digits only")]
        [Required(ErrorMessage = "Phone Number Required!")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter Security Answer")]
        [Display(Name = "Security Answer")]
        public string SecurityAnwser { get; set; }
        [Required(ErrorMessage ="Please enter Security Question")]
        [Display(Name ="Security Question")]
        public string SecurityQuestion { get; set; }
       
        [Required(ErrorMessage = "Please Upload Resume")]
        [Display(Name ="Upload Resume")]
        public IFormFile Resume { get; set; }
        [Required(ErrorMessage ="Please upload Image")]
        [Display(Name = "Profile Picture")]
        public IFormFile ProfileImage { get; set; }
        public string ProfilePicture { get; set; }
        public string Resumepath { get; set; }
       
        [Display(Name ="RememberMe")]

        public bool RememberMe { get; set; }

        public List<Register> Registersinfo { get; set; }
    }
}
