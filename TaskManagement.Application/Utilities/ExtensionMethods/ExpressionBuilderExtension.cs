using System.Linq.Expressions;

namespace TaskManagement.Application.Utilities.ExtensionMethods
{
    public static class ExpressionBuilderExtension
    {
        public static Expression<Func<T, TRelated>> BuildNavigationExpression<T, TRelated>(this string propertyName)
        {
            // Create a parameter for the entity (e.g., entity => ...)
            var parameter = Expression.Parameter(typeof(T), "entity");

            // Access the property dynamically (entity.PropertyName)
            var property = Expression.Property(parameter, propertyName);

            // Build the lambda (entity => entity.PropertyName)
            return Expression.Lambda<Func<T, TRelated>>(property, parameter);
        }

        public static string GetPropertyName<TEntity, TProperty>(this Expression<Func<TEntity, TProperty>> expression)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }
            else if (expression.Body is UnaryExpression unaryExpression &&
                     unaryExpression.Operand is MemberExpression memberOperand)
            {
                return memberOperand.Member.Name;
            }

            throw new ArgumentException("Invalid expression format. Could not extract property name.");
        }
    }
}
