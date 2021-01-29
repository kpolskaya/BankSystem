using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Model
{
    /// <summary>
    /// Делегат для события, инициирующего проводки по банковским счетам
    /// </summary>
    /// <param name="sender">Департамен, инициирующий транзакцию</param>
    /// <param name="t">Параметры транзакции</param>
    public delegate void TransactionHandler(Division sender, Transaction t);
    
    /// <summary>
    /// Абстрактный класс для департаментов банка
    /// </summary>
    [System.Runtime.Serialization.DataContract]
    [KnownType(typeof(Department<>))]
    public abstract class Division                      //TODO  везде проверки на null (GetAccountByBic) и расставить Exeptions
    {
        [DataMember]
        public string Id { get; protected set; }
        [DataMember]
        public string Name { get; protected set; }
        [DataMember]
        protected decimal fee; // плата за обслуживание - определяется типом клиентов департамента ВОЗМОЖНО УБРАТЬ 
        [DataMember]
        protected decimal rate; // базовая ставка по депозитам - определяется типом клиентов департамента ВОЗМОЖНО УБРАТЬ
        [DataMember]
        protected ObservableCollection<Account> accounts; // все счета департамента
        
        /// <summary>
        /// Список счетов департамента
        /// </summary>
        public ReadOnlyObservableCollection<Account> Accounts { get { return new ReadOnlyObservableCollection<Account>(this.accounts); } } //-ЕЩЕ ПОДУМАТЬ!!!
        
        [JsonConstructor]
        public Division(string Id, string Name, ObservableCollection<Account> accounts)
        {
            this.Id = Id;
            this.Name = Name;
            this.accounts = accounts;
            //this.Accounts = new ReadOnlyObservableCollection<Account>(this.accounts);
        }

        public Division()
        { 
        
        }

        public Division(string Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
            this.accounts = new ObservableCollection<Account>();
            //this.Accounts = new ReadOnlyObservableCollection<Account>(this.accounts);

        }

        /// <summary>
        /// Ищет клиентский счет по номеру
        /// </summary>
        /// <param name="Bic">Номер счета</param>
        /// <returns>Соответствующий клиентский счет или null если ничего не найдено</returns>
        protected Account GetAccountByBic(string Bic) 
        {
            return accounts.FirstOrDefault(x => x.Bic == Bic);
        }

        /// <summary>
        /// Открывает клиенту счет указанного типа
        /// </summary>
        /// <param name="type">Тип счета</param>
        /// <param name="customer">Клиент банка</param>
        public void OpenAccount(AccountType type, Customer customer)
        {
            switch (type)
            {
                
                case AccountType.DebitAccount:
                    accounts.Add(new DebitAccount( this.Id, customer));
                    break;
                case AccountType.DepositAccount:
                    accounts.Add(new DepositAccount(this.Id, customer));
                    break;
                case AccountType.DepositAccountCapitalized:
                    accounts.Add(new DepositAccountСapitalized (this.Id, customer));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// В случае успеха зачисляет средства на указанный счет
        /// </summary>
        /// <param name="bic">Номер счета</param>
        /// <param name="sum">Сумма</param>
        /// <param name="detailes">Детали платежа</param>
        /// <returns>true - если операция успешна, false - если не найден счет получателя</returns>
        public bool TryToCredit(string bic, decimal sum, string detailes)
        {
            Account account = GetAccountByBic(bic);
            bool executed = (account != null);
            if (executed)
                account.Credit(sum, detailes);
            return executed;
        }

        /// <summary>
        /// Возвращает деньги на клиенсткий счет по неисполненной транзакции
        /// </summary>
        /// <param name="t">параметры транзакции</param>
        public void Refund(Transaction t)
        {
            if (t.Status != TransactionStatus.Failed)
                throw new Exception("Попытка возмещения средств по состоявшейся транзакции!");

            if (!TryToCredit(t.SenderBic, t.Sum, "Возврат платежа (получатель не найден): " + t.Detailes))
                throw new Exception("Неизвестный счет получателя");
        }

        /// <summary>
        /// Закрывает месяц: рассчитывает и проводит все платежи и проценты
        /// </summary>
        public void CalculateCharges() // исправить начисление процентов по счетам с нулями на балансе!
        {
            foreach (var account in accounts)
            {

                if (account.Type != AccountType.DebitAccount)
                {
                    decimal Interest = account.ChargeInterest(rate);
                    OnTransactionRaised(this, new Transaction("99", account.Bic, Interest, "Начисление процентов"));
                }

                else
                    if (account.Debit(fee, "Плата за обслуживание")) 
                        OnTransactionRaised(this, new Transaction(account.Bic, "99", fee, "Плата за обслуживание"));
            }
        }

        /// <summary>
        /// Вносит деньги на счет
        /// </summary>
        /// <param name="bic">номер счета</param>
        /// <param name="sum">сумма</param>
        public void Put(string bic, decimal sum)
        {
            Account account = GetAccountByBic(bic);
            if (account == null)
                throw new Exception("Несуществующий счет");
            account.Credit(sum, "Пополнение через кассу");
            OnTransactionRaised(this, new Transaction("00", bic, sum, "Пополнение через кассу"));
        }

        /// <summary>
        /// В случае успеха снимает деньги со счета
        /// </summary>
        /// <param name="bic">номер счета</param>
        /// <param name="sum">сумма</param>
        /// <returns>true -  успех, или false - недостаточно средств/неверный счет</returns>
        public bool Withdraw(string bic, decimal sum)
        {
            string detailes = "Выдача наличных";
            Account account = GetAccountByBic(bic);
            bool executed = (account != null && account.Debit(sum, detailes));
            if (executed)
                OnTransactionRaised(this, new Transaction(bic, "00", sum, detailes));
            return executed;
        }

        /// <summary>
        /// Закрывает счет клиента
        /// </summary>
        /// <param name="bic"></param>
        public void CloseAccount(string bic) 
        {
            string detailes = "Закрытие счета";
            Account account = GetAccountByBic(bic);
            if (account == null)
                throw new Exception("Несуществующий счет");
            
            OnTransactionRaised(this, new Transaction(bic, "00", account.FullBalance(), detailes));
            this.accounts.Remove(account);
        }
 
        /// <summary>
        /// Переводит средства со счета на счет внутри банка
        /// </summary>
        /// <param name="senderBic">счет отправителя</param>
        /// <param name="beneficiaryBic">счет получателя</param>
        /// <param name="sum">Сумма</param>
        /// <param name="detailes">Детали платежа</param>
        public void Transfer(string senderBic, string beneficiaryBic, decimal sum, string detailes = "За просто так")
        {
            Account senderAccount = GetAccountByBic(senderBic);
            if (senderAccount == null)
                throw new Exception("Несуществующий счет");
            Transaction t; 
            if (senderAccount is DebitAccount && senderAccount.Debit(sum, detailes))
                t = new Transaction(senderBic, beneficiaryBic, sum, detailes, TransactionType.Transfer);
            else
            {
                t = new Transaction(senderBic, beneficiaryBic, sum, detailes + " -- Отказано в операции", TransactionType.Transfer); //проверить еще, как работает, не возвращает ли сумму ошибочно?
                t.Status = TransactionStatus.Failed;
            }
            OnTransactionRaised(this, t);
        }

        /// <summary>
        /// Событие при инициации транзакции
        /// </summary>
        public event TransactionHandler TransactionRaised;
      
        /// <summary>
        /// Вызов кода обработчика события транзакции
        /// </summary>
        /// <param name="sender">Инициатор транзакции</param>
        /// <param name="t">параметры транзакции</param>
        protected virtual void OnTransactionRaised(Division sender, Transaction t)
        {

            sender = this;
            TransactionRaised?.Invoke(sender, t);
        }
          
        /// <summary>
        /// Создает клиента с типом, соответствующим типу департамента
        /// </summary>
        /// <param name="name">Имя / Наименование </param>
        /// <param name="otherName">Фамилия / Форма собственности ю.л.</param>
        /// <param name="legalId"> паспорт / регистрация ю.л.</param>
        /// <param name="phone">Контакный телефон</param>
        public abstract void CreateCustomer(string name, string otherName, string legalId, string phone);
        
      
        //public abstract void CustomersForExample();

        //protected abstract void AccountsForExample();

        /// <summary>
        /// Средства на счетах клиентов департамента
        /// </summary>
        /// <returns>Сумму остатков по всем клиентским счетам</returns>
        public abstract decimal ClientsFunds();

        /// <summary>
        /// Обновляет подписки клиентов на сообщения об операциях по счетам
        /// </summary>
        public abstract void RefreshSubscriptions();
       
    }
}
