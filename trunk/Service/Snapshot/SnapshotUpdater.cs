using System;
using System.Collections.Generic;

namespace Service.Snapshot
{
    class SnapshotUpdater : ISnapshotUpdater
    {
        private class Snapshot : ISnapshot
        {
            public Dictionary<string, List<StakanItem>> Stakans = null;
            public List<SnapOrder> Orders = null;
            public List<PortfolioEntry> Portfolio = null;

            public IEnumerable<StakanItem> GetStakan(string instrument)
            {
                yield break;
            }

            public IEnumerable<SnapOrder> GetSnapOrders()
            {
                yield break;
            }

            public IEnumerable<PortfolioEntry> GetPorfolioItems()
            {
                yield break;
            }
        }

        public bool Update()
        {
            return false;
        }

        public ISnapshot GetSnapshot()
        {
            return new Snapshot();
        }
    }
}
