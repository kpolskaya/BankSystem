using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BankSystem.Model;
using System.Windows.Shapes;
using BankSystem.ViewModel;
using BankSystemLib;

namespace BankSystem.View
{
    /// <summary>
    /// Логика взаимодействия для Transactions.xaml
    /// </summary>
    public partial class Transactions : Window
    {
        BankVM bank;
        static readonly DependencyProperty statementProperty;
        List<Transaction> Statement
        {
            get { return (List<Transaction>)GetValue(statementProperty); }
            set { SetValue(statementProperty, value); }
        }

        static Transactions()
        {
            statementProperty = DependencyProperty.Register(
                "Statement",
                typeof(List<Transaction>),
                typeof(Transactions));
        }

        public Transactions(object DataContext, AccountVM SelectedAccount)
        {
            this.bank = DataContext as BankVM;
            this.Statement =(
                    from t in bank.TransactionHistory
                    where t.SenderBic == SelectedAccount.Bic
                    || t.BeneficiaryBic == SelectedAccount.Bic
                    select t
                    ).ToList();

            InitializeComponent();
           
        }


    }
}
