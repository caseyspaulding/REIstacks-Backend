using Microsoft.EntityFrameworkCore;
using REIstacks.Domain.Entities.Auth;
using REIstacks.Domain.Entities.Billing;
using REIstacks.Domain.Entities.Blog;
using REIstacks.Domain.Entities.CRM;
using REIstacks.Domain.Entities.Deals;
using REIstacks.Domain.Entities.Marketing;
using REIstacks.Domain.Entities.Organizations;
using REIstacks.Domain.Entities.Properties;
using REIstacks.Domain.Entities.Tasks;
using REIstacks.Domain.Entities.UploadLeads;
using REIstacks.Domain.Entities.User;

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
    public DbSet<FieldMappingTemplate> FieldMappingTemplates { get; set; }
    public DbSet<ImportJob> ImportJobs { get; set; } // Note: Changed from ImportJob to ImportJobs for consistency
    public DbSet<ImportError> ImportErrors { get; set; }
    // CRM Entities
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<Deal> Deals { get; set; }
    public DbSet<Communication> Communications { get; set; }
    public DbSet<TaskItem> TaskItems { get; set; }
    public DbSet<CampaignContact> CampaignContacts { get; set; }
    public DbSet<PropertyDocument> PropertyDocuments { get; set; }
    public DbSet<DealDocument> DealDocuments { get; set; }
    public DbSet<PropertyActivity> PropertyActivities { get; set; }
    public DbSet<PropertyInteractionCount> PropertyInteractionCounts { get; set; }
    public DbSet<ContactPhone> ContactPhones { get; set; }
    public DbSet<ContactEmail> ContactEmails { get; set; }
    public DbSet<PropertyNote> PropertyNotes { get; set; }
    public DbSet<PropertyCommunication> PropertyCommunications { get; set; }
    public DbSet<PropertyTag> PropertyTags { get; set; }
    public DbSet<List> Lists { get; set; }
    public DbSet<Offer> Offers { get; set; }
    public DbSet<PropertyList> PropertyLists { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<OfferDocument> OfferDocuments { get; set; }
    public DbSet<Board> Boards { get; set; }
    public DbSet<BoardPhase> BoardPhases { get; set; }
    public DbSet<PropertyBoard> PropertyBoards { get; set; }
    public DbSet<PropertyFile> PropertyFiles { get; set; }
    public DbSet<PhoneStatus> PhoneStatuses { get; set; }
    public DbSet<ContactStatus> ContactStatuses { get; set; }
    public DbSet<ContactTag> ContactTags { get; set; }
    public DbSet<PhoneTag> PhoneTags { get; set; }
    public DbSet<ProspectListPreset> ProspectListPresets { get; set; }
    public DbSet<DirectMailCampaign> DirectMailCampaigns { get; set; }
    public DbSet<SkipTraceActivity> SkipTraceActivities { get; set; }
    public DbSet<SkipTraceBreakdown> SkipTraceBreakdowns { get; set; }
    public DbSet<SkipTraceItem> SkipTraceItems { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DirectMailCampaign>()
            .HasOne(c => c.Property)
            .WithMany(p => p.DirectMailCampaigns)
            .HasForeignKey(c => c.PropertyId)
             .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ContactTag>()
    .HasOne(ct => ct.Contact)
    .WithMany(c => c.ContactTags)
    .HasForeignKey(ct => ct.ContactId)
    .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ContactTag>()
            .HasOne(ct => ct.Tag)
            .WithMany(t => t.ContactTags)
            .HasForeignKey(ct => ct.TagId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PhoneTag>()
    .HasOne(pt => pt.Tag)
    .WithMany(t => t.PhoneTags)  // You'll need to add this navigation property to Tag class
    .HasForeignKey(pt => pt.TagId)
    .OnDelete(DeleteBehavior.NoAction);

        // Create filter objects and serialize to JSON
        var absenteeOwnerFilter = new PropertyFilterCriteria { IsAbsenteeOwner = true };
        var vacantFilter = new PropertyFilterCriteria { IsVacant = true };
        var foreclosureFilter = new PropertyFilterCriteria { IsForeclosure = true };
        // Add more filters for each preset type...




        modelBuilder.Entity<PropertyFile>()
    .HasOne(pf => pf.Property)
    .WithMany(p => p.Files)
    .HasForeignKey(pf => pf.PropertyId)
    .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PropertyFile>()
            .HasOne(pf => pf.UploadedBy)
            .WithMany()
            .HasForeignKey(pf => pf.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Board>()
    .HasOne(b => b.Organization)
    .WithMany(o => o.Boards)
    .HasForeignKey(b => b.OrganizationId)
    .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<BoardPhase>()
            .HasOne(bp => bp.Board)
            .WithMany(b => b.Phases)
            .HasForeignKey(bp => bp.BoardId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PropertyBoard>()
            .HasOne(pb => pb.Property)
            .WithMany(p => p.PropertyBoards)
            .HasForeignKey(pb => pb.PropertyId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PropertyBoard>()
            .HasOne(pb => pb.Board)
            .WithMany(b => b.PropertyBoards)
            .HasForeignKey(pb => pb.BoardId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PropertyBoard>()
            .HasOne(pb => pb.Phase)
            .WithMany(bp => bp.PropertyBoards)
            .HasForeignKey(pb => pb.PhaseId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false);

        modelBuilder.Entity<PropertyActivity>()
    .HasOne(pa => pa.Property)
    .WithMany(p => p.Activities)
    .HasForeignKey(pa => pa.PropertyId)
    .OnDelete(DeleteBehavior.NoAction); // Prevent cascade delete issue

        modelBuilder.Entity<PropertyActivity>()
            .HasOne(pa => pa.User)
            .WithMany()
            .HasForeignKey(pa => pa.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PropertyActivity>()
            .HasOne(pa => pa.TargetUser)
            .WithMany()
            .HasForeignKey(pa => pa.TargetUserId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(false); // Not all activities will have a target user

        // Add an index for faster retrieval
        modelBuilder.Entity<PropertyActivity>()
            .HasIndex(pa => new { pa.PropertyId, pa.Timestamp });

        modelBuilder.Entity<PropertyList>()
    .HasOne(pl => pl.Property)
    .WithMany(p => p.PropertyLists)
    .HasForeignKey(pl => pl.PropertyId)
    .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PropertyList>()
            .HasOne(pl => pl.List)
            .WithMany(l => l.PropertyLists)
            .HasForeignKey(pl => pl.ListId);


        modelBuilder.Entity<List>()
    .HasOne(l => l.Organization)
    .WithMany(o => o.Lists)
    .HasForeignKey(l => l.OrganizationId);

        modelBuilder.Entity<Tag>()
            .HasOne(t => t.Organization)
            .WithMany(o => o.Tags)
            .HasForeignKey(t => t.OrganizationId);

        modelBuilder.Entity<PropertyTag>()
    .HasOne(pt => pt.Property)
    .WithMany(p => p.PropertyTags)
    .HasForeignKey(pt => pt.PropertyId)
  .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PropertyTag>()
     .HasOne(pt => pt.Organization)
     .WithMany(o => o.PropertyTags)
     .HasForeignKey(pt => pt.OrganizationId)
    .OnDelete(DeleteBehavior.NoAction);


        modelBuilder.Entity<OfferDocument>()
    .HasOne(od => od.User)
    .WithMany()
    .HasForeignKey(od => od.UserId)
    .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PropertyCommunication>()
       .HasOne(pc => pc.Property)
       .WithMany()
       .HasForeignKey(pc => pc.PropertyId)
       .OnDelete(DeleteBehavior.NoAction);

        // You might also need to modify the User relationship
        modelBuilder.Entity<PropertyCommunication>()
            .HasOne(pc => pc.User)
            .WithMany()
            .HasForeignKey(pc => pc.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        // Configure one-to-one relationship between Property and InteractionCounts
        modelBuilder.Entity<Property>()
            .HasOne(p => p.InteractionCounts)
            .WithOne(i => i.Property)
            .HasForeignKey<PropertyInteractionCount>(i => i.PropertyId);

        // Configure one-to-many relationship between Contact and ContactPhones
        modelBuilder.Entity<Contact>()
            .HasMany(c => c.PhoneNumbers)
            .WithOne(p => p.Contact)
            .HasForeignKey(p => p.ContactId);

        // Configure one-to-many relationship between Contact and ContactEmails
        modelBuilder.Entity<Contact>()
            .HasMany(c => c.EmailAddresses)
            .WithOne(e => e.Contact)
            .HasForeignKey(e => e.ContactId);

        // Configure relationship between Organization and Contacts
        modelBuilder.Entity<Contact>()
            .HasOne(c => c.Organization)
            .WithMany(o => o.Contacts)
            .HasForeignKey(c => c.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship between Organization and Properties
        modelBuilder.Entity<Property>()
            .HasOne(p => p.Organization)
            .WithMany(o => o.Properties)
            .HasForeignKey(p => p.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship between Property and OwnerContact
        modelBuilder.Entity<Property>()
            .HasOne(p => p.OwnerContact)
            .WithMany(c => c.Properties)
            .HasForeignKey(p => p.OwnerContactId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure relationship between Organization and Deals
        modelBuilder.Entity<Deal>()
            .HasOne(d => d.Organization)
            .WithMany(o => o.Deals)
            .HasForeignKey(d => d.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship between Deal and Property
        modelBuilder.Entity<Deal>()
            .HasOne(d => d.Property)
            .WithMany(p => p.Deals)
            .HasForeignKey(d => d.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure relationship between Deal and SellerContact
        modelBuilder.Entity<Deal>()
            .HasOne(d => d.SellerContact)
            .WithMany(c => c.Deals)
            .HasForeignKey(d => d.SellerContactId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure relationship between Deal and BuyerContact
        modelBuilder.Entity<Deal>()
            .HasOne(d => d.BuyerContact)
            .WithMany() // No navigation property back to Deal in Contact
            .HasForeignKey(d => d.BuyerContactId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure relationship between Organization and Communications
        modelBuilder.Entity<Communication>()
            .HasOne(c => c.Organization)
            .WithMany(o => o.Communications)
            .HasForeignKey(c => c.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship between Communication and Contact
        modelBuilder.Entity<Communication>()
            .HasOne(c => c.Contact)
            .WithMany(c => c.Communications)
            .HasForeignKey(c => c.ContactId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure relationship between Communication and Deal
        modelBuilder.Entity<Communication>()
            .HasOne(c => c.Deal)
            .WithMany() // No navigation property back to Communication in Deal
            .HasForeignKey(c => c.DealId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure relationship between Organization and Tasks
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Organization)
            .WithMany(o => o.TaskItems)
            .HasForeignKey(t => t.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship between Task and Contact
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Contact)
            .WithMany(c => c.Tasks)
            .HasForeignKey(t => t.ContactId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure relationship between Task and Property
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Property)
            .WithMany(p => p.TaskItems)
            .HasForeignKey(t => t.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure relationship between Task and Deal
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Deal)
            .WithMany(d => d.TaskItems)
            .HasForeignKey(t => t.DealId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure relationship between Organization and MarketingCampaigns
        modelBuilder.Entity<MarketingCampaign>()
            .HasOne(mc => mc.Organization)
            .WithMany(o => o.MarketingCampaigns)
            .HasForeignKey(mc => mc.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure many-to-many relationship between Campaigns and Contacts
        modelBuilder.Entity<CampaignContact>()
            .HasOne(cc => cc.Campaign)
            .WithMany(c => c.CampaignContacts)
            .HasForeignKey(cc => cc.CampaignId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<CampaignContact>()
            .HasOne(cc => cc.Contact)
            .WithMany() // No navigation property back to CampaignContact in Contact
            .HasForeignKey(cc => cc.ContactId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure relationship between PropertyDocument and Property
        modelBuilder.Entity<PropertyDocument>()
            .HasOne(pd => pd.Property)
            .WithMany(p => p.Documents)
            .HasForeignKey(pd => pd.PropertyId)
            .OnDelete(DeleteBehavior.NoAction);

        // Configure relationship between DealDocument and Deal
        modelBuilder.Entity<DealDocument>()
            .HasOne(dd => dd.Deal)
            .WithMany(d => d.Documents)
            .HasForeignKey(dd => dd.DealId)
            .OnDelete(DeleteBehavior.Cascade);

        // Add indexes for better query performance
        modelBuilder.Entity<Contact>()
            .HasIndex(c => new { c.OrganizationId, c.LastName, c.FirstName });

        modelBuilder.Entity<Property>()
            .HasIndex(p => new { p.OrganizationId, p.City, p.State, p.ZipCode });

        modelBuilder.Entity<Deal>()
            .HasIndex(d => new { d.OrganizationId, d.DealStatus });

        modelBuilder.Entity<TaskItem>()
            .HasIndex(t => new { t.OrganizationId, t.DueDate, t.TaskStatus });

        // Configure LandingPage
        modelBuilder.Entity<LandingPages>()
     .HasOne(lp => lp.Organization)
     .WithMany(o => o.LandingPages) // Add this to your Organization class if not already
     .HasForeignKey(lp => lp.OrganizationId)
     .OnDelete(DeleteBehavior.NoAction);


        modelBuilder.Entity<LandingPageComponent>()
    .HasOne(lpc => lpc.LandingPage)
    .WithMany(lp => lp.Components)  // If you have a ICollection<LandingPageComponent> in your LandingPages entity
    .HasForeignKey(lpc => lpc.LandingPageId)
    .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<FieldMappingTemplate>()
            .HasOne(fmt => fmt.Organization)
            .WithMany(o => o.FieldMappingTemplates) // You'll need to add this to your Organization class
            .HasForeignKey(fmt => fmt.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // First definition (correct)
        modelBuilder.Entity<ImportError>()
            .HasOne(ie => ie.Organization)
            .WithMany(o => o.ImportErrors)
            .HasForeignKey(ie => ie.OrganizationId)
            .OnDelete(DeleteBehavior.NoAction);
        // Configure relationship between LeadListFile and ImportJobs
        modelBuilder.Entity<ImportJob>()
            .HasOne(ij => ij.File)
            .WithMany(llf => llf.ImportJobs) // You'll need to add this to your LeadListFile class
            .HasForeignKey(ij => ij.FileId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ImportJob>()
    .HasOne(ij => ij.Organization)
    .WithMany(o => o.ImportJobs) // Add this property to Organization
    .HasForeignKey(ij => ij.OrganizationId)
    .OnDelete(DeleteBehavior.NoAction);

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

        modelBuilder.Entity<OfferDocument>()
    .HasOne(od => od.Offer)
    .WithMany(o => o.Documents)
    .HasForeignKey(od => od.OfferId)
    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Offer>()
    .HasOne(o => o.Property)
    .WithMany(p => p.Offers)
    .HasForeignKey(o => o.PropertyId)
    .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Offer>()
            .Property(o => o.OfferAmount)
            .HasPrecision(18, 2); // Adjust precision and scale as needed



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
       .OnDelete(DeleteBehavior.Restrict);

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

        modelBuilder.Entity<Contact>()
        .Property(c => c.StatusId)
        .IsRequired(false);

        // ContactPhone → Contact (required)
        modelBuilder.Entity<ContactPhone>()
            .HasOne(cp => cp.Contact)
            .WithMany(c => c.PhoneNumbers)    // your Contact class should have ICollection<ContactPhone> PhoneNumbers
            .HasForeignKey(cp => cp.ContactId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Contact>()
    .HasOne(c => c.Status)              // each Contact → one ContactStatus
    .WithMany(s => s.Contacts)          // ContactStatus → many Contacts (that property you showed)
    .HasForeignKey(c => c.StatusId)
    .OnDelete(DeleteBehavior.Cascade);

        // make StatusId on ContactPhone nullable
        modelBuilder.Entity<ContactPhone>()
            .Property(cp => cp.StatusId)
            .IsRequired(false);

        // hook up the optional FK → lookup
        modelBuilder.Entity<ContactPhone>()
            .HasOne(cp => cp.Status)               // each phone may have one status
            .WithMany(ps => ps.PhoneNumbers)       // the PhoneStatus.PhoneNumbers collection
            .HasForeignKey(cp => cp.StatusId)      // StatusId is the FK
            .OnDelete(DeleteBehavior.Cascade);     // or Restrict/SetNull per your needs

        // ContactPhone.StatusId is optional:
        modelBuilder.Entity<ContactPhone>()
            .Property(cp => cp.StatusId)
            .IsRequired(false);

        modelBuilder.Entity<PropertyList>()
    .HasOne(pl => pl.Organization)
    .WithMany(o => o.PropertyLists)
    .HasForeignKey(pl => pl.OrganizationId)
    .OnDelete(DeleteBehavior.NoAction);  // Or Restrict/NoAction per your needs

        modelBuilder.Entity<PropertyNote>()
    .HasOne(pn => pn.Organization)
    .WithMany(o => o.PropertyNotes)
    .HasForeignKey(pn => pn.OrganizationId)
    .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PropertyDocument>()
        .HasOne(pd => pd.Organization)
        .WithMany(o => o.PropertyDocuments)
        .HasForeignKey(pd => pd.OrganizationId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SkipTraceBreakdown>()
         .HasOne(b => b.SkipTraceActivity)
         .WithMany(a => a.Breakdown)
         .HasForeignKey(b => b.SkipTraceActivityId)
         .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<SkipTraceActivity>()
      .HasMany(a => a.Items)
      .WithOne(i => i.Activity)
      .HasForeignKey(i => i.SkipTraceActivityId)
      .OnDelete(DeleteBehavior.NoAction);

    }
}