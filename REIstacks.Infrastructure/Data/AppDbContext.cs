using Microsoft.EntityFrameworkCore;
using REIstack.Domain.Models;
using REIstacks.Domain.Models;


namespace REIstacks.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<BlogPost> BlogPosts { get; set; }
    // Define your DbSet properties here for your entities
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Lead> Leads { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<PermissionEntity> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<OrganizationRole> OrganizationRoles { get; set; }
    public DbSet<StripeSubscription> StripeSubscriptions { get; set; }
    public DbSet<MarketingCampaign> MarketingCampaigns { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<ExternalAuth> ExternalAuths { get; set; }
    public DbSet<DomainVerification> DomainVerifications { get; set; }
    public DbSet<DomainVerification> PropertyRecord { get; set; }
    public DbSet<DomainVerification> ValueEstimate { get; set; }
    public DbSet<LandingPageComponent> LandingPageComponents { get; set; }
    public DbSet<Template> Templates { get; set; }
    public DbSet<TemplateComponent> TemplateComponents { get; set; }
    public DbSet<LandingPageLead> LandingPageLeads { get; set; }
    public DbSet<LandingPages> LandingPages { get; set; }
    public DbSet<LeadListFile> LeadListFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure LandingPage
        modelBuilder.Entity<LandingPages>()
     .HasOne(lp => lp.Organization)
     .WithMany(o => o.LandingPages) // Add this to your Organization class if not already
     .HasForeignKey(lp => lp.OrganizationId)
      .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<LandingPageComponent>()
    .HasOne(lpc => lpc.LandingPage)
    .WithMany(lp => lp.Components)  // If you have a ICollection<LandingPageComponent> in your LandingPages entity
    .HasForeignKey(lpc => lpc.LandingPageId)
    .OnDelete(DeleteBehavior.Cascade);


        // Configure unique slug per organization
        modelBuilder.Entity<LandingPages>()
            .HasIndex(p => new { p.OrganizationId, p.Slug })
            .IsUnique();

        // Define relationships (Organization ↔ StripeSubscription)
        modelBuilder.Entity<StripeSubscription>()
     .HasOne(s => s.Organization)
     .WithMany(o => o.StripeSubscriptions)
     .HasForeignKey(s => s.OrganizationId)
     .OnDelete(DeleteBehavior.SetNull);

        // Configure unique constraints
        modelBuilder.Entity<Organization>()
            .HasIndex(o => o.Subdomain)
            .IsUnique();

        modelBuilder.Entity<LandingPageComponent>()
    .HasIndex(lpc => new { lpc.LandingPageId, lpc.OrderIndex });

        modelBuilder.Entity<BlogPost>()
             .HasIndex(b => b.Slug)
             .IsUnique();

        modelBuilder.Entity<StripeSubscription>()
            .HasIndex(s => s.StripeSubscriptionId)
            .IsUnique();



        modelBuilder.Entity<Organization>()
    .Property(o => o.CustomDomain)
    .IsRequired(false);
        // Configure the index to ignore null values
        modelBuilder.Entity<Organization>()
            .HasIndex(o => o.CustomDomain)
            .IsUnique()
            .HasFilter("[CustomDomain] IS NOT NULL");

        // Configure relationship between Organization and UserProfiles
        modelBuilder.Entity<Organization>()
            .HasMany(o => o.UserProfiles)
            .WithOne(p => p.Organization)
            .HasForeignKey(p => p.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<StripeSubscription>()
    .Property(s => s.Status)
    .HasConversion<string>();

        modelBuilder.Entity<Organization>()
            .Property(o => o.SubscriptionStatus)
            .HasConversion<string>();
        // Configure relationship between Organization and Leads
        modelBuilder.Entity<Organization>()
            .HasMany(o => o.Leads)
            .WithOne(l => l.Organization)
            .HasForeignKey(l => l.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship between Organization and ActivityLogs
        modelBuilder.Entity<Organization>()
            .HasMany(o => o.ActivityLogs)
            .WithOne(a => a.Organization)
            .HasForeignKey(a => a.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship between Profile and Invitations
        modelBuilder.Entity<UserProfile>()
            .HasMany(p => p.InvitationsSent)
            .WithOne(i => i.InvitedByProfile)
            .HasForeignKey(i => i.InvitedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure relationship between Organization and Invitations
        modelBuilder.Entity<Organization>()
            .HasMany(o => o.Invitations)
            .WithOne(i => i.Organization)
            .HasForeignKey(i => i.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // ✅ Ensure Role enum is stored as a string, not an integer
        modelBuilder.Entity<Invitation>()
            .Property(i => i.Role)
            .HasConversion<string>();

        // Configure many-to-many relationship for Permissions
        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Organization)
            .WithMany(o => o.RolePermissions)
            .HasForeignKey(rp => rp.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship between Profile and RefreshTokens
        modelBuilder.Entity<UserProfile>()
            .HasMany(p => p.RefreshTokens)
            .WithOne(rt => rt.Profile)
            .HasForeignKey(rt => rt.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship between Profile and ExternalAuth
        modelBuilder.Entity<UserProfile>()
            .HasMany(p => p.ExternalAuths)
            .WithOne(ea => ea.Profile)
            .HasForeignKey(ea => ea.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship between Organization and DomainVerifications
        modelBuilder.Entity<Organization>()
            .HasMany(o => o.DomainVerifications)
            .WithOne(dv => dv.Organization)
            .HasForeignKey(dv => dv.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure unique constraint for domain verifications
        modelBuilder.Entity<DomainVerification>()
            .HasIndex(dv => new { dv.OrganizationId, dv.Domain })
            .IsUnique();

        // Configure unique constraint for external auth
        modelBuilder.Entity<ExternalAuth>()
            .HasIndex(ea => new { ea.Provider, ea.ExternalId })
            .IsUnique();

        modelBuilder.Entity<Organization>()
       .HasOne(o => o.Owner)
       .WithMany(p => p.OwnedOrganizations)
       .HasForeignKey(o => o.OwnerId)
       .OnDelete(DeleteBehavior.Restrict); // ✅ Prevents accidental deletion of owner
        // Configure enum conversions
        modelBuilder.Entity<UserProfile>()
            .Property(p => p.Role)
            .HasConversion<string>();

        modelBuilder.Entity<RolePermission>()
            .Property(rp => rp.Role)
            .HasConversion<string>();

        modelBuilder.Entity<RolePermission>()
            .Property(rp => rp.Permission)
            .HasConversion<string>();
    }
}