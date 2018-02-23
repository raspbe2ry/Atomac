using Atomac.EFDataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atomac.Web.ViewModels
{
    public class ShopModel
    {
        public int Id;
        public string Style;
        public int Prize;
        public AStatus Status;
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

    public class BoughtArtifact
    {
        public int artifactId;
        public int prize;
    }
    
}