using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TaskManagement.Presenter.Filters
{
    public class InputValidationAttribute : ActionFilterAttribute
    {
        public string? ErrorMessage { get; set; } = "Invalid input data"; // Custom error message
        public bool LogValidation { get; set; } = true; // Option to enable or disable logging
        public int StatusCode { get; set; } = 400; // Customizable status code

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Check if ModelState is valid
            if (!context.ModelState.IsValid)
            {
                // Customize the response based on options
                var response = new
                {
                    StatusCode = StatusCode,
                    Message = ErrorMessage,
                    Errors = context.ModelState
                };

                // Set the custom response as the result
                context.Result = new ObjectResult(response)
                {
                    StatusCode = StatusCode
                };
            }

            // Conditionally log the action
            if (LogValidation)
            {
                Console.WriteLine("OnActionExecuting: Model state validation performed.");
            }
        }
    }
}
