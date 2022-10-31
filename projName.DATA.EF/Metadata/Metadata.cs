using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projName.DATA.EF.Models//.Metadata
{
    public class CategoryMetadata
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "*Name is required")]
        [StringLength(50, ErrorMessage = "*Must be 50 characters")]
        [Display(Name = "Category")]
        public string CategoryName { get; set; } = null!;

    }
    public class OrderMetadata
    {
        public int OrderId { get; set; }

        [Required(ErrorMessage = "*Must enter a Quantity")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "*Order Date is required")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        [Display(Name = " Order Date")]
        public DateTime OrderDate { get; set; }

    }
    public class ProductMetadata
    {
        public int ProductId { get; set; }

        public int CategoryId { get; set; }

        [Required(ErrorMessage = "*Name is required")]
        [StringLength(50, ErrorMessage = "*Cannot exceed 50 characters")]
        [Display(Name = "Name?")]
        public string ProductName { get; set; } = null!;

        [Display(Name = "Quantity Per Unit")]
        public int QtyPerUnit { get; set; }

        [Display(Name = "Price")]
        [Required(ErrorMessage = "*Price is required")]
        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = false)]
        [Range(0, (double)decimal.MaxValue)]
        public decimal ProductPrice { get; set; }


        public int ProductStatusId { get; set; }

        [StringLength(500, ErrorMessage = "*Cannot exceed 500 characters")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? ProductDescription { get; set; }

    }

    public class UserDetailMetadata
    {
        public string UserId { get; set; } = null!;

        [StringLength(50)]
        [Display(Name = "First Name")]
        [Required]
        public string FirstName { get; set; } = null!;

        [StringLength(50)]
        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; } = null!;

        [StringLength(150)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(2)]
        public string? State { get; set; }

        [StringLength(5)]
        [DataType(DataType.PostalCode)]
        public string? Zip { get; set; }

        [StringLength(24)]
        [DataType(DataType.PhoneNumber)]
        public string? Phone { get; set; }
    }

}
