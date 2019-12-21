using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IEmailService" in both code and config file together.
    [ServiceContract]
    public interface IEmailService
    {
        [OperationContract(IsOneWay = true)]
        void remind_userByEmail(string email, DateTime Endtime);
        [OperationContract(IsOneWay = false)]
        DateTime get_time();
    }
}
