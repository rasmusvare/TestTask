using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Models;

public class EntryModel
{
    public Guid? Id { get; set; }
    public MultiSelectList? AllSectors { get; set; }

    [Required(ErrorMessage = "Please enter your name")]
    [StringLength(128, ErrorMessage = "Name is too long")]
    [Display(Name = "Your name")]
    public string Name { get; set; } = default!;

    [Required]
    [MustBeTrue(ErrorMessage = "Please agree to the terms")]
    [Display(Name = "Agree to the terms")]
    public bool AgreeToTerms { get; set; }

    [Required(ErrorMessage = "Please select at least one sector")]
    [Display(Name = "Select sectors")]
    public ICollection<Guid> SelectedSectors { get; set; } = new List<Guid>();
}

public class MustBeTrueAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var entry = (EntryModel) validationContext.ObjectInstance;
        if (entry.AgreeToTerms == false)
        {
            return new ValidationResult(ErrorMessage ?? "Please accept the terms");
        }

        return ValidationResult.Success;
    }
}