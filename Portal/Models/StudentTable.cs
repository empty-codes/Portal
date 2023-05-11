//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Portal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class StudentTable
    {
        public int RegNo { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required]
        public string Department { get; set; }
        [Required]
        public string Programme { get; set; }
        [Required]
        public Nullable<int> StudyLevel { get; set; }
        [Required]
        [Phone]
        public string Phone { get; set; }
        [Required]
        [RegularExpression("^(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")]
        public string Password { get; set; }
        [Required]
        public string Sex { get; set; }
        [Required]
        public string MaritalStatus { get; set; }
        [Required]
        public string Nationality { get; set; }
        [Required]
        public string MatricNo { get; set; }
        [Required]
        public Nullable<int> Age { get; set; }
        [Required]
        public string DateOfBirth { get; set; }
    }
}
