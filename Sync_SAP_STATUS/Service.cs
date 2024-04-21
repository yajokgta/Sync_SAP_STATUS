using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync_SAP_STATUS
{
    internal class Service
    {
        public class DataModel
        {
            public EData[] E_DATA { get; set; }
            public object[] E_HISDATA { get; set; }
            public EReturn[] E_RETURN { get; set; }
        }

        public class EData
        {
            public string WOLF_NO { get; set; }
            public string WOLF_ITEM { get; set; }
            public string DOC_TYPE { get; set; }
            public string PURCH_ORG { get; set; }
            public string PUR_GROUP { get; set; }
            public string COMP_CODE { get; set; }
            public string CURRENCY { get; set; }
            public string VENDOR { get; set; }
            public double EXCH_RATE { get; set; }
            public DateTime DOC_DATE { get; set; }
            public DateTime VPER_START { get; set; }
            public DateTime VPER_END { get; set; }
            public string REF_1 { get; set; }
            public string OUR_REF { get; set; }
            public string COLLECT_NO { get; set; }
            public string PO_NUMBER { get; set; }
            public string PO_ITEM { get; set; }
            public string DEL_FLAG { get; set; }
            public string SHORT_TEXT { get; set; }
            public string MATERIAL { get; set; }
            public string PLANT { get; set; }
            public string MATL_GROUP { get; set; }
            public double QUANTITY { get; set; }
            public string PO_UNIT { get; set; }
            public double NET_PRICE { get; set; }
            public double AMOUNT { get; set; }
            public string TAX_CODE { get; set; }
            public string NO_MORE_GR { get; set; }
            public string ACCTASSCAT { get; set; }
            public string GL_ACCOUNT { get; set; }
            public string BUS_AREA { get; set; }
            public string COSTCENTER { get; set; }
            public string ASSET_NO { get; set; }
            public string SUB_NUMBER { get; set; }
            public string ORDERID { get; set; }
            public string GR_RCPT { get; set; }
            public string UNLOAD_PT { get; set; }
            public string WBS_ELEMENT { get; set; }
            public string OPEN_RELEASE { get; set; }
            public string PO_STATUS { get; set; }
            public string OPEN_ITEM { get; set; }
            public double QTY_GR { get; set; }
            public double QTY_OPEN { get; set; }
            public string RPA_STATUS { get; set; }
            public string RPA_CHANGE { get; set; }
            public string HEADER_TEXT { get; set; }
            public string INSTALLMENT_TEXT { get; set; }
            public string DELIVERY_TEXT { get; set; }
            public string HEADER_NOTE_TEXT { get; set; }
            public string PROC_METHOD_TEXT { get; set; }
            public string SAVING_TEXT { get; set; }
            public string REMARK_TEXT { get; set; }
            public string ITEM_TEXT { get; set; }
        }

        public class EReturn
        {
            public string TYPE { get; set; }
            public string ID { get; set; }
            public string NUMBER { get; set; }
            public string MESSAGE { get; set; }
            public string LOG_NO { get; set; }
            public string LOG_MSG_NO { get; set; }
            public string MESSAGE_V1 { get; set; }
            public string MESSAGE_V2 { get; set; }
            public string MESSAGE_V3 { get; set; }
            public string MESSAGE_V4 { get; set; }
            public string PARAMETER { get; set; }
            public int ROW { get; set; }
            public string FIELD { get; set; }
            public string SYSTEM { get; set; }
        }

        public class PurchaseModel
        {
            public Row[] Row { get; set; }
        }

        public class Row
        {
            public Value[] Value { get; set; }
        }

        public class Value
        {
            public string value { get; set; }
        }
    }
}
