//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CashierApp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Product
    {
        public int Id { get; set; }
        public string Nomitation { get; set; }
        public string Barcode { get; set; }
        public decimal Amount { get; set; }
        public decimal Price { get; set; }
        public byte[] Image { get; set; }
    }
}