using Microsoft.AspNetCore.Identity;
using SpaceClopedia.Models;

namespace SpaceClopedia.Models
{
    public class Utilizator
    {
        public int Id { get; set; }
        public string NumeUtilizator { get; set; }
        //Rol -- 0(admin), 1(moderator), 2(utlizator obisnuit)
        public string Parola { get; set; }
        public int Rol { get; set; }
        public DateTime DataInregistrare { get; set; }

        public Utilizator(int _Id, string _NumeUtilizator, string _Parola, int _Rol, DateTime _DataInregistrare)
        {
            Id = _Id;
            NumeUtilizator = _NumeUtilizator;
            Parola = _Parola;
            Rol = _Rol;
            DataInregistrare = _DataInregistrare;
        }

    }
}
