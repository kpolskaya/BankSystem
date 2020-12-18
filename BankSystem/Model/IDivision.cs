namespace BankSystem.Model
{
    /// <summary>
    /// Сообщает о возникновении запроса на транзакцию
    /// </summary>
    public interface IDivision
    {

        /// <summary>
        /// Возникает при запросе транзакции
        /// </summary>
        event TransactionHandler TransactionRaised;

    }
}