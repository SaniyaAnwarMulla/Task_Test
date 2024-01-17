using System;
using System.Collections.Generic;

namespace Task.DBContext;

public partial class TblUser
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public bool? IsAdmin { get; set; }

    public int? Age { get; set; }

    public string? Hobbies { get; set; }
}
