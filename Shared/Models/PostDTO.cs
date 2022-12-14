namespace Shared.Models;

public class PostDTO
{
    [Key]
    public int PostId { get; set; }
    [Required, MaxLength(128)]
    public string Title { get; set; }
    [Required, MaxLength(255)]
    public string ThumbnailImagePath { get; set; }
    [Required, MaxLength(512)]
    public string Excerpt { get; set; }
    [MaxLength(65536)]
    public string Content { get; set; }
    [Required]
    public bool Published { get; set; }
    [Required, MaxLength(128)]
    public string Author { get; set; }
    [Required]
    public int CategoryId { get; set; }
}
