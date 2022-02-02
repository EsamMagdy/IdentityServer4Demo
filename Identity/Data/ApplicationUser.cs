using System;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Identity;
using System.ComponentModel.DataAnnotations.Schema;

public class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
        Id = Guid.NewGuid().ToString();
    }

    public string CrmUserId { get; set; }
    public string Image { get; set; }
    #region implement interface
    public string CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsDeactivated { get; set; }
    public string Name { get; set; }
    public DateTime? DeletedOn { get; set; }
    public string DeletedBy { get; set; }
    public string OwnerId { get; set; }
    public ApplicationUser Owner { get; set; }
    public DateTime? LastLogin { get; set; }
    [NotMapped]
    public override string NormalizedEmail { get => base.NormalizedEmail; set => base.NormalizedEmail = value; }
    [NotMapped]
    public override string ConcurrencyStamp { get => base.ConcurrencyStamp; set => base.ConcurrencyStamp = value; }
    [NotMapped]
    public override string NormalizedUserName { get => base.NormalizedUserName; set => base.NormalizedUserName = value; }
    [NotMapped]
    public override bool LockoutEnabled { get => base.LockoutEnabled; set => base.LockoutEnabled = value; }
    [NotMapped]
    public override DateTimeOffset? LockoutEnd { get => base.LockoutEnd; set => base.LockoutEnd = value; }
    #endregion


}
