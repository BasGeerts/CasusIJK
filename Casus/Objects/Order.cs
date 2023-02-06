namespace Casus.Objects
{
    public class Order
    {
        private static int uniqueId = 1; //Would use GUID in a real application. Used this for readability in this scope. 

        public int Id { get; set; }
        public int KlantID { get; set; }

        public Order(int klantID)
        {
            Id = uniqueId;
            KlantID = klantID;
        }

        public static int incrementId()
        {
            uniqueId++;
            return uniqueId;
        }

    }
}
