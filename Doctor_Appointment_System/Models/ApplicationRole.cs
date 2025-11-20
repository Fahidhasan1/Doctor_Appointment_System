using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Doctor_Appointment_System.Models
{
    // Application role based on ASP.NET Core Identity
    public class ApplicationRole : IdentityRole
    {
        // Extend later if you need Description, CreatedDate, etc.
    }
}
