using System;
using System.Collections.Generic;

namespace projName.DATA.EF.UIModels
{
    public partial class ProductStatus
    {
        public ProductStatus()
        {
            Products = new HashSet<Product>();
        }

        public int ProductStatusId { get; set; }
        public string ProductStatusName { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; }
    }
}
