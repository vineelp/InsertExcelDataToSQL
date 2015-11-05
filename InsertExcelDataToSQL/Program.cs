using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using InsertExcelDataToSQL.DAL;
using System.Data.Entity.Validation;
using System.Data;
using System.IO;

namespace InsertExcelDataToSQL
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"Resource\UserData.xlsx";
            var currentDir = System.IO.Directory.GetCurrentDirectory();
            if (currentDir.ToLower().EndsWith(@"\bin\debug") ||
                 currentDir.ToLower().EndsWith(@"\bin\release"))
            {
                path = System.IO.Path.GetFullPath(@"..\..\" + path);
            }
            else
            {
                path = System.IO.Path.GetFullPath(path);
            }
            importdatafromexcel(path);//(@"D:\UserData.xlsx");
        }

        public static void importdatafromexcel(string excelfilepath)
        {
            int count = 0;
            DataTable dtExcel = new DataTable();
            string SourceConstr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + excelfilepath + "';Extended Properties= 'Excel 8.0;HDR=Yes;IMEX=1'";
            OleDbConnection con = new OleDbConnection(SourceConstr);
            string query = "Select * from [Sheet1$]";
            OleDbDataAdapter data = new OleDbDataAdapter(query, con);
            data.Fill(dtExcel);
            for (int i = 0; i < dtExcel.Rows.Count; i++)
            {
                try
                {
                    count++;
                    string userName = dtExcel.Rows[i][0].ToString();
                    string firstName = dtExcel.Rows[i][1].ToString();
                    string lastName = dtExcel.Rows[i][2].ToString();
                    string emailAddress = dtExcel.Rows[i][3].ToString();
                    string password = dtExcel.Rows[i][4].ToString();
                    int roleID = Convert.ToInt32(dtExcel.Rows[i][5]);
                    AddUser(userName, firstName, lastName, emailAddress, password, roleID);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            if (count == dtExcel.Rows.Count)
            {
                Console.WriteLine("success");
            }
            else
            {
                Console.WriteLine("failed");
            }
        }

        public static void AddUser(string userName, string firstName, string lastName, string emailAddress, string password, int roleID)
        {
            try
            {
                using (var ctxt = new AuthenticationEntities())
                {
                    ctxt.Users.Add(new User() { UserName = userName, FirstName = firstName, LastName = lastName, Password = password, RoleID = roleID, EmailAddress = emailAddress });
                    ctxt.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }
    }
}
