using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace myapp.Models
{
    public class NewsAttachment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int NewsArticleId { get; set; }

        [ForeignKey("NewsArticleId")]
        public NewsArticle NewsArticle { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
