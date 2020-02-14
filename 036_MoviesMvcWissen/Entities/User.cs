using _036_MoviesMvcWissen.Validations.FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public string UserName { get; set; }
        [StringLength(25),Required]
        public string Password { get; set; }
    }
}