using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Project._02.Models
{
    public class Category
    {
        public int id { get; set; }
        [Required]
        public string Name { get; set; }

    }
}
