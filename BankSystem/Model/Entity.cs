using System;


namespace BankSystem.Model
{
    public class Entity : Customer 
    {
        // Что-то у нас нет никакого функционала для клиентов. Вообще, непонятно, чем  они отличаются
        // персональные ставки по кредитам/депозитам сделать?

        public Entity(string Name, string OtherName, string LegalId, string Phone)
            : base(Name, OtherName, LegalId, Phone)
        {
           
        }
    }
}
