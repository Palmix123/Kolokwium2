using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KolosGrupa1.Models;

[Table("titles")]
public class Title
{
    [Key]
    public int Id { get; set; }

    [Required] [MaxLength(100)] public string Name { get; set; } = null!;

    public List<CharacterTitle> CharacterTitles { get; set; } = new List<CharacterTitle>();
}