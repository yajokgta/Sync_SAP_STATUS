using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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
                var db = new SingDataContext(_Con);

                var templateId = db.MSTTemplates.FirstOrDefault(x => x.DocumentCode == _DocumentCode)?.TemplateId;

                var memos = db.TRNMemos.Where(x => x.TemplateId == templateId && x.StatusName == "Completed").ToList();

                foreach (var memo in memos)
                {
                    Console.WriteLine($"MEMO ID : {memo.MemoId}");
                    var sapRespone = JsonConvert.DeserializeObject<DataModel>(CallAPI(memo.DocumentNo));
                    if (sapRespone != null && sapRespone.E_DATA.Any())
                    {
                        var cols = GetColumnInTable(memo.MAdvancveForm, "Purchase Requisition Table");

                        var selectCol = cols
                            .Select((x, i) => new { x.label, Index = i })
                            .Where(w => w.label.Contains("PO Number") || w.label.Contains("PO Item") || w.label.Contains("PO Status SAP")).ToList();

                        var dataTableString = getValueAdvanceForm(memo.MAdvancveForm, "Purchase Requisition Table").Replace(Environment.NewLine, "");

                        var dataTable = JsonConvert.DeserializeObject<RootObject>(dataTableString);
                        if (dataTable != null && dataTable.row.Any())
                        {
                            var rows = dataTable.row;

                            //dataTable.row[0][49].value = "TEST";
                            foreach (var data in sapRespone.E_DATA)
                            {
                                var rowSelector = rows.Select((s, i) => new { index = i, s }).Where(x => x.s.Any(a => a.value == data.PO_NUMBER && a.value == data.PO_ITEM)).FirstOrDefault();

                                if (rowSelector != null)
                                {
                                    rowSelector.s[selectCol.FirstOrDefault(x => x.label == "PO Status SAP").Index].value = data.PO_STATUS;
                                    dataTable.row[rowSelector.index] = rowSelector.s;
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
