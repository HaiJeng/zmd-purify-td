public class Assert
{
    /**
     * Assert that a condition is true, otherwise throw an exception with the given message.
     * @param condition The condition to assert.
     * @param message The message to throw if the condition is false.
     */
    public static void IsTrue(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    /**
     * Assert that a condition is true, otherwise throw an exception with the given message.
     * @param condition The condition to assert.
     * @param message The exception to throw if the condition is false.
     */
    public static void IsTrue(bool condition, Exception exception)
    {
        if (!condition) throw exception;
    }

    public static void IsNotNull(object? obj, string message)
    {
        if (obj == null) throw new ArgumentNullException(message);
    }
    public static void IsNotNull(object? obj, Exception exception)
    {
        if (obj == null) throw exception;
    }
    public static void IsNotBlank(string str, string message)
    {
        if (string.IsNullOrWhiteSpace(str)) throw new ArgumentNullException(message);
    }
    public static void IsNotBlank(string str, Exception exception)
    {
        if (string.IsNullOrWhiteSpace(str)) throw exception;
    }
}