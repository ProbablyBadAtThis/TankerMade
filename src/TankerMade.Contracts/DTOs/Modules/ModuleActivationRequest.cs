using System.ComponentModel.DataAnnotations;

namespace TankerMade.Contracts.DTOs.Modules;

public class ModuleActivationRequest
{
    [Required]
    [StringLength(100)]
    public string ModuleKey { get; set; } = string.Empty;
}
