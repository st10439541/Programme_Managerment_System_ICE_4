using System.ComponentModel.DataAnnotations;

namespace ProgrammeManagementSystem.Models
{
    public class Module
    {
        public int ModuleID { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Module Name")]
        public string ModuleName { get; set; } = string.Empty;

        [Required, StringLength(20)]
        [Display(Name = "Module Code")]
        public string ModuleCode { get; set; } = string.Empty;

        [Required, Range(1, 30)]
        public int Credits { get; set; }

        [Required, StringLength(20)]
        [Display(Name = "Academic Year")]
        public string AcademicYear { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
        public ICollection<ModuleAssignment> ModuleAssignments { get; set; } = new List<ModuleAssignment>();

        public string ModuleDisplay => $"{ModuleCode} - {ModuleName}";
    }
}
