using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HomeFoodies.Models
{
    [MetadataType(typeof(CustomerMetaData))]

    public partial class Customer
    {
    }
    public class CustomerMetaData
    {   
        public int CustomerID { get; set; }

        [Required]
        [Display(Name = "Your Name")]
        public string CutomerName { get; set; }

        [Required]
        [Display(Name = "Your Address")]
        public string CustomerAddress { get; set; }

        [Required]
        [Display(Name = "Your Phone Number")]
        public string CustomerPhone { get; set; }

       
    }
}