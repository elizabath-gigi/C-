using System;
using System.Collections.Generic;

namespace UserManagement.Models;

public partial class User
{
    public int UserId { get; set; }
    public int IsDeleted { get; set; } = 0;

    public string UserName { get; set; } = null!;
    public string NameHindi { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = "User";

}