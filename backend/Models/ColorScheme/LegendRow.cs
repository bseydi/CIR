namespace backend.Models.ColorScheme
{
    public class LegendRow
    {
        public int LegendRowId { get; set; }
        public int LegendId { get; set; }
        public string HexColor { get; set; }
        public string Text { get; set; }
        public int LevelId { get; set; }
        public int ValueId { get; set; }
    }
}
