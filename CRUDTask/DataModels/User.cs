using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRUDTask.DataModels;

public partial class User
{
    public int Id { get; set; }
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9.-_]{1,}@[a-zA-Z.-]{2,}[.]{1}[a-zA-Z]{2,}", ErrorMessage = "Please enter a valid Email Id")]
    public string? Email { get; set; }
    [Required]
    public string? StreetAddress { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }
    [Required]
    public string? UserName { get; set; }
    [Required]
    [StringLength(50, ErrorMessage = "Password should be 6 characters long", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$", ErrorMessage = "Password Must contain at least one UpperCase letter,lowercase letter,number and special character")]
    public string? Password { get; set; }
    [Required]
    [DataType(DataType.PhoneNumber)]
    [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Please enter valid phone number")]
    public string? Phone { get; set; }

    public DateTime? DeletedAt { get; set; }
}
