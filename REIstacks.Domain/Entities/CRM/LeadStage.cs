﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace REIstacks.Domain.Entities.CRM;

[Table("lead_stages")]
public class LeadStage
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }



    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    public int DisplayOrder { get; set; }

    public bool IsCompleted { get; set; }




    public virtual ICollection<Lead> Leads { get; set; } = new List<Lead>();
}