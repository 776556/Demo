using Dapper;
using Demo.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace Demo.Controllers
{
    public class DemoController : ApiController
    {
        private string _connectionStr = String.Format(@"Data Source={0}\{1}.db3;Version=3;", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["dbName"]);

        /// <summary>
        /// 查詢
        /// </summary>
        /// <returns></returns>
        // GET api/Demo
        public IHttpActionResult Get()
        {
            try
            {
                using (var conn = new System.Data.SQLite.SQLiteConnection(_connectionStr))
                {
                    string queryHeadSql = @"SELECT * FROM note_head ";
                    string queryBodySql = @"SELECT * FROM note_body WHERE [date]=@date";
                    List<NoetHeadModel> listData = conn.Query<NoetHeadModel>(queryHeadSql).ToList();
                    List<ResponseQueryModel> listResponseQueryModel = new List<ResponseQueryModel>();
                    if (listData!= null && listData.Count>0) {
                        foreach (NoetHeadModel noetHeadModel in listData) {
                            List<NoetBodyModel> listBodyData = conn.Query<NoetBodyModel>(queryBodySql,new {date= noetHeadModel.date }).ToList();
                            ResponseQueryModel responseQueryModel = new ResponseQueryModel();
                            responseQueryModel.Head = noetHeadModel;
                            responseQueryModel.Body = listBodyData;
                            listResponseQueryModel.Add(responseQueryModel);
                        }
                    }
                    return Json(listResponseQueryModel);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 條件查詢
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/Demo/5
        [System.Web.Http.Route("api/Demo/{date}")]
        public IHttpActionResult Get(string date)
        {
            try
            {
                using (var conn = new System.Data.SQLite.SQLiteConnection(_connectionStr))
                {
                    string queryHeadSql = @"SELECT * FROM note_head WHERE [date]=@date";
                    string queryBodySql = @"SELECT * FROM note_body WHERE [date]=@date";
                    List<NoetHeadModel> listData = conn.Query<NoetHeadModel>(queryHeadSql,new { date= date }).ToList();
                    List<ResponseQueryModel> listResponseQueryModel = new List<ResponseQueryModel>();
                    if (listData != null && listData.Count > 0)
                    {
                        foreach (NoetHeadModel noetHeadModel in listData)
                        {
                            List<NoetBodyModel> listBodyData = conn.Query<NoetBodyModel>(queryBodySql, new { date = noetHeadModel.date }).ToList();
                            ResponseQueryModel responseQueryModel = new ResponseQueryModel();
                            responseQueryModel.Head = noetHeadModel;
                            responseQueryModel.Body = listBodyData;
                            listResponseQueryModel.Add(responseQueryModel);
                        }
                    }
                    return Json(listResponseQueryModel);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST api/Demo
        public IHttpActionResult Post([FromBody] RequestInsertModel request)
        {
            try
            { 
                RequestInsertHeadModel head = request?.head;
                List<RequestInsertBodyModel> body = request?.body;
                if (string.IsNullOrEmpty(head?.name)) {
                    throw new Exception("請輸入Name");
                }
                if (body ==null || body.Count==0) {
                    throw new Exception("請輸入一筆remark資料");
                }
                string date = DateTime.Now.ToString("yyyyMMddHHmmss");
                using (var conn = new System.Data.SQLite.SQLiteConnection(_connectionStr))
                {
                    conn.Open();
                    using (IDbTransaction transaction = conn.BeginTransaction())
                    {
                        var insertHeadSql = @" INSERT INTO [note_head]
		                                            ([date]
		                                            ,[name])
	                                            VALUES
		                                            (@date
                                                    ,@name)";
                        NoetHeadModel noetHeadModel = new NoetHeadModel();
                        noetHeadModel.date = date;
                        noetHeadModel.name = head.name;
                        conn.Execute(insertHeadSql, noetHeadModel);
                        var insertBodySql = @" INSERT INTO [note_body]
		                                            ([date]
		                                            ,[remark])
	                                            VALUES
		                                            (@date,
		                                             @remark)";
                        List<NoetBodyModel> listNoetBodyModel = new List<NoetBodyModel>();
                        foreach (RequestInsertBodyModel requestInsertBodyModel in body) {
                            NoetBodyModel noetBodyModel = new NoetBodyModel();
                            noetBodyModel.date = date;
                            noetBodyModel.remark = requestInsertBodyModel.remark;
                            listNoetBodyModel.Add(noetBodyModel);
                        }
                        int count=conn.Execute(insertBodySql, listNoetBodyModel);
                        transaction.Commit();
                        return Json($"成功新增{count}資料");
                    }
                }
                //---------------------------
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // PUT api/Demo/5
        public IHttpActionResult Put([FromBody] RequestUpdateModel request)
        {
            try
            {
                NoetHeadModel head = request?.head;
                List<RequestUpdateBodyModel> body = request?.body;
                if (string.IsNullOrEmpty(head?.date))
                {
                    throw new Exception("date不能為空");
                }
                if (string.IsNullOrEmpty(head?.name))
                {
                    throw new Exception("請輸入Name");
                }
                if (body == null || body.Count == 0)
                {
                    throw new Exception("請輸入一筆body資料");
                }
                var data = from m in body
                           where string.IsNullOrEmpty(m.ID)|| string.IsNullOrEmpty(m.remark)
                           select m;
                if (data!= null && data.Count()>0) {
                    throw new Exception("ID或remark不能為空");
                }
                using (var conn = new System.Data.SQLite.SQLiteConnection(_connectionStr))
                {
                    var updateHeadSql = @"UPDATE [note_head]
	                                      SET [name] = @name
	                                      WHERE date=@date";
                    var updateBodySql = @"UPDATE [note_body]
	                                      SET [remark] = @remark
	                                      WHERE ID=@ID";

                    conn.Execute(updateHeadSql, head);
                    conn.Execute(updateBodySql, body);
                    return Json("修改成功");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE api/Demo/5
        [System.Web.Http.Route("api/Demo/{date}")]
        [System.Web.Http.HttpDelete]
        public IHttpActionResult Delete(string date)
        {
            try
            {
                if (string.IsNullOrEmpty(date))
                {
                    throw new Exception("date 不能為空");
                }
                using (var conn = new System.Data.SQLite.SQLiteConnection(_connectionStr))
                {
                    conn.Open();
                    using (IDbTransaction transaction = conn.BeginTransaction())
                    {
                        var sql = @" DELETE FROM [note_head]
		                             WHERE date=@date;
                                     DELETE FROM [note_body]
		                             WHERE date=@date;
                                   ";
                       int count= conn.Execute(sql, new { date = date });
                       transaction.Commit();
                       return Json($"成功刪除{count}筆資料");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
