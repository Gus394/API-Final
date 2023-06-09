using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univali.Api.Entities;

public class Customer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id {get; set;}
    [Required]
    [MaxLength(50)]
    public string Name {get; set;} = string.Empty;
    [MaxLength(11)]
    public string Cpf {get; set;} = string.Empty;
    public ICollection<Address> Addresses {get; set;} = new List<Address>();
}