using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CW5.DTOs.Requests
{
    public class EnrollStudentRequest
    {
        [Required]
        public string IndexNumber { get; set; }

        [Required] 
        [MaxLength(15)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(250)]
        public string LastName{ get; set; }

        [Required]
        public string BirthDate { get; set; }

        [Required]
        public string Studies{ get; set; }

    }
}
