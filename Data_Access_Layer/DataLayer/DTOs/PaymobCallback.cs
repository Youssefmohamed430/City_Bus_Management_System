using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.DataLayer.DTOs
{
    public class PaymobCallback
    {
        public string type { get; set; }
        public PaymobObj obj { get; set; }
    }

    public class PaymobObj
    {
        public string amount_cents { get; set; }
        public string created_at { get; set; }
        public string currency { get; set; }
        public bool error_occurred { get; set; }
        public bool has_parent_transaction { get; set; }
        public long id { get; set; }
        public int integration_id { get; set; }
        public bool is_3d_secure { get; set; }
        public bool is_auth { get; set; }
        public bool is_capture { get; set; }
        public bool is_refunded { get; set; }
        public bool is_standalone_payment { get; set; }
        public bool is_voided { get; set; }
        public int order_id { get; set; }
        public int owner { get; set; }
        public bool pending { get; set; }
        public PaymobSourceData source_data { get; set; }
        public bool success { get; set; }
    }

    public class PaymobSourceData
    {
        public string pan { get; set; }
        public string sub_type { get; set; }
        public string type { get; set; }
    }


}
