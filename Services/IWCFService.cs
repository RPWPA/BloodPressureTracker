using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;

namespace Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWCFService" in both code and config file together.
    [ServiceContract]
    public interface IWCFService
    {
        [OperationContract(IsOneWay = true)]
        void UpdateInformation(int id, string name, int weight, int age, char gender);


        [OperationContract(IsOneWay = false)]
        DataTable ViewInfo(int id);

        [OperationContract]
        DataTable Viewbloodhistory(int id);

        [OperationContract]
        string ViewDiet(int id);
    }
}
