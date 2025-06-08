using System.ComponentModel.DataAnnotations;

public class OrganizationUpdateReq
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public Guid? OwnerId { get; set; }
}