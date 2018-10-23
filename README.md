# utilities-database-manager
Disposable SQL Database Manager


How to use?

using (SqlDb _db = new SqlDb(your-connection-string))
{
    string sqlQuery = "your-sql-query-or-stored-procedures-name";
    CommandType commandType = CommandType.StoredProcedure;  # or CommandType.Text;

    Dictionary<string, object> _params = new Dictionary<string, object>();
    _params.Add("@param_01", param_01;
    _params.Add("@param_02", param_02);

    # get bulk data as datatable
    DataTable dt = _db.ExecuteReader(sqlQuery, commandType, _params);

    # update or insert and get single data as int/string/etc..
    int id = _db.ExecuteScalar<int>(sqlQuery, commandType, _params);

    # update or insert and get bool as success or fail
    bool isSuccess = _db.ExecuteNonQuery(sqlQuery, commandType, _params);
}

