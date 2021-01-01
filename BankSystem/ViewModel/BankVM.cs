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
                  

        public BankVM()
        {
            bank = new Bank("BANK");
            bank.ExampleCustomers();
            this.Departments = new ObservableCollection<DivisionVM>();
            foreach (var item in bank.Departments)
            {
                this.Departments.Add(new DivisionVM(item));
            }
        }

        public DivisionVM SelectedItem
        {
            get
            {
                return this.Departments.FirstOrDefault(d => d.IsSelected);

            }
        }




        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
