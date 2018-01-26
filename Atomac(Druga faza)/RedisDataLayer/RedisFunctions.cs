using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisDataLayer
{
    public class RedisFunctions
    {
        readonly RedisClient redis = new RedisClient(Config.SingleHost, Config.port);

        public string MakeHashId(string left, string right)
        {
            return left + ":" + right;
        }

        public bool CheckIfKeyExists(string key)
        {
            long ex = redis.Exists(key);
            if (ex == 0)
                return false;
            else return true;
        }

        public void CreateGameHash(string hashGame, string keyT1, string keyT2, string keyMessages)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("t1", keyT1);
            dictionary.Add("t2", keyT2);
            dictionary.Add("msg", keyMessages);
            redis.SetRangeInHash(hashGame, dictionary);
        }

        //rules = game:id:team:id:pravila
        public void CreateTeamHash(string keyGame, string keyTeam)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("status", "prepare");
            string rules = MakeHashId(keyGame, MakeHashId(keyTeam, "rules"));
            dictionary.Add("rules", rules);
            redis.SetRangeInHash(keyTeam, dictionary);
        }

        public string GetHashAttributeValue(string hashName, string key)
        {
            return redis.GetValueFromHash(hashName, key);
        }

        public bool SetHashAttributeValue(string hashName, string key, string value)
        {
            return redis.SetEntryInHash(hashName, key, value);
        }

        public void PushItemToList(string listName, string item)
        {
            redis.PushItemToList(listName, item);
        }

        public string GetItemFromList(string listName, int index)
        {
            return redis.GetItemFromList(listName, index);
        }

        public void RemoveItemFromList(string listName, string item)
        {
            redis.RemoveItemFromList(listName, item);
        }

        public void RemoveAllFromList(string listName)
        {
            redis.RemoveAllFromList(listName);
        }

        public long GetListCount(string listName)
        {
            return redis.GetListCount(listName);
        }

        public void DeleteKey(string key)
        {
            redis.Del(key);
        }
    }
}