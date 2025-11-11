using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PP4CodeFirst.Models
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }

        [Required]
        public string AuthorName { get; set; } = string.Empty;

        public ICollection<Title> Titles { get; set; } = new List<Title>();
    }
}
