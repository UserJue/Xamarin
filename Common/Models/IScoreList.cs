using System.Collections.Generic;

namespace Common.Models
{
    public interface IScoreList<T> : ICollection<IScore<T>>  where T : IScoreValue , new()
    {
        bool Sort(int sort);
        bool SaveList();
        bool Publish(IScore<T> score);

    }
}
