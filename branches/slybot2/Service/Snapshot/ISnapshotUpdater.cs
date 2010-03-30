using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Service.Snapshot
{
    [DebuggerDisplay("({MyAsk},{Ask}|{Price}|{Bid},{MyBid}")]
    public struct StakanItem : IEquatable<StakanItem>
    {
        //public StakanItem(DbDataReader dbDataReader)
        //{
        //    MyAsk = dbDataReader.GetInt32(1);
        //    Ask = dbDataReader.GetInt32(2);
        //    Price = dbDataReader.GetDouble(3);
        //    Bid = dbDataReader.GetInt32(4);
        //    MyBid = dbDataReader.GetInt32(5);
        //}

        public int MyAsk;
        public int Ask;
        public double Price;
        public int Bid;
        public int MyBid;

        public bool Equals(StakanItem other)
        {
            return MyAsk == other.MyAsk &&
                   Ask == other.Ask &&
                   Price == other.Price &&
                   Bid == other.Bid &&
                   MyBid == other.MyBid;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return Equals((StakanItem) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = MyAsk;
                result = (result*397) ^ Ask;
                result = (result*397) ^ Price.GetHashCode();
                result = (result*397) ^ Bid;
                result = (result*397) ^ MyBid;
                return result;
            }
        }
    }

    public struct SnapOrder : IEquatable<SnapOrder>
    {
        public int Code;
        public string Time;
        public string SecCode;
        public string Operation;
        public double Price;
        public int Quantity;
        public int Remaining;
        public double Volume;
        public string State;
        public string ClassCode;


        public bool Equals(SnapOrder other)
        {
            return Code == other.Code &&
                   Time == other.Time &&
                   SecCode == other.SecCode &&
                   Operation == other.Operation &&
                   Price == other.Price &&
                   Quantity == other.Quantity &&
                   Remaining == other.Remaining &&
                   Volume == other.Volume &&
                   State == other.State &&
                   ClassCode == other.ClassCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return Equals((SnapOrder) obj);
        }

        public override int GetHashCode()
        {
            return Code << 4 + Remaining;
        }
    }

    public struct PortfolioEntry : IEquatable<PortfolioEntry>
    {
        public string Instrument;
        public int Position;
        public double VarMargin;

        public bool Equals(PortfolioEntry other)
        {
            return Equals(other.Instrument, Instrument) &&
                   other.Position == Position &&
                   other.VarMargin == VarMargin;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return Equals((PortfolioEntry)obj);
        }



        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Instrument != null ? Instrument.GetHashCode() : 0);
                result = (result*397) ^ Position;
                result = (result*397) ^ VarMargin.GetHashCode();
                return result;
            }
        }
    }

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

