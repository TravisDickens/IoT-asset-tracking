using System.ComponentModel.DataAnnotations;

namespace IOTAssetTracking.Models
{
    // A named firmware build that devices can run.
    public class Firmware
    {
        public int FirmwareID { get; set; }

        [Required]
        [StringLength(100)]
        public string FirmwareName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Version { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        // Devices currently using this firmware
        public ICollection<Device> Devices { get; set; } = new List<Device>();
    }
}