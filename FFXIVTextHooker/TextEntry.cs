namespace FFXIVTextHooker
{
    internal class TextEntry
    {
        public string Source;
        public string Time;
        public string Text;

        public TextEntry(string _Source, string _Time, string _Text)
        {
            Source = _Source;
            Time = _Time;
            Text = _Text;
        }
    }
}
