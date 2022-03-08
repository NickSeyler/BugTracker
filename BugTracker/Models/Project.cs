using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public class Project
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int ProjectPriorityId { get; set; }


        [Required]
        [DisplayName("Project Name")]
        [StringLength(50, ErrorMessage = "The {0} Must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string? Name { get; set; }
        
        [Required]
        [DisplayName("Project Description")]
        [StringLength(2000, ErrorMessage = "The {0} Must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string? Description { get; set; }


        [DisplayName("Date Created")]
        [DataType(DataType.Date)]
        public DateTimeOffset CreatedDate { get; set; }

        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset StartDate { get; set; }

        [DisplayName("End Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset EndDate { get; set; }


        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile? ImageFormFile { get; set; }

        [DisplayName("Image File Name")]
        public string? ImageFileName { get; set; }
        public byte[]? ImageFileData { get; set; }

        [Display(Name = "File Extension")]
        public string? ImageContentType { get; set; }


        public bool Archived { get; set; }


        public virtual Company? Company { get; set; }

        [DisplayName("Project Priority")]
        public virtual ProjectPriority? ProjectPriorty { get; set; }
        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();

    }
}
