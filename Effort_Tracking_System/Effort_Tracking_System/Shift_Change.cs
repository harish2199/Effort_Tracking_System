//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Effort_Tracking_System
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Linq;

    public partial class Shift_Change
    {
        public int shift_Change_id { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public Nullable<int> user_id { get; set; }

        [Required(ErrorMessage = "Assigned Shift ID is required.")]
        public Nullable<int> assigned_shift_id { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public System.DateTime date { get; set; }

        [Required(ErrorMessage = "New Shift ID is required.")]
        public Nullable<int> new_shift_id { get; set; }

        [Required(ErrorMessage = "Reason is required.")]
        [StringLength(200, ErrorMessage = "Reason cannot exceed 200 characters.")]
        public string reason { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters.")]
        public string status { get; set; }

        public virtual Shift Shift { get; set; }
        public virtual Shift Shift1 { get; set; }
        public virtual User User { get; set; }
    }
}
