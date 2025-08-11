using System.Linq;
using RestaurantPOS.Data;
using RestaurantPOS.Models;

public interface IAuthService
{
    UserModel? CurrentUser { get; }
    bool IsAuthenticated => CurrentUser != null;
    bool LoginByPin(string pin);
    void Logout();
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    public UserModel? CurrentUser { get; private set; }=null!;

    public AuthService(AppDbContext context)
    {
        _context = context;
        _context.Database.EnsureCreated();
    }

   

    public bool LoginByPin(string pin)
    {
        var user = _context.Users.FirstOrDefault(u => u.Pin == pin);
        if (user != null)
        {
            CurrentUser = user;
            return true;
        }
        return false;
    }


    public void Logout()
    {
        CurrentUser = null;
    }
}
