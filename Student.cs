using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WebApplicationS.Models
{
    public class Student
    { 
       [Key]
    public int ID { get; set; }

    [Required]
    [StringLength(18, ErrorMessage = "Name should be max 12 characters")]
    public string? Name { get; set; }

    [Required]
    public string? Class { get; set; }

    [Required]
    public bool HasPRN { get; set; }

    [Required]
    public string? Gender { get; set; }


    [Display(Name = "Department")]
    public virtual int DeptID { get; set; }


    [ForeignKey("DeptID")]
    public virtual Department? department { get; set; }

    }
    
}
