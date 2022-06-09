using System.ComponentModel.DataAnnotations;

namespace App.Domain;

public class Entry :BaseEntity
{
    // public Guid Id { get; set; }

    [MaxLength(128)]
    public string Name { get; set; } = default!;
    public bool AgreeToTerms { get; set; }
    public ICollection<EntrySector> Sectors { get; set; }
}