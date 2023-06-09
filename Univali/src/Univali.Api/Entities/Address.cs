using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univali.Api.Entities;

public class Address
{
   [Key]
   [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
   public int Id { get; set; }
   [Required]
   [MaxLength(50)]
   public string Street { get; set; } = string.Empty;
   [Required]
   [MaxLength(50)]
   public string City { get; set; } = string.Empty;
   [ForeignKey("CustomerId")]
   public Customer? Customer {get; set;}
   public int CustomerId {get; set;}
}
