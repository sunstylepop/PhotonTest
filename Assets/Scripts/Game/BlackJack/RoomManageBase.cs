using ExitGames.Client.Photon;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game
{
    public abstract class RoomManageBase<T> where T: RoomManageBase<T>, new()
    {
        private static RoomManageBase<T> _Instance;
        private static RoomManageBase<T> Instance { 
            get { 
                if(_Instance == null)
                {
                    _Instance = new T();
                }
                return _Instance; 
            } 
        }

        protected abstract Dictionary<string, RoomFilterType> roomFilter { get; }

        protected abstract RoomOptions DefaultRoomOption { get; }

        public static string GetSqlLobbyFilter(object obj)
        {
            return Instance._GetSqlLobbyFilter(obj);
        }

        public string _GetSqlLobbyFilter(object obj)
        {
            if (obj == null) return "";

            //ex. "C0 = 1 AND C1 <= 5"
            //若type為 字串使用比對, int使用 小於等於

            var properties = obj.GetType().GetProperties();

            List<string> _sql = new List<string>();
            foreach (var f in roomFilter)
            {
                var property = properties.FirstOrDefault(x => x.Name.Equals(f.Key));

                if(property != null)
                {
                    var filterName = f.Value.ToString();

                    if (f.Value == RoomFilterType.C0)
                    {
                        //先寫固定
                        _sql.Add($"{filterName} = {(int)property.GetValue(obj)}");
                    }
                    else if (property.PropertyType == typeof(string) || property.PropertyType.IsEnum)
                    {
                        _sql.Add($"{filterName} = \"{property.GetValue(obj).ToString()}\"");
                    }
                    else if (property.PropertyType.IsPrimitive || property.PropertyType.IsValueType)
                    {
                        _sql.Add($"{filterName} <= {(int)property.GetValue(obj)}");
                    }
                }
            }

            return string.Join(" AND ", _sql);
        }

        public static RoomOptions GetRoomOption(object obj)
        {
            return Instance._GetRoomOption(obj);
        }

        public virtual RoomOptions _GetRoomOption(object obj)
        {
            var roomOption = DefaultRoomOption;
            var properties = GetRoomProperties(obj);

            if (properties.Count > 0)
            {
                if (roomOption.CustomRoomProperties == null) roomOption.CustomRoomProperties = new Hashtable();

                foreach (var h in properties)
                {
                    roomOption.CustomRoomProperties.Add(h.Key, h.Value);
                }

                roomOption.CustomRoomPropertiesForLobby = properties.Keys.Select(x => x.ToString()).ToArray();
            }

            return roomOption;
        }




        private Hashtable GetRoomProperties(object obj)
        {
            if (roomFilter == null) return null;

            Hashtable props = null;
            var properties = obj.GetType().GetProperties();


            foreach (var f in roomFilter)
            {
                var property = properties.FirstOrDefault(x => x.Name.Equals(f.Key));

                if(property != null)
                {
                    var filterName = f.Value.ToString();
                    if (props == null) props = new Hashtable();

                    if (property.PropertyType == typeof(string) || property.PropertyType.IsEnum)
                    {
                        props.Add(filterName, property.GetValue(obj).ToString());
                    }
                    else if (property.PropertyType.IsPrimitive || property.PropertyType.IsValueType)
                    {
                        props.Add(filterName, (int)property.GetValue(obj));
                    }
                }
            }

            return props;
        }
    }
}
