using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atomac.ViewModels
{
    public class ShopModel
    {
        public int Id;
        public string Style;
        public int Prize;
        public AStatus Status;
    }

    public enum AStatus
    {
        NeedToBuy = 1,
        NotActive = 2,
        Active = 3
    }

    public class ListType
    {
        public string name;
        public List<ShopModel> shopModels;

        public ListType(string typeName, List<ShopModel> list)
        {
            name = typeName;
            shopModels = list;
        }

        public ListType()
        {
            shopModels = new List<ShopModel>();
        }
    }

    public class ReturnAfterActivation
    {
        public string activated;
        public string deactivated;
    }
}