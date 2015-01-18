using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebTestProject.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        [Display(Name = "First Name Here")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

}