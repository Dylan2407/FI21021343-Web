using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PP4CodeFirst.Models
{
    public class Title
    {
        [Key]
        public int TitleId { get; set; }

        [Required]
        public int AuthorId { get; set; }

        [Required]
        public string TitleName { get; set; } = string.Empty;

        [ForeignKey("AuthorId")]
        public Author Author { get; set; } = null!;

        public ICollection<TitleTag> TitleTags { get; set; } = new List<TitleTag>();
    }
}
