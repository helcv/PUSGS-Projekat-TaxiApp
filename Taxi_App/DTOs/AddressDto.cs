using System.ComponentModel.DataAnnotations;

namespace Taxi_App;

public class AddressDto
{
    [RegularExpression(@"^\d+[a-zA-Z]?\s*,\s*[\w\s]+,\s*[\w\s]+,\s*[\w\s]+$", ErrorMessage = "Address should be in the format 'Number, Street, City, Country'")]
    public string StartAddress { get; set; }
    [RegularExpression(@"^\d+[a-zA-Z]?\s*,\s*[\w\s]+,\s*[\w\s]+,\s*[\w\s]+$", ErrorMessage = "Address should be in the format 'Number, Street, City, Country'")]
    public string FinalAddress { get; set; }
}
