using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PP4CodeFirst.Models
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }

        [Required]
        public string TagName { get; set; } = string.Empty;

        public ICollection<TitleTag> TitleTags { get; set; } = new List<TitleTag>();
    }
}
