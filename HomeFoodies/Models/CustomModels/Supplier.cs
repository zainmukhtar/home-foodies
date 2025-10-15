using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeFoodies.Models
{
    [MetadataType(typeof(SupplierMetaData))]
    public partial class Supplier
    {
    }

    public class SupplierMetaData
    {
        [Required]
        [Display(Name = "Full Name")]
        [StringLength(60, MinimumLength = 3)]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Kitchen Name")]
        [StringLength(60, MinimumLength = 3)]
        public string CompanyName { get; set; }

        [Required]
        [Display(Name = "Address")]
        [StringLength(60, MinimumLength = 3)]
        public string SupplierAddress { get; set; }

        [Required]
        [Display(Name = "Locality")]
        [StringLength(60, MinimumLength = 3)]
        public string SupplierRegion { get; set; }

        [Required]
        [Display(Name = "City")]
        public string SupplierCity { get; set; }

        [Required]
        [Display(Name = "Contact")]
        [StringLength(60, MinimumLength = 11)]
        public string ContactNumber { get; set; }

        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email")]
        [StringLength(60, MinimumLength = 3)]
        public string SupplierEmail { get; set; }
    }
}