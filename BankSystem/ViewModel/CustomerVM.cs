using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankSystem.Model;

namespace BankSystem.ViewModel
{
    public class CustomerVM : INotifyPropertyChanged
    {
        private readonly Customer customer;
        public string Id { get { return this.customer.Id; } }

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

        public string OtherName
        {
            get
            {
                return this.customer.OtherName;
            }

            set
            {
                if (value != this.customer.OtherName)
                {
                    this.customer.OtherName = value;
                    OnPropertyChanged("OtherName");
                }
            }
        }
        public string LegalId 
        {
            get
            {
                return this.customer.LegalId;
            }

            set
            {
                if (value != this.customer.LegalId)
                {
                    this.customer.LegalId = value;
                    OnPropertyChanged("LegalId");
                }
            }

        }
        public string Phone 
        {
            get
            {
                return this.customer.Phone;
            }

            set
            {
                if (value != this.customer.Phone)
                {
                    this.customer.Phone = value;
                    OnPropertyChanged("Phone");
                }
            }
        } 

        public CustomerVM(Customer Customer)
        {
            this.customer = Customer;
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

       
        public Customer GetCustomer()
        {
            return this.customer;
        }
    }
}
