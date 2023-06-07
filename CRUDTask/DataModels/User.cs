using System;
using System.Collections.Generic;

namespace CRUDTask.DataModels;

public partial class User
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? StreetAddress { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? Phone { get; set; }

    public DateTime? DeletedAt { get; set; }
}
