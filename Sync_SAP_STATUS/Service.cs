using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sync_SAP_STATUS
{
    internal class Service
    {
        public class DataModel
        {
            public EData[] E_DATA { get; set; }
            public object[] E_HISDATA { get; set; }
        }

        public class EData
        {
            public string WOLF_NO { get; set; }
            public string WOLF_ITEM { get; set; }
            public string PO_NUMBER { get; set; }
            public string PO_ITEM { get; set; }
            public string PO_STATUS { get; set; }
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

        public static string _Json = @"{  ""row"": [    [      {        ""value"": ""7600 สำนักงานใหญ่ (S-HO)""      },      {        ""value"": ""7600""      },      {        ""value"": ""2024""      },      {        ""value"": ""7600614000 IT""      },      {        ""value"": ""7600614000""      },      {        ""value"": ""HOF Head Office""      },      {        ""value"": "" N/A""      },      {        ""value"": """"      },      {        ""value"": ""A.76.20.7600614000 IT""      },      {        ""value"": ""A.76.20.7600614000""      },      {        ""value"": ""วิธีตกลงราคา (Price Agreement)""      },      {        ""value"": null      },      {        ""value"": ""-""      },      {        ""value"": ""2.00""      },      {        ""value"": ""100.00""      },      {        ""value"": ""200.00""      },      {        ""value"": ""18 Apr 2024""      },      {        ""value"": ""THB""      },      {        ""value"": ""1.00000""      },      {        ""value"": ""200.00""      },      {        ""value"": ""V7 Purchase tax rate 7%""      },      {        ""value"": ""7.00""      },      {        ""value"": ""14.00""      },      {        ""value"": ""214.00""      },      {        ""value"": ""23988537""      },      {        ""value"": ""23988323""      },      {        ""value"": ""WITHIN""      },      {        ""value"": null      },      {        ""value"": ""1000025 บริษัท เอลเม็ค วิศวกรรม จำกัด""      },      {        ""value"": ""ITHW05 อุปกรณ์ต่อพ่วง""      },      {        ""value"": ""1000000014 ปลั๊กไฟ 3 ตา""      },      {        ""value"": ""EA each""      },      {        ""value"": null      },      {        ""value"": null      },      {        ""value"": null      },      {        ""value"": ""F Order""      },      {        ""value"": """"      },      {        ""value"": ""101 จัดซื้อกลาง""      },      {        ""value"": ""7600 จัดซื้อกลาง""      },      {        ""value"": null      },      {        ""value"": null      },      {        ""value"": null      },      {        ""value"": null      },      {        ""value"": null      },      {        ""value"": null      },      {        ""value"": null      },      {        ""value"": ""2024-04-20""      },      {        ""value"": ""4010000719""      },      {        ""value"": ""00010""      },      {        ""value"": null      }    ]  ]}
";

        public class Value
        {
            public string value { get; set; }
        }

        public class RootObject
        {
            public List<List<Value>> row { get; set; }
        }

        public class Column
        {
            public string label { get; set; }
        }
        public class I_DATA
        {
            public string WOLF_NO_FROM { get; set; }
            public string WOLF_NO_TO { get; set; }
            public string PO_NUMBER_FROM { get; set; }
            public string PO_NUMBER_TO { get; set; }
            public string PO_DATE_FROM { get; set; }
            public string PO_DATE_TO { get; set; }
            public string RPA_STATUS { get; set; }
            public bool? RPA_UPDATE_FLAG { get; set; }
        }

        public static string CallAPI(string requset)
        {
            string respones = null;
            try
            {
                HttpClient client = new HttpClient();
                client.Timeout = TimeSpan.FromMinutes(30);
                var body = new
                {
                    I_DATA = new I_DATA
                    {
                        WOLF_NO_FROM = requset,
                    }
                };

                var apiPath = ConfigurationSettings.AppSettings["APIPath"];
                var module = ConfigurationSettings.AppSettings["Module"];
                var sap_client = ConfigurationSettings.AppSettings["sap-client"];

                client.DefaultRequestHeaders.Add("Authorization", ConfigurationSettings.AppSettings["Token"]);
                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                var respone = client.PostAsync($"{apiPath}/e-expense/{module}?sap-client={sap_client}&format=json&sap-language=TH", content);
                respone.Wait();
                var result = respone.Result;
                var massage = result.Content.ReadAsStringAsync().Result;
                respones = massage;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return respones;
        }

        public static List<Column> GetColumnInTable(string advanceForm, string label)
        {
            //string setValue = "";
            JObject jsonAdvanceForm = JObject.Parse(advanceForm);
            if (jsonAdvanceForm.ContainsKey("items"))
            {
                JArray itemsArray = (JArray)jsonAdvanceForm["items"];
                foreach (JObject jItems in itemsArray)
                {
                    JArray jLayoutArray = (JArray)jItems["layout"];
                    foreach (JToken jLayout in jLayoutArray)
                    {
                        JObject jTemplate = (JObject)jLayout["template"];
                        var getLabel = (String)jTemplate["label"];
                        if (label == getLabel)
                        {
                            JObject attribute = (JObject)jTemplate["attribute"];
                            return JsonConvert.DeserializeObject<List<Column>>(attribute["column"].ToString());
                        }
                    }
                }
            }

            return new List<Column>();
        }

        public static string getValueAdvanceForm(string AdvanceForm, string label)
        {
            string setValue = "";
            JObject jsonAdvanceForm = JObject.Parse(AdvanceForm);
            if (jsonAdvanceForm.ContainsKey("items"))
            {
                JArray itemsArray = (JArray)jsonAdvanceForm["items"];
                foreach (JObject jItems in itemsArray)
                {
                    JArray jLayoutArray = (JArray)jItems["layout"];
                    foreach (JToken jLayout in jLayoutArray)
                    {
                        JObject jTemplate = (JObject)jLayout["template"];
                        var getLabel = (String)jTemplate["label"];
                        if (label == getLabel)
                        {
                            return jLayout["data"].ToString();
                            //if (jdata != null)
                            //{
                            //    if (jdata["value"] != null) setValue = jdata["value"].ToString();
                            //}
                            //break;
                        }
                    }
                }
            }

            return setValue;
        }

        public static string ReplaceDataProcess(string DestAdvanceForm, JObject Value, string label)
        {
            JObject jsonAdvanceForm = JObject.Parse(DestAdvanceForm);
            JArray itemsArray = (JArray)jsonAdvanceForm["items"];
            foreach (JObject jItems in itemsArray)
            {
                JArray jLayoutArray = (JArray)jItems["layout"];

                if (jLayoutArray.Count >= 1)
                {
                    JObject jTemplateL = (JObject)jLayoutArray[0]["template"];

                    if ((String)jTemplateL["label"] == label)
                    {
                        jLayoutArray[0]["data"] = Value;
                    }

                    if (jLayoutArray.Count > 1)
                    {
                        JObject jTemplateR = (JObject)jLayoutArray[1]["template"];

                        if ((String)jTemplateR["label"] == label)
                        {

                            jLayoutArray[1]["data"] = Value;
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(jsonAdvanceForm);
        }
    }
}
