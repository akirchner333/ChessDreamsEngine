namespace Engine
{
    public static class Rand
    {
        public static Random R { get; private set; }
        private const int _key = 1847123948;
        static Rand()
        {
            R = new Random(_key);
        }

        public static void Reset()
        {
            R = new Random(_key);
        }

        public static ulong RandULong()
        {
            return (ulong)((R.NextInt64() << 1) ^ R.Next());
        }

        public static ulong RandULong(Random r)
        {
            return (ulong)((r.NextInt64() << 1) ^ r.Next());
        }

        public static ulong[] RandULongArray(int count)
        {
            ulong[] array = new ulong[count];
            for (int i = 0; i < count; i++)
            {
                array[i] = RandULong();
            }
            return array;
        }
    }
}
