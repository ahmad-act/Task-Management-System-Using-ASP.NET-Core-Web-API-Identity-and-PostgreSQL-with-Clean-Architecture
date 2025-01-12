using AutoMapper;

namespace TaskManagement.Application.Utilities.ExtensionMethods
{
    public static class AutoMapperExtension
    {
        public static void UpdateEntity<TSource, TDestination>(
            this TSource source, TDestination destination)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TSource, TDestination>()
                   .ForAllMembers(opt =>
                   {
                       opt.Condition((src, dest, srcMember) =>
                           srcMember != null && !IsDefaultValue(srcMember));
                   });
            });

            var mapper = config.CreateMapper();
            mapper.Map(source, destination);
        }

        private static bool IsDefaultValue(object value)
        {
            if (value == null) return true;

            var type = value.GetType();
            return value.Equals(type.IsValueType ? Activator.CreateInstance(type) : null);
        }
    }
}
