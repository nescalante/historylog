using System;

namespace HistoryLog
{
    public class Log<T>
    {
        public DateTime Date { get; set; }

        public string User { get; set; }

        public T Entity { get; set; }

        public override string ToString()
        {
            return (this.Date + " " + this.User).Trim();
        }
    }
}
