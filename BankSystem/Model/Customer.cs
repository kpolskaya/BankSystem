namespace BankSystem.Model
{
    public class Customer
    {
        static int lastId;
        static Customer()
        {
            lastId = 0;
        }

        static int NextId()
        {
            return ++lastId;
        }
 




    }
}