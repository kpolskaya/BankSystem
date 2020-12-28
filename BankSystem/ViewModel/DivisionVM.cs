using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankSystem.Model;
using System.Diagnostics;

namespace BankSystem.ViewModel
{
    class DivisionVM : INotifyPropertyChanged
    {
        private Division division;

        public string Id { get { return division.Id; } }

        public string Name { get { return division.Name; } }

        public DivisionVM(Division Department)
        {
            this.division = Department;
            this.Customers = new ObservableCollection<CustomerVM>();

                          
                if (Department is Department<Entity>)
                foreach (var item in (Department as Department<Entity>).Customers)
                {
                    this.Customers.Add(new CustomerVM(item));
                }

                else if (Department is Department<Person>)
                foreach (var item in (Department as Department<Person>).Customers)
                {
                    this.Customers.Add(new CustomerVM(item));
                }

                else if(Department is Department<Vip>)
                foreach (var item in (Department as Department<Vip>).Customers)
                {
                    this.Customers.Add(new CustomerVM(item));
                }
            else
            {
                return;
            }
            //var dType = Department.GetType();
            //Type[] dTypeParameters = dType.GetGenericArguments();
            //Type cType = dTypeParameters[0];

            //switch (cType.Name)
            //{
            //    case "Entity":
            //        foreach (var item in (Department as Department<Entity>).Customers)
            //        {
            //            this.Customers.Add(new CustomerVM(item));
            //        }

            //        Debug.WriteLine("Entity");
            //        break;
            //}



        }



        public ObservableCollection<CustomerVM> Customers
        {
            get; private set;
        }



        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (value != this.isSelected)
                {
                    this.isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
