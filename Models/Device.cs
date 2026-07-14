using System.ComponentModel.DataAnnotations;

namespace IOTAssetTracking.Models
{
    // A tracked IoT asset with a unique serial, name, firmware, and optional group.
    public class Device
    {
        public int DeviceID { get; set; }

        [Required]
        [StringLength(50)]
        public string SerialNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string DeviceName { get; set; } = string.Empty;

        // Required FK to the firmware currently installed on this device
        [Required]
        public int FirmwareID { get; set; }
        public Firmware? Firmware { get; set; }

        // Optional FK, device may be unassigned
        public int? GroupID { get; set; }
        public Group? Group { get; set; }
    }
}