namespace MessageHandler
{
    [MessageClass("TestMessage")]
    public class TestMessage
    {
        [MessageField]
        public int Field1 { get; set; }

        [MessageField(MessageFieldTypesEn.Array, "")]
        public int[] FieldArray { get; set; }
    }
}