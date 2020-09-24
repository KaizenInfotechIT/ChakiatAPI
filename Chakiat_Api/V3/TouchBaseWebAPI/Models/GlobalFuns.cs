using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using System.Text;
using System.Data;
using System.IO;
using TouchBaseWebAPI.Data;
using System.Reflection;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web;
using System.Drawing.Imaging;
using System.Drawing;
using System.Collections.Specialized;

namespace TouchBaseWebAPI.Models
{
    public static class GlobalFuns
    {
        public static DateTime GetFormattedDate(string strDateTime)
        {
            if (strDateTime == "")
            {
                return DateTime.ParseExact("01/01/1753", "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None); ;
            }
            else
            {
                strDateTime = strDateTime.Replace("-", "/");
                strDateTime = strDateTime.Replace(".", "/");
                string[] split = strDateTime.Split('/');
                if (split[0].Length == 1)
                {
                    split[0] = "0" + split[0];
                }
                if (split[1].Length == 1)
                {
                    split[1] = "0" + split[1];
                }
                strDateTime = split[0] + "/" + split[1] + "/" + split[2];
            }

            DateTime dt = DateTime.ParseExact(strDateTime, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None);

            return dt;

        }

        public static String GetMySqlFormattedDate(string strDateTime)
        {
            if (strDateTime == "")
            {
                return "1753/01/01";
            }
            else
            {
                string[] split = strDateTime.Split('/');
                if (split[0].Length == 1)
                {
                    split[0] = "0" + split[0];
                }
                if (split[1].Length == 1)
                {
                    split[1] = "0" + split[1];
                }
                strDateTime = split[2] + "-" + split[1] + "-" + split[0];
            }

            //DateTime dt = DateTime.ParseExact(strDateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            // string dt1 = dt.ToString("yyyy-MM-dd");

            // dt1 = string.Format("u", dt);
            //DateTime dt1 = DateTime.ParseExact(strDateTime, "yyyy-MM-dd", System.Globalization.CultureInfo.


            return strDateTime;

        }

        public static string CreateRandomPassword(int PasswordLength)
        {
            //string _allowedChars = "0123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ";
            string _allowedChars = "123456789";
            Random randNum = new Random();
            char[] chars = new char[PasswordLength];
            int allowedCharCount = _allowedChars.Length;
            for (int i = 0; i < PasswordLength; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }
            return new string(chars);
        }

        public static bool ChkDuplicateIDStr(string strTableName, string strFldName, string strWhereCon)
        {
            DataSet ds = new DataSet();
            string strWhere;

            if (strWhereCon == "")
            {
                strWhere = " ";
            }
            else
            {
                strWhere = " WHERE " + strWhereCon;
            }

            string ssql = "";
            ssql = "SELECT count(*) from " + strTableName +
                     " " + strWhere;
            try
            {
                ds = MySqlHelper.ExecuteDataset(GlobalVar.strAppConn, CommandType.Text, ssql);

                if (Convert.ToInt16(ds.Tables[0].Rows[0][0]) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //return false;
            }
        }

        #region Send SMS

        //send OTP SMS to National users.
        public static string SendAlertSMS(string strMobile, string strAlertText)
        {
            string dn = ConfigurationManager.AppSettings["dn"].ToString();
            string usr = ConfigurationManager.AppSettings["usr"].ToString();
            string pwd = ConfigurationManager.AppSettings["pwd"].ToString();
            string sid = ConfigurationManager.AppSettings["sid"].ToString();
            try
            {
                SendSMS(dn, usr, pwd, sid, strMobile, strAlertText);
                return "true";
            }
            catch
            {
                return "false";
            }
        }
        public static string SendSMS(string DomainName, string User, string Password, string SenderID, string PhoneNumber, string TextMessage)
        {
            string strValue = string.Empty;
            try
            {
                StringBuilder sms = new StringBuilder();
                //string txt = "Please enter one time password (OTP) " + TextMessage.Trim() + " to activate the mobile app you have just downloaded. It is valid only for 60 mins.";
                //string txt = TextMessage.Trim() + " is Your verification code for TouchBase.";

                sms.Append(DomainName);
                sms.Append("?user=" + User + ":" + Password);
                sms.Append("&state=4");
                // sms.Append("&ReceiptRequested=Yes");
                sms.Append("&senderID=" + SenderID);
                sms.Append("&receipientno=" + PhoneNumber);
                sms.Append("&msgtxt=" + TextMessage);


                //string strUrl = "http://api.mVaayoo.com/mvaayooapi/MessageCompose?user=shilpa.pal@advan-t-edge.com:advan@123&senderID=TEST SMS&receipientno="+PhoneNumber+"&msgtxt="+TextMessage+"&state=4";

                //string StrUrl = "http://api.mVaayoo.com/mvaayooapi/MessageCompose?user=shilpa.pal@advan-t-edge.com:advan@123&state=4&senderID=TEST SMS&receipientno=9762807172&msgtxt=EfxQ3pS3";
                WebRequest request = WebRequest.Create(sms.ToString());
                request.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                if (responseFromServer.Contains("Ok"))
                {
                    strValue = responseFromServer;
                }
                response.Close();
                reader.Close();
                dataStream.Close();
                response.Close();

                return strValue;
            }
            catch (Exception ex)
            {
                strValue = "Error to send SMS: " + ex.ToString();
                return strValue;
            }
        }

        //send general SMS to National users like on member Add/Remove,Ann/Event/Ebull etc.
        public static bool SendSMSOnAdd(string strMobile, string strAlertText)
        {
            bool flag = false;

            string dn = ConfigurationManager.AppSettings["dnapp"].ToString();
            string usr = ConfigurationManager.AppSettings["usrapp"].ToString();
            string pwd = ConfigurationManager.AppSettings["pwdapp"].ToString();
            string sid = ConfigurationManager.AppSettings["sidapp"].ToString();
            try
            {
                SendSMSAdd(dn, usr, pwd, sid, strMobile, strAlertText);
                flag = true;
            }
            catch
            {
                flag = false;
            }

            return flag;
        }
        public static string SendSMSAdd(string DomainName, string User, string Password, string SenderID, string PhoneNumber, string TextMessage)
        {
            string strValue = string.Empty;
            try
            {
                StringBuilder sms = new StringBuilder();

                sms.Append(DomainName);
                sms.Append("?username=" + User + "&pass=" + Password);
                //sms.Append("&state=4");
                sms.Append("&dest_mobileno=" + PhoneNumber);
                sms.Append("&senderid=" + SenderID);
                sms.Append("&message=" + TextMessage);

                WebRequest request = WebRequest.Create(sms.ToString());
                request.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                if (responseFromServer.Contains("Ok"))
                {
                    strValue = responseFromServer;
                }
                response.Close();
                reader.Close();
                dataStream.Close();
                response.Close();

                return strValue;
            }
            catch (Exception ex)
            {
                strValue = "Error to send SMS: " + ex.ToString();
                return strValue;
            }
        }


        //send OTP SMS to InterNational users.
        public static string SendSMSInternational(string strMobile, string strAlertText, string CountryCode)
        {
            string strValue = string.Empty;
            //ds.Clear();
            string dn = ConfigurationManager.AppSettings["dn_int"].ToString();
            string apikey = ConfigurationManager.AppSettings["apikey"].ToString();
            string sid = ConfigurationManager.AppSettings["sid_int"].ToString();

            string Number = CountryCode + strMobile;
            try
            {
                SendSMSInter(dn, apikey, sid, Number, strAlertText);
            }
            catch (Exception ex)
            {
                strValue = "Error to send SMS: " + ex.ToString();
                return strValue;
            }
            strValue = "SMS Send SuccessFully";
            return strValue;

        }
        public static string SendSMSInter(string DomainName, string apikey, string SenderID, string PhoneNumber, string TextMessage)
        {
            string strValue = string.Empty;
            try
            {
                StringBuilder sms = new StringBuilder();
                //string txt = "Please enter one time password (OTP) " + TextMessage.Trim() + " to activate the mobile app you have just downloaded. It is valid only for 60 mins.";
                string txt = TextMessage.Trim() + " is Your verification code for TouchBase.";

                sms.Append(DomainName);
                sms.Append("?api_key=" + apikey);
                sms.Append("&method=sms");
                sms.Append("&sender=" + SenderID);
                sms.Append("&to=" + PhoneNumber);
                sms.Append("&message=" + txt);

                WebRequest request = WebRequest.Create(sms.ToString());
                request.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                if (responseFromServer.Contains("Ok"))
                {
                    strValue = responseFromServer;
                }
                response.Close();
                reader.Close();
                dataStream.Close();
                response.Close();

                return strValue;
            }
            catch (Exception ex)
            {
                strValue = "Error to send SMS: " + ex.ToString();
                return strValue;
            }
        }

        //send general SMS to InterNational users like on member Add/Remove,Ann/Event/Ebull etc.
        public static bool SendSMSInterOnAdd(string strMobile, string strAlertText, string CountryCode)
        {
            //string strValue = string.Empty;
            //ds.Clear();

            bool flag = false;

            string dn = ConfigurationManager.AppSettings["dn_int"].ToString();
            string apikey = ConfigurationManager.AppSettings["apikey"].ToString();
            string sid = ConfigurationManager.AppSettings["sid_int"].ToString();

            string Number = CountryCode + strMobile;
            try
            {
                SendSMSInterAdd(dn, apikey, sid, Number, strAlertText);
                flag = true;
            }
            catch 
            {
                //strValue = "Error to send SMS: " + ex.ToString();
                //return strValue;
                flag = false;
            }

            //strValue = "SMS Send SuccessFully";
            //return strValue;

            return flag;
        }
        public static string SendSMSInterAdd(string DomainName, string apikey, string SenderID, string PhoneNumber, string TextMessage)
        {
            string strValue = string.Empty;
            try
            {
                StringBuilder sms = new StringBuilder();

                sms.Append(DomainName);
                sms.Append("?api_key=" + apikey);
                sms.Append("&method=sms");
                sms.Append("&sender=" + SenderID);
                sms.Append("&to=" + PhoneNumber);
                sms.Append("&message=" + TextMessage);

                WebRequest request = WebRequest.Create(sms.ToString());
                request.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                if (responseFromServer.Contains("Ok"))
                {
                    strValue = responseFromServer;
                }
                response.Close();
                reader.Close();
                dataStream.Close();
                response.Close();

                return strValue;
            }
            catch (Exception ex)
            {
                strValue = "Error to send SMS: " + ex.ToString();
                return strValue;
            }
        }

        #endregion

        public static List<T> DataTableToList<T>(this DataTable table) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

        public static string SendEmail(string ToMailID, string MsgSubject, string MsgBody)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient();
                MailMessage message = new MailMessage();

                MailAddress fromAddress = new MailAddress(ConfigurationManager.AppSettings["frommail"].ToString());
                smtpClient.Host = ConfigurationManager.AppSettings["smtpserver"].ToString();
                smtpClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings["port"].ToString());
                smtpClient.EnableSsl = true;

                message.From = fromAddress;
                message.To.Add(ToMailID);

                message.Subject = MsgSubject;
                message.IsBodyHtml = true;
                message.Body = MsgBody;

                NetworkCredential basicAuthentication = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["frommail"].ToString(), ConfigurationManager.AppSettings["pass"].ToString());
                smtpClient.Credentials = basicAuthentication;

