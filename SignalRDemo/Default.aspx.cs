using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SignalRDemo
{
    using RestSharp;
    using RestSharp.Deserializers;

    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            JsonDeserializer deserial = new JsonDeserializer();

            var json = "{ \"has_title\": true, \"title\": \"GoodLuck\", \"entries\": [ [ \"/getting " +
                        " started. pdf\", { \"thumb_exists\": false, \"path\": \"/Getting " +
                        " Started. pdf\", \"client_mtime\": \"Wed, 08 Jan 2014 18: 00: 54" + " +0000\", \"bytes\": 249159} ] ," +
                        " [ \"/task.jpg\", {\"thumb_exists\": true, \"path\": \"/Ta" + " sk.jpg\", \"client_mtime\": \"Tue, 14 Jan 2014 05: 53: 570000\", \"bytes\": 207696} ] ] }";

            var response = new RestResponse { Content = json };

            var x = deserial.Deserialize<SampleTask>(response);
            Console.WriteLine(x);
        }
    }

    /// <summary>
    /// The sample task.
    /// </summary>
    public class SampleTask
    {
        public bool HasTitle { get; set; }
        public string Title { get; set; }
        public List<List<object>> Entries { get; set; }
    }
}