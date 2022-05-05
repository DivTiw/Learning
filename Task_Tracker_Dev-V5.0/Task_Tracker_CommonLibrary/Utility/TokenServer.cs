using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Task_Tracker_CommonLibrary.Utility
{
    public class TokenModel
    {
        //public int CreatedBy { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; } // Currently keeping it to default User
        public Dictionary<string, object> Payload { get; set; }
    }
    public class TokenServer
    {
        private JWT.JWT webtoken = null;
        string TokenDBConString = string.Empty;
        string strPrivateCert = string.Empty;
        string strPublicCert = string.Empty;
        string sPwd = string.Empty;

        public TokenServer(string _TokenDBConString = "")
        {
            TokenDBConString = !string.IsNullOrEmpty(_TokenDBConString) ? _TokenDBConString : ConfigurationManager.ConnectionStrings["CommonEmailConnection"].ToString();
            strPrivateCert = ConfigurationManager.AppSettings["Cert_Priv_Path"].ToString();
            strPublicCert = ConfigurationManager.AppSettings["Cert_Pub_Path"].ToString();
            sPwd = ConfigurationManager.AppSettings["Cert_Pass"].ToString();
            webtoken = new JWT.JWT(TokenDBConString, strPrivateCert, strPublicCert, sPwd);
        }

        public string GetToken(TokenModel tm)//int createdBy, int recordsSyscode, string mongoFileId = ""
        {
            try
            {
                string _token = string.Empty;
                Dictionary<string, Object> mDictObj_Token = new Dictionary<string, object>();
                mDictObj_Token.Add("sUserName", tm.UserID);
                mDictObj_Token.Add("Threshold", Convert.ToInt32(ConfigurationManager.AppSettings["Threshold"]));
                mDictObj_Token.Add("ProjectSyscode", Convert.ToInt32(ConfigurationManager.AppSettings["TokenProjectSysCode"]));
                mDictObj_Token.Add("ExpiryOn", ConfigurationManager.AppSettings["TokenExpiryOn"].ToString());
                mDictObj_Token.Add("sConnection", TokenDBConString);
                mDictObj_Token.Add("sTokenConnection", TokenDBConString);
                mDictObj_Token.Add("sPrivateCertificate", ConfigurationManager.AppSettings["Cert_Priv_Path"].ToString());
                mDictObj_Token.Add("sPublicCerificate", ConfigurationManager.AppSettings["Cert_Pub_Path"].ToString());
                mDictObj_Token.Add("sPwd", ConfigurationManager.AppSettings["Cert_Pass"].ToString());

                mDictObj_Token.Add("EmployeeSyscode", tm.UserID);
                mDictObj_Token.Add("IP", "");

                mDictObj_Token.Add("UserRole", tm.UserRole);

                //Dictionary<string, object> payload = new Dictionary<string, object>();
                //payload.Add("record_syscode", tm.AttachmentGUID);
                //if (!string.IsNullOrEmpty(tm.MongoID))
                //    payload.Add("file_id", tm.MongoID);
                _token = Encrypt(mDictObj_Token, tm.Payload);
                return _token;
            }
            catch (Exception e)
            {
                throw new Exception("Exception occured in TokenServer while generating the token. See inner exception", e);
            }
        }

        public string Encrypt(Dictionary<string, Object> dictionary, Dictionary<string, Object> Payload)
        {
            try
            {

                string sUserName = "", sIP = "", sUserRole = "";
                sUserName = dictionary["sUserName"].ToString();
                sIP = dictionary["IP"].ToString();
                sUserRole = dictionary["UserRole"]?.ToString();

                int Threshold = Convert.ToInt32(dictionary["Threshold"].ToString());
                int ProjectSyscode = Convert.ToInt32(dictionary["ProjectSyscode"].ToString());
                int ExpiryOn = Convert.ToInt32(dictionary["ExpiryOn"].ToString());
                int employeeSyscode = Convert.ToInt32(dictionary["EmployeeSyscode"].ToString());

                ///*** GEnerate JWT Token ***///////

                if (string.IsNullOrEmpty(sUserName))
                {
                    return string.Empty;
                }

                Dictionary<string, Object> tokenval = new Dictionary<string, object>();

                tokenval.Add("emp_syscode", employeeSyscode);
                tokenval.Add("project_syscode", ProjectSyscode);
                tokenval.Add("expires_on", ExpiryOn);
                if (!string.IsNullOrEmpty(sIP))
                    tokenval.Add("IP", sIP);

                if (!string.IsNullOrEmpty(sUserRole))
                {
                    tokenval.Add("UserRole", sUserRole);
                    Payload.Add("UserRole", sUserRole);
                }

                // start adding actual data to be decrypt
                if (Payload == null)
                {
                    Payload = new Dictionary<string, object>();
                }
                Payload.Add("emp_syscode", employeeSyscode);
                Payload.Add("project_syscode", ProjectSyscode);
                Payload.Add("sUserName", sUserName);

                tokenval.Add("payload", Payload);


                string Encval = webtoken.GenerateJWTToken(tokenval, Threshold);

                return Encval;
                ///******

            }
            catch (Exception ex)
            {
                throw new Exception("Exception occurred while encoding token. See inner exception!", ex);
            }

        }
        /// <summary>
        /// This is plain decode function without validation using JWT library.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string Decode(string token)
        {
            try
            {
                byte[] file = System.IO.File.ReadAllBytes(ConfigurationManager.AppSettings["Cert_Priv_Path"].ToString());
                var privateKey = new X509Certificate2(file, ConfigurationManager.AppSettings["Cert_Pass"].ToString(), X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet).PrivateKey as RSACryptoServiceProvider;
                string json = Jose.JWT.Decode(token + "", privateKey);
                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception occurred while decoding the token. See inner exception!", ex);
            }

        }
        /// <summary>
        /// This is decoding using inbuilt validation through database.
        /// </summary>
        /// <param name="tokenval"></param>
        /// <returns></returns>
        public string DecodeToken(string tokenval)
        {
            string decodetoken = "";
            decodetoken = webtoken.DecodeToken(tokenval);

            return decodetoken;
        }
        /// <summary>
        /// This is plain validation function through database.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool ValidateToken(string token)
        {
            bool isValidToken = false;

            isValidToken = webtoken.ValidateToken(token);

            return isValidToken;
        }
    }
}