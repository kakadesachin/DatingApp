using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        //we need to inject our configuration to start utilizing the JWT logic.
        public TokenService(IConfiguration config)
        {
            /*
            symmetric security key uses just one key to verify the token as the name suggests
            there is also AsymmetricSecurityKey to verify at the server end and the other client end
            as well if required.
            What we are doing is storing a key at the server and use it to verify the encryption.
            thats why we are creating this service and pulling the key through Configfile

            Note: the construction of the SymmetricSecKey requires the key value in the byte array
            hence Encoding it.
            */
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
        public string CreateToken(AppUser appUser)
        {
            /*
                JWT consists of 3 parts,
                1. The Token type and the algorithm used to create that token.
                2. Claims, to allow only whats necessary(like adding authorization)
                3. Signature, the key stored in the server can verify if the signature matches
                    if the token is modified the signature string changes making the token unverifiable.
            */

            //the below logic is to create claims to store within our JWT
            var claims = new List<Claim>
            {
                //for now we are just adding one claim, later part of the tutorial we will learn 
                //what other kinds of claims would be necessary to build an application
                new Claim(JwtRegisteredClaimNames.NameId,appUser.UserName)
            };

            // below logic is to create credentials for the token, like the signature
            // as you can see below the ctor needs the key and the algorithm to create a complex signature
            // that can be decoded only with the help of the same algorithm and the key.
            var creds = new SigningCredentials(_key,SecurityAlgorithms.HmacSha512Signature);

            //below logic is used to create the description of the token.
            //its like creating the content/body of the token entirely

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
    }
}