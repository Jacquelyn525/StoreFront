using System;
using System.Collections.Generic;

namespace projName.DATA.EF.UIModels
{
    public partial class Order
    {
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
