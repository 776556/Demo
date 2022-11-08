using Dapper;
using Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Demo.Service
{
    public class QueryService
    {
        private readonly string _connectionStr = string.Empty;
        public QueryService(string connectionStr) {
            _connectionStr = connectionStr;
        }
        public List<ResponseQueryModel> Query() {
           return Query("");
        }
        public List<ResponseQueryModel> Query(string date) {
            string queryHeadSql = @"SELECT * FROM note_head "+(string.IsNullOrEmpty(date)?"": " WHERE [date]=@date");
            string queryBodySql = @"SELECT * FROM note_body WHERE [date] in @date";
            List<NoetHeadModel> listHeadData = Query<NoetHeadModel>(queryHeadSql,new { date= date });
            var headData = from head in listHeadData select head.date;
            List<NoetBodyModel> listBodyData = Query<NoetBodyModel>(queryBodySql,new { date= headData.ToArray() });
            List<ResponseQueryModel> listResponseQueryModel = new List<ResponseQueryModel>();
            if (listHeadData != null && listHeadData.Count > 0)
            {
                foreach (NoetHeadModel noetHeadModel in listHeadData)
                {
                    var bodyData =  from body in listBodyData
                                    where (body?.date).Equals(noetHeadModel?.date)
                                    select body;
                    ResponseQueryModel responseQueryModel = new ResponseQueryModel();
                    responseQueryModel.Head = noetHeadModel;
                    responseQueryModel.Body = bodyData.ToList();
                    listResponseQueryModel.Add(responseQueryModel);
                }
            }
            return listResponseQueryModel;
        }
        private List<T> Query<T>(string sql) where T : new()
        {
                return Query<T>(sql,"");
        }
        private List<T> Query<T>(string sql ,Object obj) where T : new()
        {
            using (var conn = new System.Data.SQLite.SQLiteConnection(_connectionStr))
            {
                List<T> listData = conn.Query<T>(sql, obj).ToList();
                return listData;
            }
        }
    }
}