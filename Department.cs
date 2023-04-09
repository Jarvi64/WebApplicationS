using System.ComponentModel.DataAnnotations;

namespace WebApplicationS.Models
{
    public class Department
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string? DeptName { get; set; }
    }
}
