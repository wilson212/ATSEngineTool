namespace System
{
    public static class Epoch
    {
        /// <summary>
        /// The value for the Unix epoch (e.g. January 1, 1970 at midnight, in UTC).
        /// </summary>
        private static readonly DateTimeOffset UnixEpoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        /// <summary>
        /// Returns the current Epoch timestamp
        /// </summary>
        public static long Now => DateTimeOffset.Now.ToUnixTimeSeconds();

        /// <summary>
        /// Converts the supplied Epoch timestamp to a <see cref="DateTime"/>
        /// </summary>
        /// <param name="secondsSinceEpoch"></param>
        /// <returns></returns>
        public static DateTimeOffset FromEpoch(long secondsSinceEpoch)
        {
            return UnixEpoch.AddSeconds(secondsSinceEpoch);
        }

        /// <summary>
        /// Converts the supplied <see cref="DateTime"/> to an Epoch timestamp.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToEpoch(DateTime dateTime)
        {
            var dto = (DateTimeOffset)dateTime.ToUniversalTime();
            return dto.ToUnixTimeSeconds();
        }
    }
}
