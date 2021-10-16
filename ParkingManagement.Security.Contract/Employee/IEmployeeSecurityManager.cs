namespace ParkingManagement.Security.Contract.Employee
{
    public interface IEmployeeSecurityManager
    {
        LoginReturn Login(string email, string password);
        RegisterReturn Register(RegisterEmployee registerEmployee);
    }
}
