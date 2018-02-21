using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atomac.EFDataLayer
{

    public enum PStatus
    {
        Offline = 1,
        Active = 2,
        InGame = 3
    }

    public enum Title
    {
        Novice = 0,
        Amateur = 1000,
        ClassD = 1200,
        ClassC = 1400,
        ClassB = 1600,
        ClassA = 1800,
        Expert = 2000,
        Master = 2200,
        SeniorMaster = 2400,
        GrandMaster = 2600,
    }

    public enum TStatus
    {
        Offline = 1, // bar jedan od igraca igraca je offline
        Online = 2, // oba igraca su aktivna
        Busy = 3, // makar 1 igrac je InGame, a drugi je aktivan
        Active = 4 //oba igraca su aktivna + oba igraca su selektovala da igraju u njemu
    }

    public enum Board
    {
        T1 = 1,
        T2 = 2
    }
    
    public enum GStatus  // game status
    {
        Created = 1,
        Playing = 2,
        Finished = 3
    }

    public enum GTStatus  // game team status
    {
        Prepare = 1, // admin selektuje zeljena pravila
        Ready = 2, // admin je selektovao pravila
        Winner = 3,
        Losser = 4,
        Draw = 5
    }

    public enum AStatus
    {
        NeedToBuy = 1,
        NotActive = 2,
        Active = 3
    }

}
