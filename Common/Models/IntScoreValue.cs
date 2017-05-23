using System;

namespace Common.Models
{
    public class IntScoreValue : ScoreValue
    {
        private int value;

        public IntScoreValue()
        {
            Descendig = true;
        }

        public IntScoreValue(string name, TimeSpan usedTime, int value)
        {
            Descendig = true;
            Name = name;
            SortOrder = 5;
            UsedTime = usedTime;
            Value = this.value = value;
        }

        public override int CompareTo(object obj)
        {
            var scoreValue = obj as IScoreValue;
            if (scoreValue != null) return Compare(scoreValue);
            throw new InvalidCastException();
        }

        public override int Compare(IScoreValue b)
        {
            var result = (Descendig) ? -1 : 1;
            var valueB = b.Value as int?;
            if (valueB == null) return result;
            if (SortOrder == 4)
            {
                if (b.Name.CompareTo(Name) < 0) return result;
                if (b.Name.CompareTo(Name) > 0) return -result;
            }
            if (SortOrder == 3)
            {
                if (UsedTime < b.UsedTime) return result;
                if (UsedTime > b.UsedTime) return result;
            }
            if (value > valueB) return result;
            if (value < valueB) return -result;
            if (UsedTime < b.UsedTime) return result;
            if (UsedTime > b.UsedTime) return result;
            return 0;
        }

        public override string SaveString()
        {
            var result = base.SaveString();
            return result + ";" + value;
        }

        public override bool LoadFromString(string stringValue)
        {
            var result = base.LoadFromString(stringValue);
            if (!result) return false;
            var parts = stringValue.Split(';');
            int help;
            result = int.TryParse(parts[2],out help);
            if (!result) return false;
            Value = value = help;
            return true;
        }
    }
}