                try
                {
                    smtpClient.Send(message);
                }
                catch
                {
                    return "ServerError";
                }

                return string.Empty;

            }
            catch (SmtpFailedRecipientsException ex)
            {
                return "Error to send mail: " + ex.Message;
            }

        }

        public static string Encrypt(string plainText,
                                 string passPhrase,
                                 string saltValue,
                                 string hashAlgorithm,
                                 int passwordIterations,
                                 string initVector,
                                 int keySize)
        {

            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            // Convert our plaintext into a byte array.
            // Let us assume that plaintext contains UTF8-encoded characters.
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // First, we must create a password, from which the key will be derived.
            // This password will be generated from the specified passphrase and 
            // salt value. The password will be created using the specified hash 
            // algorithm. Password creation can be done in several iterations.
            PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                            passPhrase,
                                                            saltValueBytes,
                                                            hashAlgorithm,
                                                            passwordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = password.GetBytes(keySize / 8);

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged();

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;

            // Generate encryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(
                                                             keyBytes,
                                                             initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream();

            // Define cryptographic stream (always use Write mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                         encryptor,
                                                         CryptoStreamMode.Write);
            // Start encrypting.
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

            // Finish encrypting.
            cryptoStream.FlushFinalBlock();

            // Convert our encrypted data from a memory stream into a byte array.
            byte[] cipherTextBytes = memoryStream.ToArray();

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert encrypted data into a base64-encoded string.
            string cipherText = Convert.ToBase64String(cipherTextBytes);

            // Return encrypted string.
            return cipherText;
        }

        public static string Decrypt(string cipherText,
                                     string passPhrase,
                                     string saltValue,
                                     string hashAlgorithm,
                                     int passwordIterations,
                                     string initVector,
                                     int keySize)
        {
            // Convert strings defining encryption key characteristics into byte
            // arrays. Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8
            // encoding.
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            // Convert our ciphertext into a byte array.
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            // First, we must create a password, from which the key will be 
            // derived. This password will be generated from the specified 
            // passphrase and salt value. The password will be created using
            // the specified hash algorithm. Password creation can be done in
            // several iterations.
            PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                            passPhrase,
                                                            saltValueBytes,
                                                            hashAlgorithm,
                                                            passwordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = password.GetBytes(keySize / 8);

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged();

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;

            // Generate decryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(
                                                             keyBytes,
                                                             initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            // Define cryptographic stream (always use Read mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                          decryptor,
                                                          CryptoStreamMode.Read);

            // Since at this point we don't know what the size of decrypted data
            // will be, allocate the buffer long enough to hold ciphertext;
            // plaintext is never longer than ciphertext.
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            // Start decrypting.
            int decryptedByteCount = cryptoStream.Read(plainTextBytes,
                                                       0,
                                                       plainTextBytes.Length);

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert decrypted data into a string. 
            // Let us assume that the original plaintext string was UTF8-encoded.
            string plainText = Encoding.UTF8.GetString(plainTextBytes,
                                                       0,
                                                       decryptedByteCount);

            // Return decrypted string.   
            return plainText;
        }

        public static string DecryptPara(string cipherText,
                                     string passPhrase,
                                     string saltValue,
                                     string hashAlgorithm,
                                     int passwordIterations,
                                     string initVector,
                                     int keySize)
        {
            // Convert strings defining encryption key characteristics into byte
            // arrays. Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8
            // encoding.
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            // Convert our ciphertext into a byte array.
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText.Replace(" ", "+"));

            // First, we must create a password, from which the key will be 
            // derived. This password will be generated from the specified 
            // passphrase and salt value. The password will be created using
            // the specified hash algorithm. Password creation can be done in
            // several iterations.
            PasswordDeriveBytes password = new PasswordDeriveBytes(
                                                            passPhrase,
                                                            saltValueBytes,
                                                            hashAlgorithm,
                                                            passwordIterations);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = password.GetBytes(keySize / 8);

            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged();

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;

            // Generate decryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(
                                                             keyBytes,
                                                             initVectorBytes);

            // Define memory stream which will be used to hold encrypted data.
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            // Define cryptographic stream (always use Read mode for encryption).
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                          decryptor,
                                                          CryptoStreamMode.Read);

            // Since at this point we don't know what the size of decrypted data
            // will be, allocate the buffer long enough to hold ciphertext;
            // plaintext is never longer than ciphertext.
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            // Start decrypting.
            int decryptedByteCount = cryptoStream.Read(plainTextBytes,
                                                       0,
                                                       plainTextBytes.Length);

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert decrypted data into a string. 
            // Let us assume that the original plaintext string was UTF8-encoded.
            string plainText = Encoding.UTF8.GetString(plainTextBytes,
                                                       0,
                                                       decryptedByteCount);

            // Return decrypted string.   
            return plainText;
        }

        public static string EncryptPassward(string strPassword)
        {
            //string plainText = "Hello, World!";    // original plaintext

            string passPhrase = "Pas5pr@se";        // can be any string
            string saltValue = "s@1tValue";        // can be any string
            string hashAlgorithm = "SHA1";             // can be "MD5"
            int passwordIterations = 2;                  // can be any number
            string initVector = "@1B2c3D4e5F6g7H8"; // must be 16 bytes
            int keySize = 256;                // can be 192 or 128

            //Console.WriteLine(String.Format("Plaintext : {0}", plainText));

            string EncryptPassword = Encrypt(strPassword,
                                                        passPhrase,
                                                        saltValue,
                                                        hashAlgorithm,
                                                        passwordIterations,
                                                        initVector,
                                                        keySize);
            return EncryptPassword;
        }
        public static string DecryptPassward(string strPassword)
        {
            //string plainText = "Hello, World!";    // original plaintext

            string passPhrase = "Pas5pr@se";        // can be any string
            string saltValue = "s@1tValue";        // can be any string
            string hashAlgorithm = "SHA1";             // can be "MD5"
            int passwordIterations = 2;                  // can be any number
            string initVector = "@1B2c3D4e5F6g7H8"; // must be 16 bytes
            int keySize = 256;                // can be 192 or 128

            //Console.WriteLine(String.Format("Plaintext : {0}", plainText));

            string DecryptPassword = Decrypt(strPassword,
                                                        passPhrase,
                                                        saltValue,
                                                        hashAlgorithm,
                                                        passwordIterations,
                                                        initVector,
                                                        keySize);
            return DecryptPassword;
        }

        /// <summary>
        /// Create module directory. Inside Modules create group directory. then save image
        /// </summary>
        /// <param name="grpID"></param>
        /// <param name="imgName"></param>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public static int UploadImage(string grpID, string imgName, string moduleName)
        {
            bool flag = false;
            try
            {
                if (!string.IsNullOrEmpty(grpID))
                {
                    //string NewDir = HttpContext.Current.Server.MapPath("~/Documents/" + moduleName + "/Group" + grpID + "");
                    //string a = HttpContext.Current.Server.MapPath("~/TempDocuments/" + imgName).ToString();

                    string NewDir = ConfigurationManager.AppSettings["imgPathSave"] + "Documents\\" + moduleName + "\\Group" + grpID;
                    string a = ConfigurationManager.AppSettings["imgPathSave"] + "TempDocuments\\" + imgName;

                    foreach (var file in Directory.GetFiles(ConfigurationManager.AppSettings["imgPathSave"] + "TempDocuments", "*.png"))
                    {
                        if (!Directory.Exists(NewDir))
                            Directory.CreateDirectory(NewDir);

                        string FileName = Path.GetFileName(file);

                        if (FileName == imgName)
                        {
                            File.Move(file, Path.Combine(NewDir, FileName));

                            //generate thumb image
                            System.Drawing.Image img1 = System.Drawing.Image.FromFile(NewDir + "\\" + imgName);
                            //System.Drawing.Image bmp1 = img1.GetThumbnailImage(50, 50, null, IntPtr.Zero);
                            using (var newImage = ScaleImage(img1, 250, 250))
                            {
                                string path = NewDir + "\\thumb\\";

                                if (!Directory.Exists(path))
                                    Directory.CreateDirectory(path);
                                newImage.Save(path + "/" + imgName, ImageFormat.Png);
                            }
                            flag = true;
                        }
                    }
                }
                if (flag)
                    return 0;
                else
                    return 1;
            }
            catch
            {
                return 1;
            }
        }

        public static int DeleteFile(string grpID, string moduleName, string imgName)
        {
            bool flag = false;
            try
            {
                if (!string.IsNullOrEmpty(grpID))
                {
                    //string imagePath = HttpContext.Current.Server.MapPath("~/Documents/" + moduleName + "/Group" + grpID);
                    string imagePath = ConfigurationManager.AppSettings["imgPathSave"] + "Documents\\" + moduleName + "\\Group" + grpID;

                    if (!Directory.Exists(imagePath))
                    {
                        flag = false;
                    }
                    else
                    {
                        System.IO.File.Delete(imagePath + "\\" + imgName);
                        System.IO.File.Delete(imagePath + "\\thumb\\" + imgName);
                        flag = true;
                    }
                }
                if (flag)
                    return 0;
                else
                    return 1;
            }
            catch
            {
                return 1;
            }
        }

        /// <summary>
        /// Create Image directly inside Module directory
        /// </summary>
        /// <param name="imgName"></param>
        /// <param name="UID"></param>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public static int UploadImage(string imgName, int UID, string moduleName)
        {
            bool flag = false;
            try
            {
                //string moduleName = "memberProfile";

                //string NewDir = HttpContext.Current.Server.MapPath("~/Documents/" + moduleName + "/");
                //string a = HttpContext.Current.Server.MapPath("~/TempDocuments/" + imgName).ToString();

                string NewDir = ConfigurationManager.AppSettings["imgPathSave"] + "Documents\\" + moduleName + "\\";
                string a = ConfigurationManager.AppSettings["imgPathSave"] + "TempDocuments\\" + imgName;

                foreach (var file in Directory.GetFiles(ConfigurationManager.AppSettings["imgPathSave"] + "TempDocuments", "*.png"))
                {
                    if (!Directory.Exists(NewDir))
                        Directory.CreateDirectory(NewDir);

                    string FileName = Path.GetFileName(file);

                    if (FileName == imgName)
                    {
                        File.Move(file, Path.Combine(NewDir, FileName));

                        //generate thumb image
                        System.Drawing.Image img1 = System.Drawing.Image.FromFile(NewDir + "\\" + imgName);
                        //  System.Drawing.Image bmp1 = img1.GetThumbnailImage(50, 50, null, IntPtr.Zero);
                        using (var newImage = ScaleImage(img1, 250, 250))
                        {
                            string path = NewDir + "\\thumb\\";
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);
                            newImage.Save(path + "/" + imgName, ImageFormat.Png);
                        }
                        flag = true;
                    }
                }
                if (flag)
                    return 0;
                else
                    return 1;
            }

            catch
            {
                return -1;
            }
        }

        public static int uploadDocs(string grpID, string imgName, string moduleName)
        {
            try
            {
                //string NewDir = HttpContext.Current.Server.MapPath("~/Documents/" + moduleName + "/Group" + grpID + "");
                //string a = HttpContext.Current.Server.MapPath("~/TempDocuments/" + imgName).ToString();

                string NewDir = ConfigurationManager.AppSettings["imgPathSave"] + "Documents\\" + moduleName + "\\Group" + grpID;
                string a = ConfigurationManager.AppSettings["imgPathSave"] + "TempDocuments\\" + imgName;

                foreach (var file in Directory.GetFiles(ConfigurationManager.AppSettings["imgPathSave"] + "TempDocuments"))
                {
                    if (!Directory.Exists(NewDir))
                        Directory.CreateDirectory(NewDir);

                    string FileName = Path.GetFileName(file);

                    if (FileName == imgName)
                    {
                        File.Move(file, Path.Combine(NewDir, FileName));
                        break;
                    }
                }

                return 0;
            }
            catch
            {
                return 1;
            }
        }
        /// <summary>
        /// Resize Image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        public static System.Drawing.Image ScaleImage(System.Drawing.Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        public static string SendEmail(string FromMailID, string ToMailID, string MsgSubject, string MsgBody)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient();
                MailMessage message = new MailMessage();

                MailAddress fromAddress = new MailAddress(FromMailID);

                smtpClient.Host = ConfigurationManager.AppSettings["smtpserver"].ToString();
                smtpClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings["port"].ToString());
                smtpClient.EnableSsl = true;

                message.From = fromAddress;

                //MailAddress copy = new MailAddress("amod.m@kaizeninfotech.com");
                //message.CC.Add(copy);
                //MailAddress copy1 = new MailAddress("harita.koli@kaizeninfotech.com");
                //message.CC.Add(copy1);

                message.To.Add(ToMailID);
                message.Subject = MsgSubject;
                message.IsBodyHtml = true;
                message.Body = MsgBody;

                NetworkCredential basicAuthentication = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["frommail"].ToString(), ConfigurationManager.AppSettings["pass"].ToString());
                smtpClient.Credentials = basicAuthentication;

                try
                {
                    smtpClient.Send(message);
                }
                catch 
                {
                    return "ServerError";
                }

                return string.Empty;

            }
            catch (SmtpFailedRecipientsException ex)
            {
                return "Error to send mail: " + ex.Message;
            }

        }

        public static string SendElasticEmail(string to, string subject, string bodyText, string bodyHtml, string from, string fromName)
        {
            string USERNAME = "sudhir.mane@kaizeninfotech.com";
            string API_KEY = "33904205-96b7-4f50-b60e-d26623c1eedf";

            WebClient client = new WebClient();
            NameValueCollection values = new NameValueCollection();
            values.Add("username", USERNAME);
            values.Add("api_key", API_KEY);
            values.Add("from", from);
            values.Add("from_name", fromName);
            values.Add("subject", subject);
            if (bodyHtml != null)
                values.Add("body_html", bodyHtml);
            if (bodyText != null)
                values.Add("body_text", bodyText);
            values.Add("to", to);

            byte[] response = client.UploadValues("https://api.elasticemail.com/mailer/send", values);
            return Encoding.UTF8.GetString(response);
        }

        public static void ExecuteCommandSync(string username, int type)//type 1/2 register/unregister
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("cd C:\\Program Files\\ejabberd-16.06\\bin");
                System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c" + sb.ToString());
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();

                sb = new StringBuilder();

                if (type == 0)
                { sb.Append("ejabberdctl register \"'" + username + "'\" \"version.touchbase.in\" \"'" + username + "'\""); }
                else
                { sb.Append("ejabberdctl unregister \"'" + username + "'\" \"version.touchbase.in\""); }

                procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c" + sb.ToString());
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;
                proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();

                string result = proc.StandardOutput.ReadToEnd();
            }
            catch 
            {
            }
        }

        public static void ExecuteCommandChat(string username, int mode)
        {
            System.Diagnostics.Process p = null;
            try
            {
                string targetDir;
                //targetDir = string.Format(@"D:\");
                targetDir = string.Format(ConfigurationManager.AppSettings["chatPath"]);

                p = new System.Diagnostics.Process();
                p.StartInfo.WorkingDirectory = targetDir;

                if (mode == 0)
                {
                    p.StartInfo.FileName = "RegisterUserChat.bat";
                }
                else
                {
                    p.StartInfo.FileName = "UnregisterUserChat.bat";
                }

                p.StartInfo.Arguments = string.Format(username);
                p.StartInfo.CreateNoWindow = true;//false
                p.Start();
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Occurred :{0},{1}",
                          ex.Message, ex.StackTrace.ToString());
            }
        }
    }
}