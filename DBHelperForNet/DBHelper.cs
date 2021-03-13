using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DBHelperForNet
{
    public class DBHelper
    {
        private string conString;

        public DBHelper(string connString)
        {
            this.conString = "";
            this.conString = ConfigurationManager.ConnectionStrings[connString].ConnectionString;
        }

        public void InitConnString(string connString)
        {
            this.conString = ConfigurationManager.ConnectionStrings[connString].ConnectionString;
        }

        public int InsertCommand(string tableName, DBParams inserted)
        {
            int num = -1;
            SqlConnection connection = new SqlConnection(this.conString);
            try
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                string str = "insert into " + tableName + " ";
                string str2 = "";
                string str3 = "";
                foreach (DBParam param in inserted.GetParams())
                {
                    str2 = str2 + param.Name + ",";
                    str3 = str3 + "'" + param.Value.ToString() + "',";
                }
                str2 = "(" + str2.Substring(0, str2.Length - 1) + ")";
                str3 = "(" + str3.Substring(0, str3.Length - 1) + ")";
                str = str + str2 + " values " + str3;
                command.CommandText = str;
                command.CommandType = CommandType.Text;
                num = command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                MyUtilities.WriteToLogFile("InsertCommand : " + exception.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return num;
        }

        public int InsertUpdateDeleteCommand(string command)
        {
            SqlConnection connection = new SqlConnection(this.conString);
            int num = -1;
            try
            {
                connection.Open();
                SqlCommand command2 = connection.CreateCommand();
                command2.CommandText = command;
                num = command2.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                MyUtilities.WriteToLogFile("InsertUpdateCommand : " + exception.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return num;
        }

        public int InsertUpdateDeleteProcedure(string procedureName, params DBParam[] @params)
        {
            SqlConnection connection = new SqlConnection(this.conString);
            int num = -1;
            try
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = procedureName;
                command.CommandType = CommandType.StoredProcedure;
                foreach (DBParam param in @params)
                {
                    SqlParameter parameter = new SqlParameter(param.Name, param.Value);
                    if (param.HasType())
                    {
                        parameter.DbType = param.MyType;
                    }
                    if (param.HasSize())
                    {
                        parameter.Size = param.Size;
                    }
                    command.Parameters.Add(parameter);
                }
                num = command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                MyUtilities.WriteToLogFile("InsertUpdateProcedure : " + exception.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return num;
        }

        public int InsertUpdateDeleteProcedure(string procedureName, DBParams @params)
        {
            SqlConnection connection = new SqlConnection(this.conString);
            int num = -1;
            try
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = procedureName;
                command.CommandType = CommandType.StoredProcedure;
                foreach (DBParam param in @params.GetParams())
                {
                    SqlParameter parameter = new SqlParameter(param.Name, param.Value);
                    if (param.HasType())
                    {
                        parameter.DbType = param.MyType;
                    }
                    if (param.HasSize())
                    {
                        parameter.Size = param.Size;
                    }
                    command.Parameters.Add(parameter);
                }
                num = command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                MyUtilities.WriteToLogFile("InsertUpdateProcedure : " + exception.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return num;
        }

        public int InsertUpdateDeleteProcedure(string procedureName, params ArrayList[] @params)
        {
            SqlConnection connection = new SqlConnection(this.conString);
            int num = -1;
            try
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = procedureName;
                command.CommandType = CommandType.StoredProcedure;
                foreach (ArrayList list in @params)
                {
                    SqlParameter parameter = new SqlParameter(list[0].ToString(), list[1]);
                    if (list.Count > 2)
                    {
                        parameter.DbType = (DbType)list[2];
                    }
                    if (list.Count > 3)
                    {
                        parameter.Size = (int)list[3];
                    }
                    command.Parameters.Add(parameter);
                }
                num = command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                MyUtilities.WriteToLogFile("InsertUpdateProcedure : " + exception.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return num;
        }

        public DataTable SelectCommand(string command)
        {
            SqlConnection connection = new SqlConnection(this.conString);
            DataTable dataTable = new DataTable();
            try
            {
                connection.Open();
                SqlCommand command2 = connection.CreateCommand();
                command2.CommandText = command;
                new SqlDataAdapter { SelectCommand = command2 }.Fill(dataTable);
            }
            catch (Exception exception)
            {
                MyUtilities.WriteToLogFile("SelectCommand : " + exception.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable SelectCommand(string tableName, List<string> selected, DBParams whereParams)
        {
            DataTable dataTable = new DataTable();
            SqlConnection connection = new SqlConnection(this.conString);
            try
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                string str = "select ";
                using (List<string>.Enumerator enumerator = selected.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        string current = enumerator.Current;
                        str = str + ",";
                    }
                }
                str = str.Substring(0, str.Length - 1) + " where ";
                foreach (DBParam param in whereParams.GetParams())
                {
                    string str2 = str;
                    str = str2 + param.Name + " = '" + param.Value.ToString() + "' and ";
                }
                str = str.Substring(0, str.Length - 5);
                command.CommandText = str;
                command.CommandType = CommandType.Text;
                new SqlDataAdapter { SelectCommand = command }.Fill(dataTable);
            }
            catch (Exception exception)
            {
                MyUtilities.WriteToLogFile("SelectCommand : " + exception.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable SelectCommand(string tableName, string[] selected, DBParams whereParams)
        {
            DataTable dataTable = new DataTable();
            SqlConnection connection = new SqlConnection(this.conString);
            try
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                string str = "select ";
                foreach (string str2 in selected)
                {
                    str = str + str2 + ",";
                }
                str = str.Substring(0, str.Length - 1) + " from " + tableName + " where ";
                foreach (DBParam param in whereParams.GetParams())
                {
                    string str3 = str;
                    str = str3 + param.Name + " = '" + param.Value.ToString() + "' and ";
                }
                str = str.Substring(0, str.Length - 5);
                command.CommandText = str;
                command.CommandType = CommandType.Text;
                new SqlDataAdapter { SelectCommand = command }.Fill(dataTable);
            }
            catch (Exception exception)
            {
                MyUtilities.WriteToLogFile("SelectCommand : " + exception.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable SelectProcedure(string procedureName, params DBParam[] @params)
        {
            DataTable dataTable = new DataTable();
            SqlConnection connection = new SqlConnection(this.conString);
            try
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = procedureName;
                command.CommandType = CommandType.StoredProcedure;
                foreach (DBParam param in @params)
                {
                    SqlParameter parameter = new SqlParameter(param.Name, param.Value);
                    if (param.HasType())
                    {
                        parameter.DbType = param.MyType;
                    }
                    if (param.HasSize())
                    {
                        parameter.Size = param.Size;
                    }
                    command.Parameters.Add(parameter);
                }
                new SqlDataAdapter { SelectCommand = command }.Fill(dataTable);
            }
            catch (Exception exception)
            {
                MyUtilities.WriteToLogFile("SelectProcedure : " + exception.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable SelectProcedure(string procedureName, DBParams @params)
        {
            DataTable dataTable = new DataTable();
            SqlConnection connection = new SqlConnection(this.conString);
            try
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = procedureName;
                command.CommandType = CommandType.StoredProcedure;
                foreach (DBParam param in @params.GetParams())
                {
                    SqlParameter parameter = new SqlParameter(param.Name, param.Value);
                    if (param.HasType())
                    {
                        parameter.DbType = param.MyType;
                    }
                    if (param.HasSize())
                    {
                        parameter.Size = param.Size;
                    }
                    command.Parameters.Add(parameter);
                }
                new SqlDataAdapter { SelectCommand = command }.Fill(dataTable);
            }
            catch (Exception exception)
            {
                MyUtilities.WriteToLogFile("SelectProcedure : " + exception.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        public DataTable SelectProcedure(string procedureName, params ArrayList[] @params)
        {
            DataTable dataTable = new DataTable();
            SqlConnection connection = new SqlConnection(this.conString);
            try
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = procedureName;
                command.CommandType = CommandType.StoredProcedure;
                foreach (ArrayList list in @params)
                {
                    SqlParameter parameter = new SqlParameter(list[0].ToString(), list[1]);
                    if (list.Count > 2)
                    {
                        parameter.DbType = (DbType)list[2];
                    }
                    if (list.Count > 3)
                    {
                        parameter.Size = (int)list[3];
                    }
                    command.Parameters.Add(parameter);
                }
                new SqlDataAdapter { SelectCommand = command }.Fill(dataTable);
            }
            catch (Exception exception)
            {
                MyUtilities.WriteToLogFile("SelectProcedure : " + exception.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dataTable;
        }

        //public object SelectScalarCommand(string command)
        //{
        //    SqlConnection connection = new SqlConnection(this.conString);
        //    object obj2 = new object();
        //    try
        //    {
        //        connection.Open();
        //        SqlCommand command2 = connection.CreateCommand();
        //        command2.CommandText = command;
        //        new SqlDataAdapter();
        //        obj2 = command2.ExecuteScalar();
        //    }
        //    catch (Exception exception)
        //    {
        //        MyUtilities.WriteToLogFile("SelectScalarCommand : " + exception.Message);
        //    }
        //    finally
        //    {
        //        if (connection.State == ConnectionState.Open)
        //        {
        //            connection.Close();
        //        }
        //    }
        //    return obj2;
        //}


        public string SelectScalarCommand(string command)
        {
            SqlConnection connection = new SqlConnection(this.conString);
            object obj2 = new object();
            try
            {
                connection.Open();
                SqlCommand command2 = connection.CreateCommand();
                command2.CommandText = command;
                new SqlDataAdapter();
                obj2 = command2.ExecuteScalar();
                return Convert.ToString(obj2);
            }
            catch (Exception exception)
            {
                MyUtilities.WriteToLogFile("SelectScalarCommand : " + exception.Message);
                return "-1";
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            //return "";
        }

        public object SelectScalarProcedure(string procedureName, DBParams @params)
        {
            object obj2 = new object();
            SqlConnection connection = new SqlConnection(this.conString);
            try
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = procedureName;
                command.CommandType = CommandType.StoredProcedure;
                foreach (DBParam param in @params.GetParams())
                {
                    SqlParameter parameter = new SqlParameter(param.Name, param.Value);
                    if (param.HasType())
                    {
                        parameter.DbType = param.MyType;
                    }
                    if (param.HasSize())
                    {
                        parameter.Size = param.Size;
                    }
                    command.Parameters.Add(parameter);
                }
                obj2 = command.ExecuteScalar();
            }
            catch (Exception exception)
            {
                MyUtilities.WriteToLogFile("SelectScalarProcedure : " + exception.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return obj2;
        }

        public int UpdateCommand(string tableName, DBParams updated, DBParams whereParams)
        {
            int num = -1;
            SqlConnection connection = new SqlConnection(this.conString);
            try
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                string str = "update " + tableName + " set ";
                foreach (DBParam param in updated.GetParams())
                {
                    string str2 = str;
                    str = str2 + " " + param.Name + "='" + param.Value.ToString() + "',";
                }
                str = str.Substring(0, str.Length - 1) + " where ";
                foreach (DBParam param2 in whereParams.GetParams())
                {
                    object obj2 = str;
                    str = string.Concat(new object[] { obj2, param2.Name, " = '", param2.Value, "' and " });
                }
                str = str.Substring(0, str.Length - 5);
                command.CommandText = str;
                command.CommandType = CommandType.Text;
                num = command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                MyUtilities.WriteToLogFile("UpdateCommand : " + exception.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return num;
        }





    }

    public class DBParam
    {
        // Fields
        public DbType MyType;
        public string Name;
        public int Size;
        public object Value;

        // Methods

        public DBParam(string Name, object Value)
        {
            this.Name = MyUtilities.Preprocess(Name);
            this.Value = Value;
        }

        public DBParam(string Name, object Value, DbType MyType)
        {
            this.Name = MyUtilities.Preprocess(Name);
            this.Value = Value;
            this.MyType = MyType;
        }

        public DBParam(string Name, object Value, DbType MyType, int Size)
        {
            this.Name = MyUtilities.Preprocess(Name);
            this.Value = Value;
            this.MyType = MyType;
            this.Size = Size;
        }


        private T Cast<T>(object o)
        {
            return (T)o;
        }

        public bool CheckConsistency()
        {
            if (this.HasType())
            {
            }
            return true;
        }

        public bool HasSize()
        {
            return (!this.Size.Equals(DBNull.Value) && (this.Size != 0));
        }

        public bool HasType()
        {
            return !this.MyType.Equals(DBNull.Value);
        }


















    }


    public class DBParams
    {
        // Fields
        public static DBParams Empty;
        private List<DBParam> myParams;

        // Methods
        static DBParams()
        {
            Empty = new DBParams();
        }

        public DBParams()
        {
            this.myParams = new List<DBParam>();
        }


        public void AddParameter(DBParam param)
        {
            if (param.CheckConsistency())
            {
                this.myParams.Add(param);
            }
        }


        public void AddParameter(string Name, object Value)
        {
            this.myParams.Add(new DBParam(Name, Value));
        }

        public void AddParameter(string Name, object Value, DbType Type)
        {
            DBParam item = new DBParam(Name, Value, Type);
            if (item.CheckConsistency())
            {
                this.myParams.Add(item);
            }
        }



        public void AddParameter(string Name, object Value, DbType Type, int Size)
        {
            DBParam item = new DBParam(Name, Value, Type, Size);
            if (item.CheckConsistency())
            {
                this.myParams.Add(item);
            }
        }

        public List<DBParam> GetParams()
        {
            return this.myParams;
        }


















    }

    public class MyUtilities
    {
        // Fields
        private static string filePath;
        public static ArrayList prohibitedStrings;

        // Methods
        static MyUtilities()
        {
            filePath = @"\DBHelper-Log.txt";
            prohibitedStrings = new ArrayList(new string[] {
        @"\", "/", "`", "'", "\"", "<", ">", ".", ",", ":", ";", "!", "?", "*", "-", "+",
        "#", "%", "^", "&", "(", ")", "=", "{", "}", "[", "]", "|"
     });
        }


        public MyUtilities()
        {
        }




        public static string CalculateMD5Hash(string input)
        {
            MD5 md = MD5.Create();
            byte[] bytes = Encoding.ASCII.GetBytes(input);
            byte[] buffer2 = md.ComputeHash(bytes);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < buffer2.Length; i++)
            {
                builder.Append(buffer2[i].ToString("X2"));
            }
            return builder.ToString();
        }


        public static string Preprocess(string teks)
        {
            StringBuilder builder = new StringBuilder(teks.Trim());
            foreach (string str in prohibitedStrings)
            {
                builder.Replace(str, "");
            }
            return builder.ToString();
        }




        public static void WriteToLogFile(string message)
        {
            StreamWriter writer = null;
            try
            {
                writer = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + filePath, true);
                StackTrace trace = new StackTrace();
                trace.GetFrame(1).GetMethod();
                writer.WriteLine("{0} :\n{1}   # {2}\n", DateTime.Now.ToShortDateString() + "-" + DateTime.Now.ToShortTimeString(), trace.ToString(), message);
            }
            catch
            {
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }



    }



}
