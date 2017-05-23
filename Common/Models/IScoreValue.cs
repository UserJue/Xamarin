using System;

namespace Common.Models
{
    public interface IScoreValue : IComparable
    {
        int Compare(IScoreValue b);
        int SortOrder { get; set; }
        bool Descendig { get; set; }
        object Value { get; }
        string Name { get; }
        TimeSpan UsedTime { get; }
        bool LoadFromString(string value);
        string SaveString();
   }
}
