using Microsoft.AspNetCore.Routing;
using System.Text.RegularExpressions;

namespace TaskManagement.Presenter.RouteTransformer
{
    public class SpinCaseTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            if (value == null) return null;

            // Convert the value to a snake_case format as an example
            // (e.g., "MyRoute" becomes "my_route")
            string vvv = Regex.Replace(value.ToString()!, "([a-z])([A-Z])", "$1-$2").ToLower();
            return vvv;
        }
    }
}
