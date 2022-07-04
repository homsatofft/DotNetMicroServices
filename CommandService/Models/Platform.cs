using System.ComponentModel.DataAnnotations;

namespace CommandService.Models;

public class Platform
{
  [Key]
  [Required]
  public int Id { get; set; }
  [Required]
  public string Name { get; set; } = String.Empty;
  [Required]
  public int ExternalId { get; set; }

  public virtual List<Command>? Commands { get; set; }
}
