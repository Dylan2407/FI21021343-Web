using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PP4CodeFirst.Models
{
    public class TitleTag
    {
        [Key]
        public int TitleTagId { get; set; }

        [Required]
        public int TitleId { get; set; }

        [Required]
        public int TagId { get; set; }

        [ForeignKey("TitleId")]
        public Title Title { get; set; } = null!;

        [ForeignKey("TagId")]
        public Tag Tag { get; set; } = null!;
    }
}
