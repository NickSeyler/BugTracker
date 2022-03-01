using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int? TicketId { get; set; }
        public int NotificationTypeId { get; set; }

        [Required]
        public string? RecipientId { get; set; }
        [Required]
        public string? SenderId { get; set; }


        [Required]
        [StringLength(50, ErrorMessage = "The {0} Must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string? Title { get; set; }

        [Required]
        [StringLength(2000, ErrorMessage = "The {0} Must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
        public string? Message { get; set; }

        [DisplayName("Date Created")]
        [DataType(DataType.Date)]
        public DateTimeOffset CreatedDate { get; set; }

        [DisplayName("Has been viewed")]
        public bool Viewed { get; set; }


        public virtual Ticket? Ticket { get; set; }
        public virtual NotificationType? NotificationType { get; set; }
        public virtual BTUser? Recipient { get; set; }
        public virtual BTUser? Sender { get; set; }

    }
}
