using City_Bus_Management_System.DataLayer;
using Data_Access_Layer.DataLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Layer.IServices
{
    public interface IDriverService
    {
        ResponseModel<object> UpdateTripStatus(string driverId, string Status);

        //ResponseModel<IssueReportingDTO> IssueReporting(IssueReportingDTO issueReporting);

    }
}
