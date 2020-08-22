using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
