using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ITRepairService.ViewModels.News;

public class CreateNewsViewModel
{
    [Required]
    [StringLength(160)]
    [Display(Name = "Title")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(4000)]
    [Display(Name = "Content")]
    public string Content { get; set; } = string.Empty;

    [Display(Name = "Attachment")]
    public IFormFile? Attachment { get; set; }

    [Display(Name = "Status")]
    public bool IsActive { get; set; } = true;
}
