using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Domain;
using WebApp.Models;

namespace WebApp.Controllers;

public class EntryController : Controller
{
    private readonly AppDbContext _context;


    public EntryController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Entry/Create

    /// <summary>
    /// Gets the list of sectors for a new entry
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> Create()
    {
        var sectors = await GetSectors();

        var vm = new EntryModel
        {
            AllSectors = new SelectList(sectors, nameof(Sector.Id), nameof(Sector.Name))
        };

        return View(vm);
    }

    // POST: Entry/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

    /// <summary>
    /// Creates a new entry with the person name and selected sectors
    /// </summary>
    /// <param name="entry">Supply data for the entry</param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EntryModel entry)
    {
        if (ModelState.IsValid)
        {
            var entity = new Entry
            {
                Name = entry.Name,
                AgreeToTerms = entry.AgreeToTerms,
            };
            _context.Entries.Add(entity);
            await _context.SaveChangesAsync();

            foreach (var each in entry.SelectedSectors)
            {
                var entrySector = new EntrySector
                {
                    EntryId = entity.Id,
                    SectorId = each
                };

                _context.EntrySectors.Add(entrySector);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Edit), new {id = entity.Id});
        }

        var sectors = await GetSectors();
        entry.AllSectors = new SelectList(sectors, nameof(Sector.Id), nameof(Sector.Name));

        return View(entry);
    }

    // GET: Entry/Edit/5

    /// <summary>
    /// Gets the data of a specified entry, including selected sectors and name of the person
    /// </summary>
    /// <param name="id">Id of the entry</param>
    /// <returns></returns>
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var entry = await _context.Entries
            .Include(e => e.Sectors)
            .ThenInclude(s => s.Sector)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (entry == null)
        {
            return NotFound();
        }

        var sectors = await GetSectors();

        List<Guid> selectedSectors = new List<Guid>();

        foreach (var sector in entry.Sectors)
        {
            selectedSectors.Add(sector.SectorId);
        }

        var vm = new EntryModel
        {
            Id = entry.Id,
            Name = entry.Name,
            AgreeToTerms = entry.AgreeToTerms,
            SelectedSectors = selectedSectors,
            AllSectors = new MultiSelectList(sectors, nameof(Sector.Id), nameof(Sector.Name))
        };

        return View(vm);
    }


    // POST: Entry/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

    /// <summary>
    /// Updates the entry and the selected sectors list
    /// </summary>
    /// <param name="id">Id of the entry</param>
    /// <param name="entry">Supply updated data for the entry</param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EntryModel entry)
    {
        if (id != entry.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var entryDb = await _context.Entries.Include(e => e.Sectors).FirstOrDefaultAsync(e => e.Id == id);

            if (entryDb == null)
            {
                return NotFound();
            }

            entryDb.Name = entry.Name;
            entryDb.AgreeToTerms = entry.AgreeToTerms;

            _context.Entries.Update(entryDb);
            await _context.SaveChangesAsync();

            foreach (var sector in entryDb.Sectors)
            {
                _context.EntrySectors.Remove(sector);
            }

            await _context.SaveChangesAsync();

            foreach (var sector in entry.SelectedSectors)
            {
                var entrySector = new EntrySector
                {
                    EntryId = entry.Id.Value,
                    SectorId = sector
                };

                _context.EntrySectors.Add(entrySector);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Edit), new {id = entry.Id});
        }

        return View(entry);
    }

    private List<Sector> _sectors = new();

    /// <summary>
    /// Retrieves all the sectors from the database and formats the sector names
    /// to display correctly in a multi select list box
    /// </summary>
    /// <returns>Formatted list of sectors</returns>
    private async Task<List<Sector>> GetSectors()
    {
        List<Sector> sectorsDb = await _context.Sectors.ToListAsync();
        AddPadding(sectorsDb);

        return _sectors;
    }

    /// <summary>
    /// Recursively adds padding to the left of the sector name depending on
    /// the tree depth of the sector 
    /// </summary>
    /// <param name="items">List of sectors to format</param>
    /// <param name="level">The depth level of the sectors tree</param>
    private void AddPadding(ICollection<Sector> items, int level = 0)
    {
        var padding = new string('\u00A0', level * 2);
        foreach (var item in items)
        {
            item.Name = padding + item.Name;
            if (!_sectors.Contains(item))
            {
                _sectors.Add(item);
            }

            if (item.SubItems?.Count > 0)
            {
                AddPadding(item.SubItems, level + 1);
            }
        }
    }
}