//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TCSapp.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class TradeTbl
    {
        [Key]
        public int TradeID { get; set; }
        public string TradeName { get; set; }
        public string TradeDescription { get; set; }
        [Display(Name = "Trade Date")]
        [Required]
        public string TradeDate { get; set; }
        public string TradeQuantity { get; set; }
        public string TradePrice { get; set; }
        public string TradeValue { get; set; }
        [Display(Name = "Trade Security")]
        [Required(ErrorMessage = "Please Select a Security")]
        public string TradeMarket { get; set; }
        [Display(Name = "Trade Type")]
        public string TradeType { get; set; }
        public string TradeDeposit { get; set; }
       [Required]
        [MaxLength(12)]
        [MinLength(1)]
        [Display(Name = "Trade Amount")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Trade Amount must be numeric")]
        public string TradeAmount { get; set; }
      
        [MaxLength(12)]
        [MinLength(1)]
        [Display(Name = "Set Loss")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Set Loss must be numeric")]
        public string TradeSL { get; set; }
       
        [MaxLength(12)]
        [MinLength(1)]
        [Display(Name = "Target Price")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Target Price must be numeric")]
        public string TradeTP { get; set; }
        public string TradeLoss { get; set; }
       
        [MaxLength(12)]
        [MinLength(1)]
        [Display(Name = "Trade Profit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Trade Profit must be numeric")]
        public string TradeProfit { get; set; }
        public string TradeZone { get; set; }
        //[Required]
        //[MaxLength(12)]
        //[MinLength(1)]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "UPRN must be numeric")]
        public string TradeLimit { get; set; }
        public string UserID { get; set; }
    }
}