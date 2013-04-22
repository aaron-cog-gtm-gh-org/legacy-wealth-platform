using System;
using System.Collections.Generic;

namespace PV.Core
{
    // RJM 2013-05: message model for the MT535 holdings statement.
    public class Mt535Statement
    {
        public string Function;           // whole 23G text, e.g. NEWM, RPLC, NEWM/DUPL
        public DateTime StatementDate;    // 98A::STAT
        public string SafekeepingAcct;    // 97A::SAFE raw, usually 7-digit padded
        public List<Mt535Line> Lines = new List<Mt535Line>();
        public string FileName;
        public int FileSeq;               // _NNN suffix of the archive file name
    }

    public class Mt535Line
    {
        public string Isin;
        public string MsgDescription;     // last line of the 35B block
        public decimal Price;             // 90A/90B market price
        public decimal Qty;               // 93B::AGGR amount
        public string QtyType;            // UNIT or FAMT - kept for reference
        public string Ccy;                // instrument ccy from 19A::HOLD
        public decimal MktVal;
        public decimal MktValGbp;
    }

    // DW 2014-03: MT54x settlement confirmation as we need it on screen.
    public class SettlementConfirm
    {
        public int MtType;
        public string Seme;
        public string Prev;
        public string Function;           // NEWM / CANC
        public DateTime TradeDate;
        public DateTime SettleDate;
        public string Isin;
        public decimal Qty;
        public string custAcct;           // 97A::SAFE raw from the confirmation
        public string AmountRaw;          // 19A::ESTT as sent, e.g. GBP4303,01
        public string Ccy;
        public string Direction;          // REC for 544/545, DEL for 546/547
        public string FilePath;
        public string Folder;             // mt54x day folder the file was found in
    }
}
