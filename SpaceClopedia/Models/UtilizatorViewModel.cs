using SpaceClopedia.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml;

namespace SpaceClopedia.Models
{
    [NotMapped]
    public class UtilizatorViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int SelectedRole { get; set; }
    }
}
