using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracker.Models.ViewModels
{
    public class AssignDeveloperViewModel
    {
        public Ticket? Ticket { get; set; }
        public SelectList? DeveloperList { get; set; }
        public string? DeveloperID { get; set; }
    }
}
