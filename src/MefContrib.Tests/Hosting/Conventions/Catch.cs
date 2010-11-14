namespace MefContrib.Hosting.Conventions.Tests
{
    using System;

    public class Catch
    {
        public static Exception Exception(Action context)
        {
            try
            {
                context();
            }
            catch (Exception thrownException)
            {
                return thrownException;
            }

            return null;
        }
    }
}