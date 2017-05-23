using System;

namespace Common.Models
{
    public abstract class ScoreValue : IScoreValue
    {
        public abstract int CompareTo(object obj);

        public abstract int Compare(IScoreValue b);

        int IScoreValue.SortOrder
        {
            get { return SortOrder; }
            set { SortOrder = value; }
        }

        bool IScoreValue.Descendig
        {
            get { return Descendig; }
            set { Descendig = value; }
        }

        public static int SortOrder { get; set; }
        public static bool Descendig { get; set; }
        public object Value { get; protected set; }
        public string Name { get; protected set; }
        public TimeSpan UsedTime { get; protected set; }

        public virtual string SaveString()
        {
            return Name + ";" + UsedTime;
        }

        public virtual bool LoadFromString(string value)
        {
            var parts = value.Split(';');
            if (parts.Length < 3) return false;
            Name = parts[0];
            UsedTime = TimeSpan.Parse(parts[1]);
            return true;
        }
    }
}
