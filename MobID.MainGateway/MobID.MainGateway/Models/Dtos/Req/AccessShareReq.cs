using System.ComponentModel.DataAnnotations;

public class AccessShareReq
{
    [Required]
    public Guid AccessId { get; set; }

    [Required]
    public Guid SourceOrganizationId { get; set; }

    [Required]
    public Guid TargetOrganizationId { get; set; }
}
