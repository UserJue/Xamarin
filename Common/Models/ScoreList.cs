using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Models
{
    public class ScoreList<T> : IScoreList<T> where T : IScoreValue,IComparable, new()
    {
        private bool communicating;
        private ICollection<IScore<T>> list;
        private Task loader = null;
        private IStoreage storage;
        private const int maxSavedItem = 10;

        public ScoreList(IStoreage storage)// string name, IOnlineGameSettingModel settingModel = null)
        {
            this.storage = storage;
            list = new Collection<IScore<T>>();
            loader = new Task(LoadList);
            loader.Start();
        }

        public bool Sort(int sort)
        {
            if (sort == ScoreValue.SortOrder)
                ScoreValue.Descendig = !ScoreValue.Descendig;
            else
                ScoreValue.SortOrder = sort;

            var listNew = new Collection<IScore<T>>();
            foreach (var score in list)
            {
                bool inserted = false;
                for (var index = 0;!inserted && (index < listNew.Count); index++)
                    if (score.Value.Compare(listNew[index].Value) < 0)
                    {
                        listNew.Insert(index,score);
                        inserted = true;
                    }
                if (!inserted)
                    listNew.Add(score);
            }
            list = listNew;
            return true;
        }

        public void LoadList()
        {
            IScore<T> item;
            string key;
            string user;
            string sValue;
            for (int i = 1; i <= maxSavedItem; i++)
            {
                key = $"ScoreList{i}_TimeStamp";
                if (!storage.GetValue(key, out sValue)) break;
                DateTime time = DateTime.Parse(sValue);
                key = $"ScoreList{i}_User";
                if (!storage.GetValue(key, out user)) break;
                key = $"ScoreList{i}_Value";
                if (!storage.GetValue(key, out sValue)) break;
                var value = new T();
                value.LoadFromString(sValue);
                item = new Score<T>(user,time,value);
                list.Add(item);
            }
        }

        public bool SaveList()
        {
            var saveList = new List<IScore<T>>();
            string key;
            foreach (var score in list)
            {
                bool inserted = false;
                if (score.Value.Name == null) continue;
                for (var index = 0; !inserted && (index < saveList.Count); index++)
                    if (score.Value.Compare(saveList[index].Value) < 0)
                    {
                        saveList.Insert(index, score);
                        inserted = true;
                    }
                if (!inserted)
                    saveList.Add(score);
            }
            for (int index = 0; index < saveList.Count; index++)
            {
                key = $"ScoreList{index+1}_TimeStamp";
                storage.SetValue(key, saveList[index].TimeStamp.ToString());
                key = $"ScoreList{index+1}_User";
                storage.SetValue(key, saveList[index].User);
                key = $"ScoreList{index+1}_Value";
                storage.SetValue(key, saveList[index].Value.SaveString());
            }
            return true;
        }

        public bool Publish(IScore<T> score)
        {
            //if (settingModel == null) return false;
            //if (restService == null)
            //    restService = new SimpleRestService();
            //communicating = true;
            //restService.MakeRequest(settingModel.HostUri + "api/Score/"+ name +"Score", "PUT", score, Published, ErrorHandler);
            return true;
        }

        private void Published(string data)
        {
            communicating = false;
            var parts = data.Split('|');
            DateTime time;
            if (DateTime.TryParse(parts[0], System.Globalization.DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None,
                out time))
            {
                var score = list.FirstOrDefault(s => s.TimeStamp == time);
                if (score != null)
                {
                    score.Published = true;
                    SaveList();
                }
            }
        }

        IEnumerator<IScore<T>> IEnumerable<IScore<T>>.GetEnumerator()
        {
            if (loader != null) loader.Wait();
            return list.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            if (loader != null) loader.Wait();
            return list.GetEnumerator();
        }

        public virtual void Add(IScore<T> item)
        {
            if (loader != null) loader.Wait();
            list.Add(item);
            SaveList();
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(IScore<T> item)
        {
            return list.Contains(item);
        }

        public void CopyTo(IScore<T>[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public bool Remove(IScore<T> item)
        {
            return list.Remove(item);
        }

        public int Count { get; private set; }
        public bool IsReadOnly { get; private set; }

        void ErrorHandler(Exception e)
        {
            ;
        }
    }
}
