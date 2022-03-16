using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int TicketTypeId { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketStatusId { get; set; }

        [Required]
        public string? OwnerUserId { get; set; }
        public string? DeveloperUserId { get; set; }


        [Required]
        [DisplayName("Ticket Title")]
        [StringLength(50, ErrorMessage = "The {0} Must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string? Title { get; set; }

        [Required]
        [DisplayName("Ticket Description")]
        [StringLength(2000, ErrorMessage = "The {0} Must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string? Description { get; set; }


        [DisplayName("Date Created")]
        [DataType(DataType.Date)]
        public DateTimeOffset CreatedDate { get; set; }

        [DisplayName("Date Updated")]
        [DataType(DataType.Date)]
        public DateTimeOffset? UpdatedDate { get; set; }

        [DisplayName("Archived")]
        public bool Archived { get; set; }

        [DisplayName("Archived By Project")]
        public bool ArchivedByProject { get; set; }


        public virtual Project? Project { get; set; }

        [DisplayName("Ticket Type")]
        public virtual TicketType? TicketType { get; set; }

        [DisplayName("Ticket Priority")]
        public virtual TicketPriority? TicketPriority { get; set; }

        [DisplayName("Ticket Status")]
        public virtual TicketStatus? TicketStatus { get; set; }

        [DisplayName("Owner")]
        public virtual BTUser? OwnerUser { get; set; }

        [DisplayName("Developer")]
        public virtual BTUser? DeveloperUser { get; set; }

        public virtual ICollection<TicketComment> Comments { get; set; } = new HashSet<TicketComment>();
        public virtual ICollection<TicketAttachment> Attachments { get; set; } = new HashSet<TicketAttachment>();
        public virtual ICollection<TicketHistory> History { get; set; } = new HashSet<TicketHistory>();
        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
    }
}
