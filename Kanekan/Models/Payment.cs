using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace KaneKan.Models;

[Table("payments")]
public class Payment
{
    [Key, Required, Column("id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required, Column("created_at", TypeName = "TEXT")]
    public DateTime CreatedAt { get; set; }

    [Column("payment_at", TypeName = "TEXT")]
    public DateTime? PaymentAt { get; set; }

    [Required, Column("is_paid", TypeName = "INTEGER")]
    public bool IsPaid { get; set; }

    [Required, Column("name")]
    public required string Name { get; set; }

    [Required, Column("amount")]
    public int Amount { get; set; }

    [Column("payment_method")]
    public string? PaymentMethod { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("categories_json")]
    public string? CategoriesJson { get; set; }
    
    [NotMapped]
    public List<string> Categories
    {
        get
        {
            if (CategoriesJson == null)
            {
                return [];
            }

            return JsonSerializer.Deserialize<List<string>>(CategoriesJson) ?? [];
        }
        set
        {
            if (value == null || value.Count == 0)
            {
                CategoriesJson = null;
                return;
            }

            CategoriesJson = JsonSerializer.Serialize(value);
        }
    }
}