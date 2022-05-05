using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Tracker_CommonLibrary.Entity;

namespace Task_Tracker_Library.Repository
{
    public class LoginRepository : MasterRepository<GroupMaster>
    {
        string DbConnection;
        public LoginRepository(TTDBContext _context) : base(_context)
        {
            DbConnection = _context.Database.Connection.ConnectionString;//ConfigurationManager.ConnectionStrings["TaskTrackerConnection"].ConnectionString;
        }
        public LoginUser AuthoriseUser(string logonName)
        {

            SqlParameter @Windows_login_id = new SqlParameter()
            {
                ParameterName = "@Windows_login_id",
                DbType = DbType.String,
                Value = logonName
            };

            SqlParameter @status = new SqlParameter()
            {
                ParameterName = "@status",
                Direction = ParameterDirection.Output,
                DbType = DbType.Boolean
            };

            SqlParameter @return_value = new SqlParameter()
            {
                ParameterName = "@return_value",
                Direction = ParameterDirection.Output,
                DbType = DbType.String,
                Size = 1000
            };

            object[] parameters = new object[] { @Windows_login_id, @status, @return_value };

            //using (var context = new TTDBContext())
            //{
            var result = context.Database.SqlQuery<LoginUser>("EXEC [dbo].[proc_authorise_user] @Windows_login_id, @status, @return_value", parameters).ToList();
            return result[0];
            //}
        }

        public DataSet GetAllMenu()
        {
            SqlConnection con = new SqlConnection(DbConnection);

            SqlCommand cmd = new SqlCommand("proc_get_All_menus", con);
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();
            DataSet ds = new DataSet();

            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(ds);
            }
            con.Close();


            return ds;
        }

        public DataSet GetWindowsLoginId(int employee_syscode)
        {
            DataSet ds = new DataSet();

            using (SqlDataAdapter da = new SqlDataAdapter("proc_get_windows_login", DbConnection))
            {
                da.SelectCommand.CommandType = CommandType.StoredProcedure;

                da.SelectCommand.Parameters.Add(new SqlParameter("@employee_syscode", SqlDbType.Int));
                da.SelectCommand.Parameters["@employee_syscode"].Value = employee_syscode;

                da.Fill(ds);
            }

            return ds;
        }
    }
}
