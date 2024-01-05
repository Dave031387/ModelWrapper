namespace TestModels
{
    public class TestModel1
    {
        public const int TM1Property1MaxValue = 10;
        public const int TM1Property1MinValue = 1;
        public static readonly DateTime TM1Property3MaxValue = new(2000, 12, 31);
        public static readonly DateTime TM1Property3MinValue = new(1900, 1, 1);

        public int TM1Property1 { get; set; }

        public string TM1Property2 { get; set; } = string.Empty;

        public DateTime? TM1Property3 { get; set; }

        public bool TM1Property4 { get; set; }
    }
}