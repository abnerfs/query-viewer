using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryViewer
{
    public class ConnectionInfo
    {
        public string Server { get; set; }
        public string User { get; set; }
        public string Database { get; set; }
        public bool Last { get; set; }
        public ConnectionTypeEnum ConnectionType { get; set; }

        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(Server))
                    return "";

                return $"{Server}/{Database} - {User}";
            }
        }

        public string StringSave()
        {
            var TmpLast = Last ? "last" : "";
            var Type = (int)ConnectionType;
            return $"{User};{Database};{Server};{Type};{TmpLast}#";
        }

        public override bool Equals(object ob)
        {
            var obj = ob as ConnectionInfo;
            if (obj == null)
                return false;

            return obj.Server == this.Server && obj.Database == this.Database && obj.User == this.User && obj.ConnectionType == this.ConnectionType;

        }
    }
}
