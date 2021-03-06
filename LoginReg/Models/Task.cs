//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LoginReg.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Task
    {
        public int Id { get; set; }
        public string Email { get; set; }

        [Display(Name = "Task Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Title Required!")]
        public string Title { get; set; }

        [DataType(DataType.MultilineText)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Description Required!")]
        public string Description { get; set; }

        [Display(Name = "Due Date")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Date Required!")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public string DueDate { get; set; }

        
        public string Status { get; set; }
        public string Priority { get; set; }

    }

}
