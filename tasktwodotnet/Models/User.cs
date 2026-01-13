using System.ComponentModel.DataAnnotations;

namespace tasktwodotnet.Models
{
    public class User
    {
        [Key]
        public int id { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }   
        public string? password { get; set; }
        public string? mobile { get; set; }  
        public string? city { get; set; }
        public string? dob { get; set; }
        public string? address { get; set; }
        public bool verification { get; set; }
    }
}
