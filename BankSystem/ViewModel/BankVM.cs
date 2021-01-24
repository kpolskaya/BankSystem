using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankSystem.Model;

namespace BankSystem.ViewModel
{
    public class BankVM : INotifyPropertyChanged
    {
        public ObservableCollection<DivisionVM> Departments { get; set; }

        private readonly Bank bank;
        public decimal Profit {get { return this.bank.Profit;} }

        public decimal Capital { get { return this.bank.Cash - ClientsFunds; } }

        public decimal ClientsFunds { get { return this.bank.ClientsFunds(); } }

        public decimal Cash { get { return this.bank.Cash; } }
        public BankVM(Bank Bank)
        {
            bank = Bank;
            //bank.ExampleCustomers();
            this.Departments = new ObservableCollection<DivisionVM>();
            foreach (var item in bank.Departments)
            {
                this.Departments.Add(new DivisionVM(item));
            }
            bank.BankBalanceChanged += UpdateTarget;
        }

        public DivisionVM SelectedItem
        {
            get
            {
                return this.Departments.FirstOrDefault(d => d.IsSelected);

            }
        }


        public void ClearSelectedCustomer()
        {
            SelectedItem.SelectedCustomer.IsSelected = false;
        }

        public void MonthlyCharge()
        {
            bank.MonthlyCharge();
            OnPropertyChanged("ClientsFunds");
            OnPropertyChanged("Capital");
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateTarget (string name)
        {
            OnPropertyChanged(name);
          
        }
    }
}
