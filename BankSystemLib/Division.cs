using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BankSystemLib
{
  
    /// <summary>
    /// Абстрактный класс для департаментов банка
    /// </summary>
    [System.Runtime.Serialization.DataContract]
    [KnownType(typeof(Department<>))]
    public abstract class Division
    {

        [DataMember]
        public string Id { get; protected set; }
        [DataMember]
        public string Name { get; protected set; }

        [DataMember]
        protected ObservableCollection<Account> accounts; // все счета департамента

        /// <summary>
        /// Публичный список счетов департамента
        /// </summary>
        public ReadOnlyObservableCollection<Account> Accounts { get; private set; }

        public Division()
        {
            this.accounts = new ObservableCollection<Account>();
            this.Accounts = new ReadOnlyObservableCollection<Account>(accounts);
        }

        public Division(string Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
            this.accounts = new ObservableCollection<Account>();
            this.Accounts = new ReadOnlyObservableCollection<Account>(accounts);
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
                    accounts.Add(new DebitAccount(this.Id, customer));
                    break;
                case AccountType.DepositAccount:
                    accounts.Add(new DepositAccount(this.Id, customer));
                    break;
                case AccountType.DepositAccountCapitalized:
                    accounts.Add(new DepositAccountСapitalized(this.Id, customer));
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
        public bool TryCredit(string bic, decimal sum, string detailes)
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
                throw new FraudOnRefundExeption();

            if (!TryCredit(t.SenderBic, t.Sum, "Возврат платежа (получатель не найден): " + t.Detailes))
                throw new NonexistantAccountExeption();
        }

        /// <summary>
        /// Закрывает месяц: рассчитывает и проводит все платежи и проценты
        /// </summary>
        public abstract void CalculateCharges();

        /// <summary>
        /// Вносит деньги на счет. Исключение, если счет не существует
        /// </summary>
        /// <param name="bic">номер счета</param>
        /// <param name="sum">сумма</param>
        public void Put(string bic, decimal sum)
        {
            Account account = GetAccountByBic(bic);
            if (account == null)
                throw new NonexistantAccountExeption();
            account.Credit(sum, "Пополнение через кассу");
            Processing.TransactionsQueue.Enqueue(new Transaction("00", bic, sum, "Пополнение через кассу"));
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
                Processing.TransactionsQueue.Enqueue(new Transaction(bic, "00", sum, detailes));
            return executed;
        }

        /// <summary>
        /// Закрывает счет клиента. Исключение, если счет не существует.
        /// </summary>
        /// <param name="bic"></param>
        public void CloseAccount(string bic)
        {
            string detailes = "Закрытие счета";
            Account account = GetAccountByBic(bic);
            if (account == null)
                throw new NonexistantAccountExeption();
            account.NotifyIfRemoved();
            Processing.TransactionsQueue.Enqueue(new Transaction(bic, "00", account.FullBalance(), detailes));
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

            if (senderAccount.Debit(sum, detailes))
                t = new Transaction(senderBic, beneficiaryBic, sum, detailes, TransactionType.Transfer);
            else
            {
                t = new Transaction(senderBic, beneficiaryBic, sum, detailes + " -- Отказано в операции", TransactionType.Transfer);
                t.Status = TransactionStatus.Failed;
            }
            Processing.TransactionsQueue.Enqueue(t);
        }

        /// <summary>
        /// Создает клиента с типом, соответствующим типу департамента
        /// </summary>
        /// <param name="name">Имя / Наименование </param>
        /// <param name="otherName">Фамилия / Форма собственности ю.л.</param>
        /// <param name="legalId"> паспорт / регистрация ю.л.</param>
        /// <param name="phone">Контакный телефон</param>
        public abstract void CreateCustomer(string name, string otherName, string legalId, string phone);

        /// <summary>
        /// Средства на счетах клиентов департамента
        /// </summary>
        /// <returns>Сумму остатков по всем клиентским счетам</returns>
        public decimal ClientsFunds() 
        {
            decimal total = 0;
            foreach (var item in this.accounts)
            {
                total += (decimal)item;
            }
            return total;
        }

        /// <summary>
        /// Обновляет подписки клиентов на сообщения об операциях по счетам
        /// </summary>
        public abstract void RefreshSubscriptions();

    }
}
