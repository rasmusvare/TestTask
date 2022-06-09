namespace App.Domain;

public class EntrySector : BaseEntity
{
    public Guid EntryId { get; set; }
    public Entry? Entry { get; set; }

    public Guid SectorId { get; set; }
    public Sector? Sector { get; set; }
}