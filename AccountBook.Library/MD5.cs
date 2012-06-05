using System.Security.Cryptography;
using System.Text;

namespace AccountBook.Library
{
    /// <summary>
    /// Md5加密
    /// 返回md5hash字符串
    /// 返回兼容asp 的 md5的值
    /// 16位为兼容asp加密类
    /// 32位为.net正常加密类
    /// </summary>
    public static class MD5
    {
        /// <summary>
        /// 16位MD5加密方法,以前的DVBBS所使用
        /// </summary>
        /// <param name="strSource">待加密字串</param>
        /// <returns>加密后的字串</returns>
        public static string MD5Encrypt(this string strSource)
        {
            return MD5Encrypt(strSource, 32);
        }

        /// <summary>
        /// MD5加密,和动网上的16/32位MD5加密结果相同
        /// </summary>
        /// <param name="strSource">待加密字串</param>
        /// <param name="length">16或32值之一,其它则采用.net默认MD5加密算法</param>
        /// <returns>加密后的字串</returns>
        public static string MD5Encrypt(this string strSource, int length)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(strSource);
            byte[] hashValue = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            switch (length)
            {
                case 16:
                    for (int i = 4; i < 12; i++)
                        sb.Append(hashValue[i].ToString("x2"));
                    break;
                case 32:
                    for (int i = 0; i < 16; i++)
                    {
                        sb.Append(hashValue[i].ToString("x2"));
                    }
                    break;
                default:
                    for (int i = 0; i < hashValue.Length; i++)
                    {
                        sb.Append(hashValue[i].ToString("x2"));
                    }
                    break;
            }
            return sb.ToString();
        }
    }
}