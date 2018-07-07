using AngularDemo1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace AngularDemo1.Controllers.Api
{
    public class AuthController : ApiController
    {

        [Authorize]
        [RoutePrefix("api/Auth")]
        public class UserController : ApiController
        {
 
    // POST api/User/Login
    [HttpPost]
            [AllowAnonymous]
            [Route("Login")]
            public async Task<HttpResponseMessage> LoginUser(LoginUserBindingModel model)
            {
                // Invoke the "token" OWIN service to perform the login: /api/token
                // Ugly hack: I use a server-side HTTP POST because I cannot directly invoke the service (it is deeply hidden in the OAuthAuthorizationServerHandler class)
                var request = HttpContext.Current.Request;
                var tokenServiceUrl = request.Url.GetLeftPart(UriPartial.Authority) + request.ApplicationPath + "/Token";
                using (var client = new HttpClient())
                {
                    var requestParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", model.Username),
                new KeyValuePair<string, string>("password", model.Password)
            };
                    var requestParamsFormUrlEncoded = new FormUrlEncodedContent(requestParams);
                    var tokenServiceResponse = await client.PostAsync(tokenServiceUrl, requestParamsFormUrlEncoded);
                    var responseString = await tokenServiceResponse.Content.ReadAsStringAsync();
                    var responseCode = tokenServiceResponse.StatusCode;
                    var responseMsg = new HttpResponseMessage(responseCode)
                    {
                        Content = new StringContent(responseString, Encoding.UTF8, "application/json")
                    };
                    return responseMsg;
                }
            }

        }

       
    }
}
