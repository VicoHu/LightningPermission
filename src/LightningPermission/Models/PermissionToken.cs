using System.ComponentModel.DataAnnotations;

namespace LightningPermission.Models
{
    public class PermissionToken
    {
        [Key]
        public string Uid { get; set; }

        [Required]
        public string TokenStr { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
