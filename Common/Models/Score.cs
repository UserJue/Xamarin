using System;

namespace Common.Models
{
    public class Score<T> : IScore<T> where T : IScoreValue , new()
    {
        public DateTime TimeStamp { get; private set; }
        public string User { get; private set; }
        public T Value { get; private set; }

        public bool Published { get; set; }

        public Score(string user, T scoreValue)
        {
            User = user;
            Value = scoreValue;
            TimeStamp = DateTime.Now;
        }

        public Score(string user,DateTime timeStamp, T scoreValue)
        {
            User = user;
            Value = scoreValue;
            TimeStamp = timeStamp;
        }

        public Score()
        {
            
        }

        public int CompareTo(object obj)
        {
            var score = obj as Score<T>;
            if (score == null) return -1;
            return Value.Compare(score.Value);
        }
    }
}
