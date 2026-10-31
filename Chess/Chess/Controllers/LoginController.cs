using Chess.Domain.Entities;
using Chess.Domain.Helpers;
using Chess.Domain.Responces;
using Chess.Models;
using Chess.Service.Implementations;
using Chess.Service.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Chess.Controllers
{
    public class LoginController : Controller
    {
        IUserService userServise;
        public LoginController(IUserService userServise) { this.userServise = userServise; }

        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }


        public async Task<IActionResult> EnterUser(UserModel model)
        {
            if (model != null)
            {
                IBaseResponce<User> response = await userServise.GetByEmailAndPassAsync(model.Email,HelperHashCode.HashString(model.Password));
                switch (response.Code)
                {
                    case Domain.Enums.Enums.ResponceCodes.Ok:
                        HttpContext.Session.SetString("UserId", Convert.ToString(response.Data.Id));

                        ClaimsIdentity identity = userServise.Authentication(response.Data);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(identity));

                        return Redirect("/Home/Index");
                    case Domain.Enums.Enums.ResponceCodes.BoolError:
                        break;
                    case Domain.Enums.Enums.ResponceCodes.UserExist:
                        break;
                    case Domain.Enums.Enums.ResponceCodes.UnCatchedError:
                        break;
                    case Domain.Enums.Enums.ResponceCodes.InvalidParameter:
                        break;
                    default:
                        break;
                }
                return Register();
            }
            else
            {
                return Register();
            }
        }



        public async Task<IActionResult> CreateNewUser(UserModel model) 
        {
            if (model!=null)
            {
                User user = new()
                {
                    Email = model.Email,
                    Login = model.Login,
                    PasswordHash = HelperHashCode.HashString(model.Password),
                    DisplayName = model.DisplayName,
                    IsActive = true,
                    EloRating = 100,
                    CreatedAt = DateTime.Now,
                };
                IBaseResponce<bool> response = await userServise.CreateAsync(user);
                switch (response.Code)
                {
                    case Domain.Enums.Enums.ResponceCodes.Ok:
                        var usr = await userServise.GetByEmailAsync(user.Email);
                        HttpContext.Session.SetString("UserId", Convert.ToString(usr.Data.Id));

                        ClaimsIdentity identity = userServise.Authentication(usr.Data);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(identity));

                        return Redirect("/Home/Index");
                    case Domain.Enums.Enums.ResponceCodes.BoolError:
                        break;
                    case Domain.Enums.Enums.ResponceCodes.UserExist:
                        break;
                    case Domain.Enums.Enums.ResponceCodes.UnCatchedError:
                        break;
                    case Domain.Enums.Enums.ResponceCodes.InvalidParameter:
                        break;
                    default:
                        break;
                }
                return Register();
            }
            else
            {
                return Register();
            }
        }
    }
}
