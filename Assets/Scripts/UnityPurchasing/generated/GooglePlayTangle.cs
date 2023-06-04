// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("ks4sdig/LHuBhy0Dg9PNdKRNl3pz1tOW0H7fgMtjYaQSpk+ebD/W36CKucjaDxb3007T2t/Zap3KUU9UlXYDJQHJsFb/fYC23ke/7lMbFjfz5uVIKTouCAlvHoCB/kNVOAxcdWbi9A70yTdKxDI2hKMyz+P7fjgob+zi7d1v7Ofvb+zs7Ww2qMg1c/KPw0aXTeO6rMqVkxXmB5VO3Ml9HL7UWGG9Ce2CHy6jMRk2Gc1Y+FuJ25lwiBZt6ePkKXioMDVZ7RgUFmoiG9zpPEvoTUEgkGDuHrN8O1/e2PxYbAxAPSn2xX9CRNwG5mhXpojXjWPYirGzTUFgTIjME0HolQu0/l7db+zP3eDr5MdrpWsa4Ozs7Ojt7tDffnOq9jGUNO/u7O3s");
        private static int[] order = new int[] { 7,2,10,8,6,13,10,8,10,12,11,13,12,13,14 };
        private static int key = 237;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
