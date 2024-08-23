using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Models.EventDTO;

public class ChangeEventStatusDTO
{
    [Required(ErrorMessage = "Status is required.")]
    [RegularExpression("^(Pending|Active|Ended|Cancelled)$",
        ErrorMessage = "Status must be one of the following values: 'Pending', 'Active', 'Ended', 'Cancelled'.")]
    public string Status { get; set; }
}