using System;


namespace BankSystem.Model
{
    public class Entity : Customer 
    {
        static Entity()
        {
            fee = 30; // 40 30
            rate = 0.1m; // 24 10
        }
        

        public Entity(string Name, string OtherName, string LegalId, string Phone)
            : base(Name, OtherName, LegalId, Phone)
        {
           
        }
    }
}
