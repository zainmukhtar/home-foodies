using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System;
using System.Security.Claims;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;

namespace HomeFoodies.Models
{
    public class ApplicationUser : IUser<string>
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string SupplierId { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one 
            // defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity =
                await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim(ClaimTypes.Sid, this.SupplierId)); 
            return userIdentity;
        }
    }

    public class UserStore : IUserStore<ApplicationUser>
    {
        //private CustomDbContext database;

        public UserStore()
        {
            //this.database = new CustomDbContext();
        }

        public void Dispose()
        {
            // this.database.Dispose();
        }

        public Task CreateAsync(ApplicationUser user)
        {
            // TODO 
            throw new NotImplementedException();
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            // TODO 
            throw new NotImplementedException();
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            // TODO 
            throw new NotImplementedException();
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            ApplicationUser usr = new ApplicationUser();

            usr.Id = userId;

            HomeFoodiesEntities _entity = new HomeFoodiesEntities();
            ObjectResult<LoginUserGetLogin> result = _entity.SP_LoginUserGetLogin(userId);
            IEnumerator<LoginUserGetLogin> currentUser = result.GetEnumerator();

            if (currentUser != null)
            {
                currentUser.MoveNext();
                if (currentUser.Current != null)
                {
                    if (currentUser.Current.UserID > 0)
                    {
                        usr.UserName = currentUser.Current.FullName;
                        usr.Id = currentUser.Current.UserID.ToString();
                        usr.SupplierId = currentUser.Current.SupplierID.ToString();
                    }
                }
            } 
            return await Task.FromResult<ApplicationUser>(usr);
            // ApplicationUser user = await this.database.ApplicationUsers.Where(c => c.UserId == userId).FirstOrDefaultAsync();
            //  return user;
        }

        public async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            ApplicationUser usr = new ApplicationUser();
            
            usr.Id = "123";
            usr.UserName = "Masroor";


            return await Task.FromResult<ApplicationUser>(usr);

            //ApplicationUser user = await this.database.ApplicationUsers.Where(c => c.UserName == userName).FirstOrDefaultAsync();
            //return user;
        }
    }
}