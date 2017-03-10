using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HP41CV.Models
{
    public static class ActionModels
    {
        private static readonly Dictionary<string, ActionModel> actionDictionary = new Dictionary<string, ActionModel>();
        private static readonly Dictionary<string, ActionModel> extentionActionDictionary = new Dictionary<string, ActionModel>();

        public static ActionModel GetExtensionActionModel(string key)
        {
            if (extentionActionDictionary.ContainsKey(key))
                return extentionActionDictionary[key];
            return null;
        }

        public static ActionModel GetActionModel(string key)
        {
            if (actionDictionary.ContainsKey(key))
                return actionDictionary[key];
            return null;
        }

        public static Dictionary<string, ActionModel>.ValueCollection.Enumerator GetEnumerator(int index)
        {
            if (index == 1)
                return extentionActionDictionary.Values.GetEnumerator();
            if (index == 2)
                return extentionActionDictionary.Values.GetEnumerator();
            return actionDictionary.Values.GetEnumerator();
        }

        public static void AddExtension(string key, string accessory, ActionModel value)
        {
            if (extentionActionDictionary.ContainsKey(key))
            {
                extentionActionDictionary[key] = value;
            }
            else
            {
                extentionActionDictionary.Add(key, value);
            }
            value.Accessory = accessory;
        }

        public static void Add(string key, ActionModel value)
        {
            if (actionDictionary.ContainsKey(key))
            {
                actionDictionary[key] = value;
            }
            else
            {
                actionDictionary.Add(key, value);
            }
        }

        public static ICollection<string> GetActions(Func<ActionModel, bool> predicate)
        {
            ActionModel lastKey = null;
            var result = new Collection<string>();
            foreach (var actionModel in actionDictionary.Values)
            {
                if (predicate(actionModel) && (actionModel != lastKey))
                {
                    result.Add(actionModel.Text);
                    lastKey = actionModel;
                }
            }
            return result;
        }

        public static ICollection<string> GetExtentionActions(Func<ActionModel, bool> predicate)
        {
            ActionModel lastKey = null;
            var result = new Collection<string>();
            foreach (var actionModel in extentionActionDictionary.Values)
            {
                if (predicate(actionModel) && (actionModel != lastKey))
                {
                    result.Add(actionModel.Text);
                    lastKey = actionModel;
                }
            }
            return result;
        }

        public static ICollection<Tuple<string,string>> GetActionDescritions(Func<ActionModel, bool> predicate)
        {
            ActionModel lastKey = null;
            var result = new Collection<Tuple<string, string>>();
            foreach (var actionModel in actionDictionary.Values)
            {
                if (predicate(actionModel) && (actionModel != lastKey))
                {
                    result.Add(new Tuple<string, string>(actionModel.TextD,actionModel.Description));
                    lastKey = actionModel;
                }
            }
            return result;
        }
    }
}
