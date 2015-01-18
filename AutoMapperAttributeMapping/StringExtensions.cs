namespace AutoMapperAttributeMapping
{
    public static class StringExtensions
    {
        /// <summary>
        /// Remove the last char of a string. This is usefull if we have trailing dot or space from loop concatenation.
        /// </summary>
        /// <param name="stringToManipulate"></param>
        /// <returns></returns>
        public static string RemoveLastChar(this string stringToManipulate)
        {
            if (stringToManipulate == null)
                return null;
            return stringToManipulate.Length > 0 ? stringToManipulate.Remove(stringToManipulate.Length - 1) : null;
        }
    }
}