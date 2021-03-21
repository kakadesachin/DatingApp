using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountsController : BaseAPIController
    {
        private readonly DataContext _context;
        // In order to handle JWTs we need to use Token Service,
        // Hence we are injecting the ITokenService that we created to manage JWTs
        private readonly ITokenService _tokenService;
        public AccountsController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }
        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {

            if (UserExists(registerDto.Username).Result)
            {
                return BadRequest("User name already taken");
            }

            /*
                We can encrypt the password using HMACSHA512 to store passwords in byte form..
                if at all our database that stores passwords is compromised hacker will not 
                understand what the password is cannot understand.
                once encrypted in order to decrypt the sameway backward we might need to know the key for it.
                therefore we will save the key that was generated to encrypt it
                later when we want to decrypt the password we can utilize and compare the passwords for authentication
                also note that hmac implements IDisposable interface.. therefore using keyword is mandatory to auto dispose
                the object that was instantiated
            */
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = registerDto.Username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        private async Task<bool> UserExists(string userName)
        {
            try
            {
                //note AnySync method is available in Microsoft.EntityFrameworkCore
                return await _context.Users.AnyAsync(user => user.UserName.ToLower() == userName.ToLower());
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            try
            {
                var user = await _context.Users
                                .SingleOrDefaultAsync(user => user.UserName.ToLower() == loginDto.Username.ToLower());
                if (user == null)
                {
                    return Unauthorized("Invalid User Name");
                }
                using var hmac = new HMACSHA512(user.PasswordSalt);
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != user.PasswordHash[i])
                    {
                        return Unauthorized("Invalid Password");
                    }
                }
                return new UserDto{
                    Username=user.UserName,
                    Token=_tokenService.CreateToken(user)
                };

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}