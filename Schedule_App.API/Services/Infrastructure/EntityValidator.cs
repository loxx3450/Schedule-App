namespace Schedule_App.API.Services.Infrastructure
{
    public static class EntityValidator
    {
        /// <summary>
        /// Throws exception basing on income data in case of entity being null
        /// </summary>
        /// <typeparam name="T">Type of Entity</typeparam>
        /// <param name="entity">Entity to check</param>
        /// <param name="propertyName">Name of property, that was used by search</param>
        /// <param name="propertyValue">Value of property, that was used by search</param>
        /// <exception cref="KeyNotFoundException">The exception if entity is null</exception>
        public static void EnsureEntityExists<T>(T? entity, string propertyName, object propertyValue)
        {
            if (entity is not null)
                return;

            // Generating exception basing on income data
            var entityName = typeof(T).Name;

            var message = $"{entityName} with {propertyName} '{propertyValue}' does not exist";

            throw new KeyNotFoundException(message);
        }

        /// <summary>
        /// Throws exception basing on income data in case of entity being null
        /// </summary>
        /// <typeparam name="T">Type of Entity</typeparam>
        /// <param name="entity">Entity to check</param>
        /// <param name="propertyNames">Names of properties, that were used by search</param>
        /// <param name="propertyValues">Values of properties, that were used by search</param>
        /// <exception cref="KeyNotFoundException">The exception if entity is null</exception>
        public static void EnsureEntityExists<T>(T? entity, string[] propertyNames, object[] propertyValues)
        {
            if (entity is not null)
                return;

            // Generating exception basing on income data
            var entityName = typeof(T).Name;

            var propertyNamesJoined = $"[{string.Join(", ", propertyNames)}]";
            var propertyValuesJoined = $"[{string.Join(", ", propertyValues)}]";

            var message = $"{entityName} with {propertyNamesJoined} '{propertyValuesJoined}' does not exist";

            throw new KeyNotFoundException(message);
        }
    }
}
