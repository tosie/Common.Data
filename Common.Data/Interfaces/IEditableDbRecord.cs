using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Data {
    public interface IEditableDbRecord : IDbRecord {
        String Name { get; set; }
        IEditableDbRecord Duplicate();
        void Update();
        Boolean Delete();
    }
}
