using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace BankSystem.Model
{
    

    public class Department<TCustomer> : Division //добавить конструктор
    {
        public ObservableCollection<TCustomer> Customers { get; private set; }
        

        // логика, которая зависит от типа клиента...
        

       


    }
}

