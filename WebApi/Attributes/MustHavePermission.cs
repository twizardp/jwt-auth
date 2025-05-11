using Common.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Attributes
{
    public class MustHavePermission : AuthorizeAttribute
    {
        public MustHavePermission(string feature, string action)
            => Policy = AppPermission.NameFor(feature, action);
    }
}
