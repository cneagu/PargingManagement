using Microsoft.AspNetCore.Mvc;
using ParkingManagement.Contract.ParkingAllocation;
using System;
using System.Threading.Tasks;

namespace ParkingManagement.WebClient.Api.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ParkingAllocationController : ControllerBase
    {
        private readonly IParkingAllocationManager parkingAllocationManager;

        public ParkingAllocationController(IParkingAllocationManager parkingAllocationManager)
        {
            this.parkingAllocationManager = parkingAllocationManager;
        }

        [HttpGet]
        public async Task<Models.ParkingAllocation.ParkingRequestStatus> ParkingRequest(Guid requestID, Models.ParkingAllocation.PreferredType type)
        {
            Models.ParkingAllocation.ParkingRequestStatus status = await Task.Run(() =>
            {
                return (Models.ParkingAllocation.ParkingRequestStatus)parkingAllocationManager.ResolveParkingRequest(requestID, (PreferredType)type);
            });

            return status;
        }
    }
}
