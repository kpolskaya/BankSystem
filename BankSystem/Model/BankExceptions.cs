using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Model
{
    public class CustomerIdFormatException : Exception
    {
        public CustomerIdFormatException() : base("Неправильный формат поля Customer.Id")
        {

        }

    }

    public class NonexistantAccountExeption : Exception
    {
        public NonexistantAccountExeption() : base("Счета с указанным номером не существует")
        {

        }
    }

    public class FraudOnRefundExeption : Exception
    {
        public FraudOnRefundExeption() : base("Попытка возмещения средств по состоявшейся транзакции!")
        {

        }
    }

    public class LegalIdDuplicateExeption : Exception
    {
        public LegalIdDuplicateExeption() : base("Клиент с такими официальными рег. данными уже существует")
        {

        }
    }

    public class SaveDataErrorException : Exception
    {
        public SaveDataErrorException() : base("Ошибка сохранения данных в файл")
        {

        }
    }
}
