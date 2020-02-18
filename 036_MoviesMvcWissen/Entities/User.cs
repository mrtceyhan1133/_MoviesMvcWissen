using _036_MoviesMvcWissen.Validations.FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.WebPages;

namespace _036_MoviesMvcWissen.Entities
{
    [FluentValidation.Attributes.Validator(typeof(UserValidator))]
    public class User
    {
        public int Id { get; set; }
        [StringLength(50),Required]
        [Index(IsUnique = true)]
        public string UserName { get; set; }
        [StringLength(25),Required]
        public string Password { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
        public bool Active { get; set; }

    }
}