namespace SantaBot
{
    
        public class SantaLocation
        {
            public Address address { get; set; }
            public Position position { get; set; }
            public int energy { get; set; }
            public int presents { get; set; }
        }

        public class Address
        {
            public string municipality { get; set; }
        }

        public class Position
        {
            public float lon { get; set; }
            public float lat { get; set; }
        }

}
