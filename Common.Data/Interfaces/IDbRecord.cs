using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubSonic.Repository;

namespace Common.Data {
    public interface IDbRecord {
        Int64 Id { get; set; }
        void InitializeWithDefaults(Object Tag);
        void AfterLoad();
        Boolean BeforeUpdate();
        Boolean BeforeDelete();
    }
}
