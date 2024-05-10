
namespace SpaceClopedia.Models
{
    public class Domeniu
    {
        public int Id { get; set; }
        public string NumeDomeniu { get; set; }

        public Domeniu(int _Id, string _NumeDomeniu)
        {
            Id = _Id;
            NumeDomeniu = _NumeDomeniu;
        }
    }
}
