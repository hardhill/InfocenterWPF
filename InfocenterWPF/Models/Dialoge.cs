using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfocenterWPF.Models
{
    public class Dialoge
    {
        public bool ComeIn { get; set; }
        public string MessageText { get; set; }
        public string Author { get; set; }
        public DateTime DtMsg { get; set; }

        public override string ToString()
        {
            string tt = DtMsg.ToString("HH:mm:ss");
            if (ComeIn)
            {
                return String.Format("[{0}] {1}\t({2})", Author, MessageText, tt);
            }
            else
            {
                return String.Format("<<{0}\t({1})", MessageText, tt);
            }
        }
    }
}
