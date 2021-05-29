using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankSystemLib;

namespace BankSystem.ViewModel
{
    public class AccountVM : INotifyPropertyChanged 
    {
        private readonly Account account;
        
        /// <summary>
        /// Последнее информационное сообщение пользователю по операциям с данным счетом
        /// </summary>
        public string Message { get; set; }
        public string Bic { get { return this.account.Bic; } }
        public decimal Balance { get { return this.account.Balance; } }
        public AccountType Type { get { return this.account.Type; } }
        public decimal Interest { get { return this.account.AccruedInterest; } }
        
        public AccountVM(Account Account)
        {
            this.account = Account;
            Subscribe(true); 
        }

        /// <summary>
        /// Обработчик события движения по счету
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void AccountMovement(Account sender, string message)
        {
            {
                OnPropertyChanged("Balance");
                this.Message = message;
                OnPropertyChanged("Message");
            }    
               
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

        /// <summary>
        /// Подписывает или отписывает представление счета на события(движение) его оригинала
        /// </summary>
        /// <param name="f">флаг подписки</param>
        public void Subscribe( bool f) 
        {
           if (f) this.account.Movement += AccountMovement;
           else this.account.Movement -= AccountMovement;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
     
    }
}
