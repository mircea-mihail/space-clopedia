using SpaceClopedia.Models;
using System.Xml;

namespace SpaceClopedia.Models
{
    public class ArticolModel
    {
        public int Id { get; set; }
        public int DomeniuId { get; set; }
        public DomeniuModel? Domeniu { get; set; }
        public string Titlu { get; set; }
        public string Continut { get; set; }
        public string? TitluPoza { get; set; }
        public string? Autor { get; set; }
        public string? AutorModificare { get; set; }
        public DateTime? DataCreare { get; set; }
        public DateTime? DataModificare { get; set; }
        public int? NumarVersiune { get; set; }

        //public ArticolModel(int _Id, int _IdDomeniu, string _Titlu, string _Continut, string _TitluPoza, string _Autor,
        //                string _AutorModificare, DateTime _DataCreare, DateTime _DataModificare, int _NumarVersiune)
        //{
        //    Id = _Id;
        //    IdDomeniu = _IdDomeniu;
        //    Titlu = _Titlu;
        //    Continut = _Continut;
        //    TitluPoza = _TitluPoza;
        //    Autor = _Autor;
        //    AutorModificare = _AutorModificare;
        //    DataCreare = _DataCreare;
        //    DataModificare = _DataModificare;
        //    NumarVersiune = _NumarVersiune;            
        //}
    }
}
