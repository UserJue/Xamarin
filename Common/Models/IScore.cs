using System;

namespace Common.Models
{
    public interface IScore<T> : IComparable where T : IScoreValue 
    {
        DateTime TimeStamp { get; }
        string User { get; }
        T Value { get;  }
        bool Published { get; set; }
    }
}
