﻿using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace haver.Models
{
    public class SalesOrder : Auditable
    {
        public int ID { get; set; }

        //#region Summary Properties

        //[Display(Name = "Order Date")]
        //public string SummaryDate => SoDate.ToString("MMM d, yyyy");

        //[Display(Name = "Approval Drawings Expected")]
        //public string AppDExp => AppDwgExp.ToString("MMM d, yyyy");

        //[Display(Name = "Approval Drawings Released")]
        //public string AppDRel => AppDwgRel.ToString("MMM d, yyyy");

        //[Display(Name = "Approval Drawings Returned")]
        //public string AppDRet => AppDwgRet.ToString("MMM d, yyyy");

        //[Display(Name = "Pre Orders Expected")]
        //public string POExp => PreOExp.ToString("MMM d, yyyy");

        //[Display(Name = "Pre Orders Released")]
        //public string PORel => PreORel.ToString("MMM d, yyyy");

        //[Display(Name = "Engineering Package Expected")]
        //public string EPExp => EngPExp.ToString("MMM d, yyyy");

        //[Display(Name = "Engineering Package Released")]
        //public string EPRel => EngPRel.ToString("MMM d, yyyy");


        //#endregion

        [Display(Name = "Order Number")]
        [Required(ErrorMessage = "Order Number cannot be blank")]
        [RegularExpression("^\\d{8}$", ErrorMessage = "The sales order number must be exactly 8 numeric digits.")]
        [StringLength(8, ErrorMessage = "Order number must be exactly 8 digits.")]
        public string? OrderNumber { get; set; }

        [Display(Name = "Order Date")]
        [Required(ErrorMessage = "Order date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}")]
        public DateTime SoDate { get; set; }

        
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "The price must be a positive value.")]
        public Decimal? Price { get; set; }

        [Display(Name = "Shipping Terms")]
        [MaxLength(800, ErrorMessage = "Shipping terms cannot exceed 800 characters")]
        public string? ShippingTerms { get; set; }

        [Display(Name = "Approved Drawings Expected")]
        [Required(ErrorMessage = "Enter the approval drawings expected date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}")]
        public DateTime? AppDwgExp { get; set; }

        [Display(Name = "Approved Drawings Released")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}")]
        public DateTime? AppDwgRel { get; set; }

        [Display(Name = "Approved Drawings Returned")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}")]
        public DateTime? AppDwgRet { get; set; }

        [Display(Name = "Pre Orders Expected")]
        [Required(ErrorMessage = "Enter the date pre orders are expected")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}")]
        public DateTime? PreOExp { get; set; }

        [Display(Name = "Pre Orders Released")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}")]
        public DateTime? PreORel { get; set; }

        [Display(Name = "Engineering Package Expected")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}")]
        public DateTime? EngPExp { get; set; }

        [Display(Name = "Engineering Package Released")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM d, yyyy}")]
        public DateTime? EngPRel { get; set; }

		[Display(Name = "Customer")]
        [Required(ErrorMessage = "Select the customer related to this order")]
        public int CustomerID { get; set; }
        public Customer? Customer { get; set; }

        // One-to-Many: A Sales Order has multiple Machines
        public ICollection<Machine> Machines { get; set; } = new HashSet<Machine>();

        // Many-to-Many: A Sales Order can have multiple Engineers
        [Display(Name = "Engineers")]
        public ICollection<SalesOrderEngineer> SalesOrderEngineers { get; set; } = new HashSet<SalesOrderEngineer>();
      
        [Display(Name = "Notes/Comments")]
        public string? Comments { get; set; }

        [Display(Name = "Package Release Information")]
        public PackageRelease? PackageRelease { get; set; } // Navigation property to PackageRelease

        public Status Status { get; set; } = Status.InProgress;

     
    }
}
