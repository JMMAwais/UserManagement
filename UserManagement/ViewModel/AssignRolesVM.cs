using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.ViewModel
{
    public class AssignRolesVM
    {
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Roll")]
        public string RollName { get; set; }

        public List<SelectListItem> RolesList { get; set; }
    }
}
