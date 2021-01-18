using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankSystem.Model;
using System.Diagnostics;
using System.Collections.Specialized;

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

        public ObservableCollection<CustomerVM> Customers { get; private set; }

        public ObservableCollection<AccountVM> Accounts { get; private set; }
      
        public IList<AccountType> AccountTypeList
        {
            get
            {
                return Enum.GetValues(typeof(AccountType)).Cast<AccountType>().ToList<AccountType>();
            }
        }


        private Type typeOfCustomer;

        public DivisionVM(Division Department)
        {
            
            this.division = Department;
            this.typeOfCustomer = Department.GetType().GetGenericArguments()[0];
            //this.Customers = new ObservableCollection<CustomerVM>();
            Header1 = "Имя";
            Header2 = "Фамилия";
            Header3 = "Паспорт";
            Header4 = "Телефон";

            switch (typeOfCustomer.Name)
            {
                case "Entity":

                    this.Customers = new ObservableCollection<CustomerVM>(
                        (from c in (this.division as Department<Entity>).Customers
                         select new CustomerVM(c)).ToList()
                        );

                    Header1 = "Наименование";
                    Header2 = "Форма";
                    Header3 = "ОГРН";
                    break;

                case "Person":
                    this.Customers = new ObservableCollection<CustomerVM>(
                        (from c in (this.division as Department<Person>).Customers
                         select new CustomerVM(c)).ToList()
                        );
                    break;

                case "Vip":
                    this.Customers = new ObservableCollection<CustomerVM>(
                        (from c in (this.division as Department<Vip>).Customers
                         select new CustomerVM(c)).ToList()
                        );
                    break;
                default:
                    this.Customers = new ObservableCollection<CustomerVM>();
                    break;
            }


            
            //if (Department is Department<Entity>)
            //{
            //    Header1 = "Наименование";
            //    Header2 = "Форма";
            //    Header3 = "ОГРН";

            //    foreach (var item in (Department as Department<Entity>).Customers)
            //    {
            //        this.Customers.Add(new CustomerVM(item));

            //    }
            //}


            //else if (Department is Department<Person>)
            //{
            //    Header1 = "Имя";
            //    Header2 = "Фамилия";
            //    Header3 = "Паспорт";

            //    foreach (var item in (Department as Department<Person>).Customers)
            //    {
            //        this.Customers.Add(new CustomerVM(item));

            //    }
            //}

            //else if (Department is Department<Vip>)
            //{
            //    Header1 = "Имя";
            //    Header2 = "Фамилия";
            //    Header3 = "Паспорт";

            //    foreach (var item in (Department as Department<Vip>).Customers)
            //    {
            //        this.Customers.Add(new CustomerVM(item));


            //    }
            //}

            this.Accounts = new ObservableCollection<AccountVM>(); //не нужна тут обзервабл
            foreach (var item in this.division.Accounts)
            {
                this.Accounts.Add(new AccountVM(item));
            }
            INotifyCollectionChanged notifyAccountsChanged = this.division.Accounts;
            notifyAccountsChanged.CollectionChanged += RereadAcconts;
        }


        public CustomerVM SelectedCustomer
        {
            get
            {
                return Customers.FirstOrDefault(x => x.IsSelected);
            }

        }



       

        public AccountVM SelectedAccount
        {
            get
            {
                return Accounts.FirstOrDefault(x => x.IsSelected);
            }
        }

        public ObservableCollection<AccountVM> CustomersAccounts //не нужна обзервабл (реадонли лист)
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

        public void CreateCustomer(string name, string otherName, string legalId, string phone)
        {
            division.CreateCustomer(name, otherName, legalId, phone);
            this.OnPropertyChanged("Customers");
        }

        public void Put(string bic, decimal sum)
        {
            division.Put(bic, sum);

        }

        public void Withdraw(string bic, decimal sum)
        {
            division.Withdraw(bic, sum);
        }

        public void CloseAccount(string bic)
        {
            division.CloseAccount(bic);
        }

        public void OpenAccount(AccountType type, CustomerVM customerVM)
        {
            division.OpenAccount(type, customerVM.GetCustomer());

        }

        public void Transfer(string senderBic, string beneficiaryBic, decimal sum, string detailes = "Перевод клиенту банка")
        {
            division.Transfer(senderBic, beneficiaryBic, sum, detailes);
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

        private void RereadAcconts(object sender, NotifyCollectionChangedEventArgs e)
        {
            Accounts = new ObservableCollection<AccountVM>();
            foreach (var item in division.Accounts)
            {
                this.Accounts.Add(new AccountVM(item));
            }

            OnPropertyChanged("Accounts");
            OnPropertyChanged("CustomersAccounts");
        }

    }


     
}
