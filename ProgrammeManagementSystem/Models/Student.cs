using System.ComponentModel.DataAnnotations;

namespace ProgrammeManagementSystem.Models
{
    public class Student
    {
        public int StudentID { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Phone, Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required, Range(1, 4)]
        [Display(Name = "Year of Study")]
        public int YearOfStudy { get; set; }

        // Navigation property
        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();

        public string FullName => $"{FirstName} {LastName}";
    }
}
