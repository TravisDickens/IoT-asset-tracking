using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IOTAssetTracking.Models
{
    // Logical grouping of devices, can nest under another group (tree).
    public class Group
    {
        public int GroupID { get; set; }

        [Required]
        [StringLength(100)]
        public string GroupName { get; set; } = string.Empty;

        // Optional parent - null means top-level group
        public int? ParentGroupID { get; set; }
        public Group? ParentGroup { get; set; }

        // Groups nested under this one
        public ICollection<Group> ChildGroups { get; set; } = new List<Group>();

        // Devices assigned to this group
        public ICollection<Device> Devices { get; set; } = new List<Device>();

        // UI-only: nest level for Index indentation (set in the controller, not stored in DB)
        [NotMapped]
        public int Depth { get; set; }
    }
}