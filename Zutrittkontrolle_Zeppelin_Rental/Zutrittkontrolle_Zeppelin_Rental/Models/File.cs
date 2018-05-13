using System.ComponentModel.DataAnnotations;

namespace Zutrittkontrolle_Zeppelin_Rental.Models
{
    public class Files
    {
        [Key] 
        public int AutoId { get; set; }
        public string FileName { get; set; }
    }
}