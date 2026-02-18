using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Models
{
    public class BaseUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // order Navigation property 
        public ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}
