using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceClopedia.Models
{
    [NotMapped]
    public class ArticolViewModel
    {
        public int Id { get; set; }
        public int DomeniuId { get; set; }
        public DomeniuModel? Domeniu { get; set; }
        public string Titlu { get; set; }
        public string Continut { get; set; }
        public string? Autor { get; set; }
        public string? AutorModificare { get; set; }
        public DateTime? DataCreare { get; set; }
        public DateTime? DataModificare { get; set; }
        public int? NumarVersiune { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}