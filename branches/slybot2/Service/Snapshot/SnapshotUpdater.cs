using System;
using System.Collections.Generic;

namespace Service.Snapshot
{
    class SnapshotUpdater : ISnapshotUpdater
    {
        private class Snapshot : ISnapshot
        {
            public IEnumerable<StakanItem> GetStakan(string instrument)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<SnapOrder> GetSnapOrders()
            {
                throw new NotImplementedException();
            }

            public IEnumerable<PortfolioEntry> GetPorfolioItems()
            {
                throw new NotImplementedException();
            }
        }

        public bool Update()
        {
            throw new NotImplementedException();
        }

        public ISnapshot GetSnapshot()
        {
            throw new NotImplementedException();
        }
    }
}
