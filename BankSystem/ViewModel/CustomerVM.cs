using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankSystem.Model;

namespace BankSystem.ViewModel
{
    class CustomerVM : INotifyPropertyChanged
    {
        private Customer customer;
        public string Id { get { return this.customer.Id; } }
        
        public string OtherName { get; set; } //фамилия/форма собственности юр. лица
        public string LegalId { get; set; } // номер удостоверения личности или регистрации фирмы
        public string Phone { get; set; } //контактный телефон

        public CustomerVM(Customer Customer)
        {
            this.customer = Customer;
        }

        public string Name 
        { 
            get
            {
                return this.customer.Name;
            }

            set 
            {
                if (value != this.customer.Name)
                {
                    this.customer.Name = value;
                    OnPropertyChanged("Name");
                }
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

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
