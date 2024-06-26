﻿using Microsoft.AspNetCore.Identity;
using SpaceClopedia.Logic;
using SpaceClopedia.Models;

namespace SpaceClopedia.Models
{
    public class UtilizatorModel
    {
        public int Id { get; set; }
        public string NumeUtilizator { get; set; }
        //Rol -- 0(admin), 1(moderator), 2(utlizator obisnuit)
        public string Parola { get; set; }
        public string? ParolaConfirm { get; set; }
        public Rol Rol { get; set; }
        public DateTime DataInregistrare { get; set; }

        //public UtilizatorModel(int _Id, string _NumeUtilizator, string _Parola, int _Rol, DateTime _DataInregistrare)
        //{
        //    Id = _Id;
        //    NumeUtilizator = _NumeUtilizator;
        //    Parola = _Parola;
        //    Rol = _Rol;
        //    DataInregistrare = _DataInregistrare;
        //}

    }
}
