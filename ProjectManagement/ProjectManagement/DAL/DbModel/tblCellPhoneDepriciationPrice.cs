//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProjectManagement.DAL.DbModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblCellPhoneDepriciationPrice
    {
        public long DepriciationPriceID { get; set; }
        public string ProductCode { get; set; }
        public string Model { get; set; }
        public System.DateTime ReleaseDate { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal InvoicePrice { get; set; }
        public decimal DepriciationPrice { get; set; }
        public Nullable<bool> IsReleaseDate { get; set; }
    }
}
