//using System.Data;
//using System.Linq;
//using IdentityServer4.Validation;
//using System.Threading.Tasks;
//using Dapper;
//using IdentityModel;
//using IdentityServer.Models;
//using Microsoft.Data.SqlClient;

//namespace IdentityServer.Configurations
//{
//    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
//    {
//        private readonly string _dbConnectionString = "";

//        /// <summary>
//        /// Validate users
//        /// </summary>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
//        {
//            using (IDbConnection db = new SqlConnection(_dbConnectionString))
//            {
//                var user = db.Query<User>("select * from Accounts where UserName=@UserName and Password=@Passw",
//                    new { UserName = context.UserName, Pass = context.Password }).SingleOrDefault<User>();

//                if (user == null)
//                {
//                    context.Result = new GrantValidationResult(OidcConstants.TokenErrors.InvalidRequest,
//                        "User name or password is incorrect");
//                    return Task.FromResult(0);
//                }

//                context.Result = new GrantValidationResult(user.Id.ToString(), "password");
//                return Task.FromResult(0);
//            }
//        }
//    }
//}
