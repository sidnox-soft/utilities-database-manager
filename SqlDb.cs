using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace sidnox_soft.utilities.Database
{
    public class SqlDb: IDisposable
    {
        private SqlConnection _sqlConn;
        private SqlTransaction _sqlTran;

        #region init
        /// <summary>
        /// Constructor (open connection)
        /// </summary>
        public SqlDb(string conn_str)
        {
            try
            {
                _sqlConn = new SqlConnection(conn_str);
                _sqlConn.Open();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Destructor (close connection)
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (_sqlTran != null)
                {
                    _sqlTran.Commit();
                    _sqlTran = null;
                }
                _sqlConn.Close();
                _sqlConn = null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region sql
        /// <summary>
        /// Begin MS SQL Transaction
        /// </summary>
        /// <returns>SqlTransaction</returns>
        private void BeginTransaction()
        {
            try
            {
                _sqlTran = _sqlConn.BeginTransaction();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Commit/Rollback MS SQL Transaction
        /// </summary>
        /// <param name="isCommit">bool</param>
        private void CompleteTransaction(bool isCommit)
        {
            try
            {
                if (isCommit) _sqlTran.Commit();
                else _sqlTran.Rollback();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Execute a MS SQL NON-QUERY statement
        /// </summary>
        /// <param name="sQuery">string</param>
        /// <param name="dictParameters">Dictionary</param>
        /// <returns>bool</returns>
        public bool ExecuteNonQuery(string sQuery, Dictionary<string, object> dictParameters = null,
            bool isStoredProc = true)
        {
            try
            {
                if (_sqlTran == null) _sqlTran = _sqlConn.BeginTransaction();
                SqlCommand oCmd = new SqlCommand(sQuery, _sqlConn, _sqlTran);

                if ((dictParameters != null))
                {
                    foreach (KeyValuePair<string, object> Item in dictParameters)
                    {
                        oCmd.Parameters.AddWithValue(Item.Key, Item.Value);
                    }
                }

                oCmd.CommandType = isStoredProc ? CommandType.StoredProcedure : CommandType.Text;

                if (oCmd.ExecuteNonQuery() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {
                _sqlTran.Rollback();
                _sqlTran = null;
                throw;
            }
        }

        public T ExecuteScalar<T>(string sQuery, Dictionary<string, object> dictParameters = null,
            bool isStoredProc = true)
        {
            try
            {
                if (_sqlTran == null) _sqlTran = _sqlConn.BeginTransaction();
                SqlCommand oCmd = new SqlCommand(sQuery, _sqlConn, _sqlTran);

                if ((dictParameters != null))
                {
                    foreach (KeyValuePair<string, object> Item in dictParameters)
                    {
                        oCmd.Parameters.AddWithValue(Item.Key, Item.Value);
                    }
                }

                oCmd.CommandType = isStoredProc ? CommandType.StoredProcedure : CommandType.Text;

                return (T)oCmd.ExecuteScalar();
            }
            catch (Exception)
            {
                _sqlTran.Rollback();
                _sqlTran = null;
                throw;
            }
        }

        /// <summary>
        /// Execute a MS SQL QUERY statement
        /// </summary>
        /// <param name="oSQLConn">SqlConnection</param>
        /// <param name="sQuery">string</param>
        /// <param name="dictParameters">Dictionary</param>
        /// <param name="oSQLTran">SqlTransaction</param>
        /// <returns>DataTable</returns>
        public DataTable ExecuteReader(string sQuery, Dictionary<string, object> dictParameters = null,
            bool isStoredProc = true)
        {
            try
            {
                SqlCommand oCmd;

                if (_sqlTran != null)
                {
                    oCmd = new SqlCommand(sQuery, _sqlConn, _sqlTran);
                }
                else
                {
                    oCmd = new SqlCommand(sQuery, _sqlConn);
                }

                if ((dictParameters != null))
                {
                    foreach (KeyValuePair<string, object> item in dictParameters)
                    {
                        oCmd.Parameters.AddWithValue(item.Key, item.Value);
                    }
                }

                oCmd.CommandType = isStoredProc ? CommandType.StoredProcedure : CommandType.Text;

                SqlDataReader oResult = oCmd.ExecuteReader();

                DataTable dtResult = new DataTable();
                dtResult.Load(oResult);

                return dtResult;
            }
            catch (Exception ex)
            {
                if (_sqlTran != null) _sqlTran.Rollback(); _sqlTran = null;
                throw;
            }
        }
        #endregion
    }
}
