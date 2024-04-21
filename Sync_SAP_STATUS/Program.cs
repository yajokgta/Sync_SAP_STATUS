using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using static Sync_SAP_STATUS.Service;

namespace Sync_SAP_STATUS
{
    internal class Program
    {
        public static string _Con = ConfigurationSettings.AppSettings["ConnectionString"];
        public static string _DocumentCode = ConfigurationSettings.AppSettings["DocumentCode"];
        static void Main(string[] args)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                var db = new SingDataContext(_Con);

                var templateId = db.MSTTemplates.FirstOrDefault(x => x.DocumentCode == _DocumentCode)?.TemplateId;
                var memoId = ConfigurationSettings.AppSettings["SrcMemoId"];
                var memos = new List<TRNMemo>();
                if (!string.IsNullOrEmpty(memoId))
                {
                    memos = db.TRNMemos.Where(x => x.MemoId.ToString() == memoId).ToList();
                }
                else
                {
                    memos = db.TRNMemos.Where(x => x.TemplateId == templateId && x.StatusName == "Completed").ToList();
                }

                foreach (var memo in memos)
                {
                    Console.WriteLine($"MEMO ID : {memo.MemoId}");
                    var sapRespone = JsonConvert.DeserializeObject<DataModel>(CallAPI(memo.DocumentNo));
                    if (sapRespone != null && sapRespone.E_DATA.Any())
                    {
                        Console.WriteLine($"SAP COUNT : {sapRespone.E_DATA.Count()}");

                        var cols = GetColumnInTable(memo.MAdvancveForm, "Purchase Requisition Table");

                        var selectCol = cols
                            .Select((x, i) => new { x.label, Index = i })
                            .Where(w => w.label.Contains("PO Number") || w.label.Contains("PO Item") || w.label.Contains("PO Status SAP")).ToList();

                        var dataTableString = getValueAdvanceForm(memo.MAdvancveForm, "Purchase Requisition Table").Replace(Environment.NewLine, "");

                        var dataTable = JsonConvert.DeserializeObject<RootObject>(dataTableString);
                        if (dataTable != null && dataTable.row.Any())
                        {
                            var rows = dataTable.row;

                            foreach (var data in sapRespone.E_DATA)
                            {
                                var rowSelector = rows.Select((s, i) => new { index = i, s }).Where(x => x.s.Any(a => a.value == data.PO_NUMBER) && x.s.Any(a => a.value == data.PO_ITEM)).FirstOrDefault();

                                if (rowSelector != null)
                                {
                                    Console.WriteLine($"RowSelector PO_NUMBER : {data.PO_NUMBER} PO_ITEM : {data.PO_ITEM}");
                                    rowSelector.s[selectCol.FirstOrDefault(x => x.label == "PO Status SAP").Index].value = data.PO_STATUS;
                                    dataTable.row[rowSelector.index] = rowSelector.s;
                                    Console.WriteLine($"REPLACE DATA : PROCESS");
                                    Console.WriteLine($"SAVE DATA!");
                                }
                            }

                            memo.MAdvancveForm = ReplaceDataProcess(memo.MAdvancveForm, JObject.Parse(JsonConvert.SerializeObject(dataTable)), "Purchase Requisition Table");

                            db.SubmitChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Thread.Sleep(10000);
            }
        }
    }
}
