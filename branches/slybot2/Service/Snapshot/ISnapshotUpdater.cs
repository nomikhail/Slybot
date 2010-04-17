using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Service.Snapshot
{
    internal interface ISnapshot
    {
        IEnumerable<StakanItem> GetStakan(string instrument);
        IEnumerable<SnapOrder> GetSnapOrders();
        IEnumerable<PortfolioEntry> GetPorfolioItems();
    }

    interface ISnapshotUpdater
    {
        bool Update();
        ISnapshot GetSnapshot();
    }
}

