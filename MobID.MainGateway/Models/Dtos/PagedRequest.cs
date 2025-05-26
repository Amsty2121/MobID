using System.ComponentModel.DataAnnotations;

namespace MobID.MainGateway.Models.Dtos;

public class PagedRequest
{
    [Range(0, int.MaxValue)]
    public int PageIndex { get; set; }

    [Range(1, 5000)]
    public int PageSize { get; set; }
}
