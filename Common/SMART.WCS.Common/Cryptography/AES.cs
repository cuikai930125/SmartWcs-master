using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SMART.WCS.Common.Cryptography
{
    /// <summary>
    /// AES 암호화 (양방향, 단방향), 복호화 
    /// 2019-08-24
    /// 추성호
    /// </summary>
    public class AES : DisposeClass
    {
        #region ▩ 전역변수
        // 키
        private static readonly string KEY = "98765432101234567890987654321012";

        //128bit (16자리)
        private static readonly string KEY_128 = KEY.Substring(0, 128 / 8);

        //256bit (32자리)
        private static readonly string KEY_256 = KEY.Substring(0, 256 / 8);
        #endregion

        #region ▩ 함수
        #region > EncryptAES67 - AES256 암호화
        /// <summary>
        /// AES 256 암호화.., CBC, PKCS7, 예외발생하면 null
        /// </summary>
        /// <param name="_strToEncryptValue">암호화 대상값</param>
        /// <returns></returns>
        public static string EncryptAES256(string _strToEncryptValue)
        {
            try
            {
                byte[] bytesEncodingData    = Encoding.UTF8.GetBytes(_strToEncryptValue);
                var strEncryptedData        = string.Empty;

                using (RijndaelManaged rm = new RijndaelManaged())
                {
                    rm.Mode         = CipherMode.CBC;
                    rm.Padding      = PaddingMode.PKCS7;
                    rm.KeySize      = 256;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        //key, iv값 정의
                        using (ICryptoTransform ict = rm.CreateEncryptor(Encoding.UTF8.GetBytes(KEY_256), Encoding.UTF8.GetBytes(KEY_128)))
                        {
                            using (CryptoStream cs = new CryptoStream(ms, ict, CryptoStreamMode.Write))
                            {
                                cs.Write(bytesEncodingData, 0, bytesEncodingData.Length);
                                cs.FlushFinalBlock();
                            }
                        }

                        byte[] bytesEncrypt     = ms.ToArray();
                        strEncryptedData        =  Convert.ToBase64String(bytesEncrypt);
                    }
                }

                return strEncryptedData;
            }
            catch { throw; }
        }
        #endregion

        /// <summary>
        /// AES256 복호화.., CBC, PKCS7, 예외발생하면 null
        /// </summary>
        /// <param name="_strToDescryptValue">복호화 대상값</param>
        /// <returns></returns>
        public static string DecryptAES256(string _strToDescryptValue)
        {
            try
            {
                //base64를 바이트로 변환 
                byte[] bytesBase64Data      = Convert.FromBase64String(_strToDescryptValue);
                var strDecryptedData        = string.Empty;

                using (RijndaelManaged rm = new RijndaelManaged())
                {
                    rm.Mode         = CipherMode.CBC;
                    rm.Padding      = PaddingMode.PKCS7;
                    rm.KeySize      = 256;

                    using (MemoryStream ms = new MemoryStream(bytesBase64Data))
                    {
                        using (ICryptoTransform ict = rm.CreateDecryptor(Encoding.UTF8.GetBytes(KEY_256), Encoding.UTF8.GetBytes(KEY_128)))
                        {
                            using (CryptoStream cs = new CryptoStream(ms, ict, CryptoStreamMode.Read))
                            {
                                byte[] bytesDecrypt = new byte[bytesBase64Data.Length];
                                var iDataLength     = cs.Read(bytesBase64Data, 0, bytesBase64Data.Length);

                                strDecryptedData = Encoding.UTF8.GetString(bytesBase64Data, 0, iDataLength);
                            }
                        }
                    }

                    return strDecryptedData;
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// 단방향 암호화
        /// SHA256 해쉬 함수 암호화.., 예외발생하면 null
        /// </summary>
        /// <param name="_strToEncryptValue">암호화 대상값</param>
        /// <returns></returns>
        public static string EncryptSHA256(string _strToEncryptValue)
        {
            try
            {
                byte[] bytesEncodingData    = Encoding.UTF8.GetBytes(_strToEncryptValue);
                var strEncryptedData        = string.Empty;

                using (SHA256Managed sm = new SHA256Managed())
                {
                    byte[] bytesEncrypt     = sm.ComputeHash(bytesEncodingData);
                    strEncryptedData        = Convert.ToBase64String(bytesEncrypt);
                }

                return strEncryptedData;
            }
            catch { throw; }
        }
        #endregion
    }
}
