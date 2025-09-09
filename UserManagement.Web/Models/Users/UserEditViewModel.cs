using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Web.Models.Users
{
    public class UserEditViewModel
    {
        public long Id { get; set; }

        [Required, StringLength(100)]
        public string Forename { get; set; } = default!;

        [Required, StringLength(100)]
        public string Surname { get; set; } = default!;

        [Required, EmailAddress, StringLength(256)]
        public string Email { get; set; } = default!;

        [Display(Name = "Account Active")]
        public bool IsActive { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }
    }
}
