using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankSystem.Model;

namespace BankSystem.ViewModel
{
    public class AccountVM : INotifyPropertyChanged
    {
        private readonly Account account;

        public string Bic { get { return this.account.Bic; } }
        public decimal Balance { get { return this.account.FullBalance();} }
        public AccountType Type { get { return this.account.Type; } }
        public decimal Interest { get { return this.account.AccruedInterest; } }
        public AccountVM(Account Account)
        {
            this.account = Account;
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //public override string ToString()
        //{
        //    return $"{this.Bic} : {this.Balance: ### ### ### ##0.00}";
        //}
    }
}
