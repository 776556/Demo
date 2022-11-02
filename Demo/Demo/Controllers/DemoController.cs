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
            ResponseModel<List<DemoModel>> responseModel = new ResponseModel<List<DemoModel>>();
            try {
                using (var conn = new System.Data.SQLite.SQLiteConnection(_connectionStr))
                {
                    conn.Open();
                    var sql = @" SELECT * FROM Demo";
                    List<DemoModel> listData = conn.Query<DemoModel>(sql).ToList();
                    responseModel.Status = 1;
                    responseModel.retObj = listData;
                    responseModel.errMsg = "";
                }
            } catch (Exception ex) {
                responseModel.Status = 0;
                responseModel.errMsg = ex.Message;
            }
            return Json(responseModel);
        }
        /// <summary>
        /// 條件查詢
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/Demo/5
        public IHttpActionResult Get(string id)
        {
            ResponseModel<List<DemoModel>> responseModel = new ResponseModel<List<DemoModel>>();
            try
            {
                using (var conn = new System.Data.SQLite.SQLiteConnection(_connectionStr))
                {
                    conn.Open();
                    var sql = @" SELECT * FROM Demo WHERE ID=@ID";
                    List<DemoModel> listData = conn.Query<DemoModel>(sql,new { ID= id }).ToList();
                    responseModel.Status = 1;
                    responseModel.retObj = listData;
                    responseModel.errMsg = "";
                }
            }
            catch (Exception ex)
            {
                responseModel.Status = 0;
                responseModel.errMsg = ex.Message;
            }
            return Json(responseModel);
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST api/Demo
        public IHttpActionResult Post([FromBody] List<DemoModel> model)
        {
            ResponseModel<string> responseModel = new ResponseModel<string>();
            try
            {

                var data = from m in model
                           where string.IsNullOrEmpty(m.NAME)
                           select m;
                if (data!=null && data.Count()>0)
                {
                    throw new Exception("NAME 不能為空");
                }
                using (var conn = new System.Data.SQLite.SQLiteConnection(_connectionStr))
                {
                    conn.Open();
                    using (IDbTransaction transaction = conn.BeginTransaction())
                    {
                        var sql = @" INSERT INTO [Demo]
		                                ([NAME])
	                                 VALUES
		                                (@NAME)";
                        conn.Execute(sql, model);
                        transaction.Commit();
                        responseModel.Status = 1;
                        responseModel.errMsg = "";
                    }
                }
            }
            catch (Exception ex)
            {
                responseModel.Status = 0;
                responseModel.errMsg = ex.Message;
            }
            return Json(responseModel);
        }

        // PUT api/Demo/5
        public void Put([FromBody]DemoModel mode)
        {
        
        }

        // DELETE api/Demo/5
        public void Delete(int id)
        {
           
        }
    }
}
