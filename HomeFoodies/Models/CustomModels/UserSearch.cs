using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeFoodies.Models.CustomModels
{
    public class UserSearch
    {
        public UserSearch()
        {
            this.SupplierRegion = "";
            this.SupplierId = 0;
            this.ItemCategoryId = 0;
            this.ItemName = "";
        }

        public string SupplierRegion { get; set; }
        public int SupplierId { get; set; }
        public int ItemCategoryId { get; set; }
        public string ItemName { get; set; }
    }
}