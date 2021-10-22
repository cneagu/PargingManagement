namespace ParkingManagement.Security.Resource.Employee.Contract
{
    public interface IEmployeeResource
    {
        Employee SelectByEmail(string email);
        void Insert(Employee employee);
        int CountByEmail(string email);
    }
}
