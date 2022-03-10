using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracker.Models.ViewModels
{
    public class AddTicketWithProjectViewModel
    {
        public Ticket? Ticket { get; set; }

        public string? OwnerUserID { get; set; }

        public SelectList? ProjectList { get; set; }

        public int? ProjectID { get; set; }

        public SelectList? PriorityList { get; set; }

        public SelectList? StatusList { get; set; }

        public SelectList? TypeList { get; set; }
    }
}
