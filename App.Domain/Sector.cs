using System.ComponentModel.DataAnnotations;

namespace App.Domain;

public class Sector : BaseEntity
{
    // public Guid Id { get; set; }
    
    public Guid? ParentId { get; set; }
    public Sector? Parent { get; set; }
    [MaxLength(256)]
    public string Name { get; set; }
    [MaxLength(128)]
    public string Value { get; set; }
    public ICollection<Sector>? SubItems { get; set; }

}