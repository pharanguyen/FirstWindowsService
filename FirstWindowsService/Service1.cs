using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Data.SqlClient;
using System.Windows;
using System.Security.Cryptography;
//using FirstWindowsService.Models;
using System.Windows.Media;
using System.Drawing;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Xml.Linq;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace FirstWindowsService
{
    public partial class Service1 : ServiceBase
    {
        // tạo 1 biến Timer private
       // System.Timers.Timer timerVlog = new System.Timers.Timer();
        System.Timers.Timer timerVlog = new System.Timers.Timer();
        //DateTime _scheduleTime = DateTime.Today.AddDays(1).AddHours(8).AddMinutes(26);
        public Service1()
        {
            
            InitializeComponent();
            
            //Utilities.WriteLogError($"{_scheduleTime}");
           






             
        }


        protected override void OnStart(string[] args)
        {
            DateTime nowTime = DateTime.Now;
            DateTime scheduledTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 7, 0, 0, 0); //Specify your scheduled time HH,MM,SS [8am and 42 minutes]
            if (nowTime > scheduledTime)
            {
                scheduledTime = scheduledTime.AddDays(1);
            }

            double tickTime = (double)(scheduledTime - DateTime.Now).TotalMilliseconds;
            timerVlog = new System.Timers.Timer(tickTime);
            timerVlog.Elapsed += new ElapsedEventHandler(TimeInterval);
            //timerVlog.Interval = 120000;
            timerVlog.Enabled = true;


            Utilities.WriteLogError("Service started at " + scheduledTime);
          //  timerVlog.Elapsed += new ElapsedEventHandler(TimeInterval);
           




        }


        private void TimeInterval(object sender, ElapsedEventArgs args)
        {
            var a = GetLuuLuong();
            Utilities.WriteLogError(a);


            Thread.Sleep(10000);
            var b = PostLuuLuong(a);
            Utilities.WriteLogError(b);
            Thread.Sleep(10000);
            var m = Get3ThongSo();
            Utilities.WriteLogError(m);
            Thread.Sleep(10000);
            var n = Post3ThongSo(m);
            Utilities.WriteLogError(n);
        }







        protected override void OnStop()
        {
            // Ghi log lại khi Services đã được stop
            timerVlog.Enabled = true;
            //Utilities.WriteLogError("1st WindowsService has been stop");
        }
        public string GetLuuLuong()
        {
           
                double a = 0;
                string ConnectionString = @"Data Source=113.160.111.205;Initial Catalog=baocao_scada_db;User ID=ad;Password=cnhp123456";
                SqlConnection cnn = new SqlConnection(ConnectionString);
                cnn.Open();
                var day = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                string sql = "DECLARE @max datetime;" +
                "DECLARE @min datetime;" +
                "DECLARE @GTDAU NVARCHAR(200);" +
                "DECLARE @GTCUOI NVARCHAR(200);" +
                "SET @min = (select MIN(Thoi_Gian) from Nhat_Ky_Thang where Thoi_Gian between '" + day + "' and '" + day + " 23:59:59');" +

                "SET @max = (select MAX(Thoi_Gian) from Nhat_Ky_Thang where Thoi_Gian between '" + day + "' and '" + day + " 23:59:59');" +

                "SET @GTDAU = (select Sum(CAST([Gia_Tri] as FLOAT)) AS GTdau from Nhat_Ky_Thang  where Thoi_Gian = @min and(Id_Thongso = 13 or Id_Thongso = 14 or Id_Thongso = 15) and Id_Tram = 1 );" +
                "SET @GTCUOI = (select Sum(CAST([Gia_Tri] as FLOAT)) AS GTCUOI from Nhat_Ky_Thang  where Thoi_Gian = @max and(Id_Thongso = 13 or Id_Thongso = 14 or Id_Thongso = 15) and Id_Tram = 1 );" +
               "select cast(@GTCUOI as float)-cast(@GTDAU as float)";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;

                cmd.Connection = cnn;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    a = (double)reader[0];
                }
                var getluuluong = new
                {
                    MaTinh = "HP",
                    KyHieuCongTrinh = "NMNANDUONG",
                    KyHieuTram = "HM",
                    ThongSoDo = "LUULUONG",
                    ThoiGianGui = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    NoiDung = new List<object>() { new List<object>() { DateTime.Now.AddDays(-1).ToString("yyyyMMdd235959"), a, "m3/h", "00" } }
                    //NoiDung = new Array[] { DateTime.Now.AddDays(-1).ToString("yyyyMMdd235959"),a.ToString, "m3/h", "00" }



                };

                var c = JsonConvert.SerializeObject(getluuluong, Newtonsoft.Json.Formatting.Indented);
                return c;

            
        }
        public string Get3ThongSo()
        {
            double b = 0;
           

             
                string ConnectionString = @"Data Source=113.160.111.205;Initial Catalog=baocao_scada_db;User ID=ad;Password=cnhp123456";
                SqlConnection cnn = new SqlConnection(ConnectionString);
                cnn.Open();
                var day = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                string sql = "DECLARE @max datetime;" +
                              "SET @max = (select MAX(Thoi_Gian) from Nhat_Ky_Thang where  Thoi_Gian between '2022-12-26 00:00:00' and '2022-12-26 23:59:59' );" +
                              "select c.Ten,d.Gia_Tri,c.Don_Vi,d.Thoi_Gian from Dm_Tram a  join ThongSo_Tram b on a.Id= b.Id_Tram   join Dm_ThongSo c on b.Id_ThongSo = c.Id  join Nhat_Ky_Thang d on c.Id= d.Id_ThongSo\r\nwhere a.Id=1 and d.Id_Tram=1 and (c.Ten = N'ĐỘ ĐỤC 1'  or c.Ten=N'ĐỘ DẪN ĐIỆN 1'  or c.Ten='pH 1' ) and d.Thoi_Gian = @max";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;

                cmd.Connection = cnn;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                List<object> list = new List<object>();
                foreach (DataRow dr in dt.Rows)
                {
                    string ten = dr[0].ToString();
                    if (ten == "pH 1") ten = "pH";
                    else if (ten == "ĐỘ ĐỤC 1") ten = "Độ đục";
                    else ten = "Doandien";
                    //string donvi = dr[2].ToString();
                    //if (donvi == null) dr[2].ToString()

                    list.Add(new List<object>() { ten, double.Parse(dr[1].ToString()), string.IsNullOrWhiteSpace(dr[2].ToString()) ? "-" : dr[2], DateTime.Parse(dr[3].ToString()).ToString("yyyyMMddHHmmss"), "00" });

                }
                //DateTime.Now.AddDays(-1).ToString("yyyyMMdd235959") , "m3/h", "00"
                var get3thongso = new
                {
                    MaTinh = "HP",
                    KyHieuCongTrinh = "NMNANDUONG",
                    KyHieuTram = "QUANTRAC",
                    ThoiGianGui = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    NoiDung = list

                };
                var d = JsonConvert.SerializeObject(get3thongso, Newtonsoft.Json.Formatting.Indented);
                return d;

            
        }
        public string PostLuuLuong( string c)
        {
            
                var responseMess = "";
                string URL = "https://gstnn.monre.gov.vn:8443/input/TNN_REST_INPUT";
                string urlParameters = "?DATA=" + c;
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(URL);
                client.Timeout = TimeSpan.FromMinutes(5);
                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
                );
                client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue(
            "Basic", Convert.ToBase64String(
                System.Text.ASCIIEncoding.ASCII.GetBytes(
                   $"capnuochp:fSm7nTlZPG")));

                // List data response.
                HttpResponseMessage response = client.GetAsync(urlParameters).Result;
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body.
                    responseMess = response.Content.ToString();
                }
                else
                {
                    responseMess = string.Format("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                }
                client.Dispose();
                return response.StatusCode.ToString();
           


        }
        public string Post3ThongSo( string d)
        {
            
                var responseMess = "";
                string URL = "https://gstnn.monre.gov.vn:8443/input/TNN_REST_INPUT";
                string urlParameters = "?DATA=" + d;
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(URL);
                client.Timeout = TimeSpan.FromMinutes(5);
                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
                );
                client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue(
            "Basic", Convert.ToBase64String(
                System.Text.ASCIIEncoding.ASCII.GetBytes(
                   $"capnuochp:fSm7nTlZPG")));

                // List data response.
                HttpResponseMessage response = client.GetAsync(urlParameters).Result;
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body.
                    responseMess = response.Content.ToString();
                }
                else
                {
                    responseMess = string.Format("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                }
                client.Dispose();
                return response.StatusCode.ToString();
           

        }
    }

        



      }
        

    




