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
    class BankVM : INotifyPropertyChanged
    {
        public ObservableCollection<DivisionVM> Departments { get;  set; }

        private Bank bank;

        public BankVM(Bank Bank)
        {
            this.Departments = new ObservableCollection<DivisionVM>();
            this.bank = Bank;
            foreach (var item in this.bank.Departments)
            {
                this.Departments.Add(new DivisionVM(item));
            }
                
        }


        public DivisionVM SelectedItem()
        {
            return this.Departments.FirstOrDefault(d => d.IsSelected);
        }




        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
