using System.ComponentModel.DataAnnotations;
 
namespace BeveragesLib.Models
{
    public class IdName
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
