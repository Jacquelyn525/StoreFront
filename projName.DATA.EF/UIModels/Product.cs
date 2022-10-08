using System;
using System.Collections.Generic;

namespace projName.DATA.EF.UIModels
{
    public partial class Product
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string ProductName { get; set; } = null!;
        public int? QtyPerUnit { get; set; }
        public decimal ProductPrice { get; set; }
        public int ProductStatusId { get; set; }
        public string ProductDescription { get; set; } = null!;

        public virtual Category Category { get; set; } = null!;
        public virtual ProductStatus ProductStatus { get; set; } = null!;
    }
}
