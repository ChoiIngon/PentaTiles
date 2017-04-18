using UnityEngine;
using UnityEngine.Analytics;
#if UNITY_EDITOR
using UnityEngine.Assertions;
#endif
using System.Collections;
using System.Collections.Generic;

public static class Quest
{
    public class Complete
    {
        public string id;
        public int date;
        public int count;
    };
    public class Trigger
    {
        public virtual bool IsAvailable() { return true; }
    }
    public class Progress
    {
        public string name;
        public string type;
        public string key;
        public int goal;
        public int progress;
        
        public Progress(string name, string type, string key, int goal)
        {
            this.name = name;
            this.type = type;
            this.key = key;
            this.goal = goal;
            this.progress = 0;
        }

        public virtual void Start()
        {
#if UNITY_EDITOR
            Assert.AreNotEqual(null, type);
            Assert.AreNotEqual("", type);
#endif
            if (false == Quest.updates.ContainsKey(type))
            {
                Quest.updates[type] = this._Update;
            }
            else
            {
                Quest.updates[type] += this._Update;
            }
        }

        public virtual void Update(string key)
        {
            if ("" == this.key || this.key == key)
            {
                progress++;
            }
        }

        public void _Update(string key)
        {
            int tmpProgress = progress;
            Update(key);
            if (progress >= goal)
            {
                Quest.updates[type] -= _Update;
            }
            if (tmpProgress != progress && null != Quest.onProgress)
            {
                Quest.onProgress(this);
            }
        }
        public virtual bool IsComplete()
        {
            if (progress >= goal)
            {
                return true;
            }
            return false;
        }
    }
    public class ProgressSelector : Progress
    {
        Progress [] progressList;
        public ProgressSelector(string name, string type, string key, int goal) : base(name, type, key, goal)
        {
        }

        public ProgressSelector(Progress[] progressList) : base("", "", "", 0)
        {
            this.progressList = progressList;
        }

        public override void Start()
        {
            foreach(Progress p in progressList)
            { 
                if (false == Quest.updates.ContainsKey(p.type))
                {
                    Quest.updates[p.type] = p._Update;
                }
                else
                {
                    Quest.updates[p.type] += p._Update;
                }
            }
        }

        public override void Update(string key)
        {
        }

        public override bool IsComplete()
        {
            foreach (Progress p in progressList)
            {
                if (p.progress >= p.goal)
                {
                    return true;
                }
            }
            return false;
        }
    }
    public class Data
    {
        public enum State
        {
            Invalid,
            StartWait,
            Started,
            Complete,
            Rewared,
            Max
        };

        public string id;
        public string name;
        public State state = State.Invalid;
        
        public List<Trigger> triggers = new List<Trigger>();
        public List<Progress> progress = new List<Progress>();

        public bool IsAvailable()
        {
            if (State.Invalid != state)
            {
                return false;
            }
            foreach (Trigger trigger in triggers)
            {
                if (false == trigger.IsAvailable())
                {
                    return false;
                }
            }
            state = State.StartWait;
            return true;
        }

        public void Start()
        {
            if (State.StartWait != state)
            {
                return;
            }
            foreach (Progress p in progress)
            {
                p.Start();
            }
            state = State.Started;
        }
        public bool IsComplete()
        {
            if (State.Started != state)
            {
                return false;
            }

            foreach (Progress p in progress)
            {
                if (false == p.IsComplete())
                {
                    return false;
                }
            }
            state = State.Complete;
            return true;
        }
    }
    
    public delegate void UpdateDelegate(string key);
    public delegate void ProgressDelegate(Progress progress);
    public delegate void CompleteDelegate(Data quests);

    public static CompleteDelegate onComplete;
    public static ProgressDelegate onProgress;

    public static Dictionary<string, UpdateDelegate> updates = new Dictionary<string, UpdateDelegate>();
    public static Dictionary<string, Complete> completes = new Dictionary<string, Complete>();
    public static Dictionary<string, Data> datas = new Dictionary<string, Data>();

    public static void Update(string type, string key)
    {
        if (false == updates.ContainsKey(type))
        {
            return;
        }
        if (null == updates[type])
        {
            return;
        }
        updates[type](key);
        foreach (var itr in datas)
        {
            Data questData = itr.Value;
            if (true == questData.IsComplete())
            {
                if (null != onComplete)
                {
                    onComplete(questData);
                }
            }
        }
    }
    public static Data Find(string questID)
    {
        return datas.ContainsKey(questID) ? datas[questID] : null;
    }
    public static List<Data> GetAvailableQuests()
    {
        List<Data> availableQuestDatas = new List<Data>();
        foreach (var v in datas)
        {
            Data availableQuestData = v.Value;
            if (true == availableQuestData.IsAvailable())
            {
                availableQuestDatas.Add(availableQuestData);                    
            }
        }
        return availableQuestDatas;
    }

    public static Data GetAvailableQuest()
    {
        foreach (var v in datas)
        {
            Data availableQuestData = v.Value;
            if (true == availableQuestData.IsAvailable())
            {
                return availableQuestData;
            }
        }
        return null;
    }
}
