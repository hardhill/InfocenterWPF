using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfocenterWPF.Models
{
    public class ContactUser
    {
        public string SessionId { get; set; }
        public string Winlogin { get; set; }
        public string Fa { get; set; }
        public string Im { get; set; }
        public string Ot { get; set; }
        public DateTime Bday { get; set; }
        public string Avatar { get; set; }
        public bool Active { get; set; }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", Fa, Im, Ot);
        }
    }
}
