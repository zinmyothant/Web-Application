using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace EmployeeManagement.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Name : ")]
        [Required]
        public string Name { get; set; }
        [DisplayName("Position : ")]
        [Required]
        public string Position { get; set; }
        [DisplayName("Salary : ")]
        [Required]
        public string Salary { get; set; }
    }
}