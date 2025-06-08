using System.ComponentModel.DataAnnotations;

public class OrganizationCreateReq
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public Guid OwnerId { get; set; }
}
