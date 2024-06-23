using System.ComponentModel.DataAnnotations;

namespace Taxi_App;

public class CreateMessageDto
{
    [Required]
    public string RecipientUsername { get; set; }
    [Required]
    public string Content { get; set; }
}
