using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using SocialNetwork.Data.Repositories;

namespace SocialNetwork.OAuth
{
    public class SocialNetworkUserService : UserServiceBase
    {
        private readonly IUserRepository userRepository;

        public SocialNetworkUserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public override async Task AuthenticateLocalAsync(LocalAuthenticationContext context)
        {
            var user = await userRepository.GetAsync(context.UserName,
                HashHelper.Sha512(context.Password + context.UserName));

            if (user == null)
            {
                context.AuthenticateResult 
                    = new AuthenticateResult("Incorrect credentials");
                return;
            }

            context.AuthenticateResult = 
                new AuthenticateResult("/terms", context.UserName, context.UserName);
        }
    }
}