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
   
    public class DivisionVM : INotifyPropertyChanged
    {
        private readonly Division division;

        public string Id { get { return division.Id; } }

        public string Name { get { return division.Name; } }

        public string Header1 { get; }
        public string Header2 { get; }
        public string Header3 { get; }
        public string Header4 { get; }

      
       
        public IList<AccountType> AccountTypeList
        {
            get
            {
               return Enum.GetValues(typeof(AccountType)).Cast<AccountType>().ToList<AccountType>();
            }
        }

      

        public DivisionVM(Division Department)
        {
            this.division = Department;
            this.Customers = new ObservableCollection<CustomerVM>();

            Header4 = "Телефон";
            if (Department is Department<Entity>)
            {
                Header1 = "Наименование";
                Header2 = "Форма";
                Header3 = "ОГРН";
                
                foreach (var item in (Department as Department<Entity>).Customers)
                {
                    this.Customers.Add(new CustomerVM(item));

                }
            }


            else if (Department is Department<Person>)
            {
                Header1 = "Имя";
                Header2 = "Фамилия";
                Header3 = "Паспорт";
                
                foreach (var item in (Department as Department<Person>).Customers)
                {
                    this.Customers.Add(new CustomerVM(item));

                }
            }

            else if (Department is Department<Vip>)
            {
                Header1 = "Имя";
                Header2 = "Фамилия";
                Header3 = "Паспорт";
               
                foreach (var item in (Department as Department<Vip>).Customers)
                {
                    this.Customers.Add(new CustomerVM(item));
                    
                    //else
                    //{
                    //    return;
                    //}
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
            }

            this.Accounts = new ObservableCollection<AccountVM>();
            foreach (var item in Department.Accounts)
            {
                this.Accounts.Add(new AccountVM(item));
            }


        }

        public ObservableCollection<CustomerVM> Customers { get; private set; }

        public CustomerVM SelectedCustomer
        {
            get
            {
                return Customers.FirstOrDefault(x => x.IsSelected);
            }

        }



        public ObservableCollection<AccountVM> Accounts { get; private set; }

        public AccountVM SelectedAccount
        {
            get
            {
                return Accounts.FirstOrDefault(x => x.IsSelected);
            }
        }

        public ObservableCollection<AccountVM> CustomersAccounts
        {
            get
            {
                return new ObservableCollection<AccountVM>(
                    from a in Accounts
                    where a.Bic.Substring(2, 8) == SelectedCustomer.Id
                    select a
                    );
            }
        }

        public void Put(string bic, decimal sum)
        {
            division.Put(bic, sum);
        }

        public void OpenAccount(AccountType type, string departmentId, string customerId)
        {
            division.OpenAccount(type, departmentId, customerId);
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
